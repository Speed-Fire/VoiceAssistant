using Microsoft.Extensions.Options;
using Plugin.S2T.Base;
using Plugin.S2T.VK.Exceptions;
using Plugin.S2T.VK.Extensions;
using Plugin.S2T.VK.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using VoiceAssistant.Core.Interfaces;
using VoiceAssistant.Core.Misc;

namespace Plugin.S2T.VK
{
	public class S2TConverterVK(
		HttpClient client,
		IOptionsMonitor<ConverterSettings> settings) : IS2TConverter
	{
		private readonly HttpClient _client = client;
		private readonly IOptionsMonitor<ConverterSettings> _settings = settings;

		private UploadUrlInfo? _uploadUrlInfo;

		public Task<OneOf<string, Exception>> Convert(Stream input, int timeot = 3000)
		{
			return Task.Run(() => ConvertInternal(input, timeot));
		}

		private async Task<OneOf<string, Exception>> ConvertInternal(Stream input, int timeot = 3000)
		{
			var uploadUrl_res = await GetAudioUploadURL();
			if (!uploadUrl_res.IsFirst)
				return uploadUrl_res;

			var audioInfo_res = await SendAudioToServer(uploadUrl_res.First, input);
			if (!audioInfo_res.IsFirst)
				return audioInfo_res;

			var taskId_res = await StartAudioRecognition(audioInfo_res.First);
			if(!taskId_res.IsFirst)
				return taskId_res;

			var request_count = 3;

			do
			{
				request_count++;
				var status_res = await CheckRecognitionStatus(taskId_res.First);
				if(!status_res.IsFirst)
					return status_res;

				if (!string.IsNullOrEmpty(status_res.First))
				{
					return status_res;
				}

				if (request_count < 5)
					await Task.Delay(200);
				else
				{
					request_count = 0;
					await Task.Delay(1000);
				}
			}while (true);
		}

		#region GetAudioUploadURL

		private async Task<OneOf<string, Exception>> GetAudioUploadURL()
		{
			if (_uploadUrlInfo is not null &&
				_uploadUrlInfo.Date == DateOnly.FromDateTime(DateTime.Now))
				return new OneOf<string, Exception>(_uploadUrlInfo.Url);

			var res = await GetAudioUploadURL_API();

			if (res.IsFirst)
			{
				var dt = DateOnly.FromDateTime(DateTime.Now);

				var item = new UploadUrlInfo()
				{
					Date = dt,
					Url = res.First
				};

				_uploadUrlInfo = item;
			}

			return res;
		}

		private async Task<OneOf<string, Exception>> GetAudioUploadURL_API()
		{
			if (_settings.CurrentValue is null)
				throw new ServiceKeyNotSpecifiedException();

			using var response = await _client
				.GetAsync($"https://api.vk.com/method/asr.getUploadUrl?access_token={_settings.CurrentValue.ServiceApiKey}&v=5.236");

			if (!response.IsSuccessStatusCode)
				return new OneOf<string, Exception>(new InternalServerErrorException());
			
			var json = await response.Content.ReadAsStreamAsync();

			var obj = await JsonNode.ParseAsync(json);

			Exception? exception;
			string result = string.Empty;

			if(obj is null)
			{
				exception = new InternalServerErrorException();
			}
#pragma warning disable CS8602 // Разыменование вероятной пустой ссылки.
			else if (obj["response"] is not null)
			{
				return new OneOf<string, Exception>(obj["response"]["upload_url"].ToString());
			}
			else if (obj["error"] is not null)
			{
				exception =  new Exception(obj["error"]["error_msg"].ToString());
			}
			else
			{
				exception = new InternalServerErrorException();
			}
#pragma warning restore CS8602 // Разыменование вероятной пустой ссылки.

			if (exception is not null)
				return new OneOf<string, Exception>(exception);
			else
				return new OneOf<string, Exception>(result);
		}

		#endregion

		#region SendAudioToServer

		private async Task<OneOf<string, Exception>> SendAudioToServer(string uploadUrl, Stream audio)
		{
			using var content = new MultipartFormDataContent();

			using var streamContent = new StreamContent(audio);
			streamContent.Headers.ContentType =
				new System.Net.Http.Headers.MediaTypeHeaderValue("audio/wav");

			var filename = Guid.NewGuid().ToString() + ".wav";
			content.Add(streamContent, "file", filename);

			using var response = await _client.PostAsync(uploadUrl, content);
			if(!response.IsSuccessStatusCode)
				return new OneOf<string, Exception>(new InternalServerErrorException());

			var json = await response.Content.ReadAsStreamAsync();

			var obj = await JsonNode.ParseAsync(json);
			if(obj is null)
			{
				return new OneOf<string, Exception>(new InternalServerErrorException());
			}

			Exception? exception = null;
			var audioInfo = string.Empty;

			if (obj["error_msg"] is not null)
			{
#pragma warning disable CS8602 // Разыменование вероятной пустой ссылки.
				exception = new(obj["error_msg"].ToString());
#pragma warning restore CS8602 // Разыменование вероятной пустой ссылки.
			}
			else if (obj["hash"] is not null)
			{
				audioInfo = obj.ToString();
			}
			else
			{
				exception = new InternalServerErrorException();
			}

			if (exception is not null)
				return new OneOf<string, Exception>(exception);
			else
				return new OneOf<string, Exception>(audioInfo);
		}

		#endregion

		#region StartAudioRecognition

		private async Task<OneOf<string, Exception>> StartAudioRecognition(string audioInfo)
		{
			if (_settings.CurrentValue is null)
				throw new ServiceKeyNotSpecifiedException();

			using var multipart = new MultipartFormDataContent();

			using var access_token = new StringContent(_settings.CurrentValue.ServiceApiKey);
			multipart.Add(access_token, "access_token");

			using var model = new StringContent("spontaneous");
			multipart.Add(model, "model");

			using var audio = new StringContent(audioInfo);
			multipart.Add(audio, "audio");

			using var response = await _client
				.PostAsync("https://api.vk.com/method/asr.process?v=5.236", multipart);

			if (!response.IsSuccessStatusCode)
				return new OneOf<string, Exception>(new InternalServerErrorException());

			var json = await response.Content.ReadAsStreamAsync();
			var obj = await JsonNode.ParseAsync(json);
			if (obj is null)
				return new OneOf<string, Exception>(new InternalServerErrorException());

			Exception? exception = null;
			var taskId = string.Empty;

#pragma warning disable CS8602 // Разыменование вероятной пустой ссылки.
			if (obj["error"] is not null)
			{
				var err_code = obj["error"]["error_code"].ToString();

				exception = err_code switch
				{
					"7701" => new AudioLimitExceededException(),
					"7702" => new TooLongAudioException(),
					"7703" => new AudioNotFoundException(),
					_ => new InternalServerErrorException(),
				};
			}
			else if (obj["response"] is not null)
			{
				taskId = obj["response"]["task_id"].ToString();
			}
#pragma warning restore CS8602 // Разыменование вероятной пустой ссылки.

			if (exception is not null)
				return new OneOf<string, Exception>(exception);
			else
				return new OneOf<string, Exception>(taskId);
		}

		#endregion

		#region CheckRecognitionStatus

		private async Task<OneOf<string, Exception>> CheckRecognitionStatus(string taskId)
		{
			if (_settings.CurrentValue is null)
				throw new ServiceKeyNotSpecifiedException();

			using var multipart = new MultipartFormDataContent();

			using var access_token = new StringContent(_settings.CurrentValue.ServiceApiKey);
			multipart.Add(access_token, "access_token");

			using var task_id = new StringContent(taskId);
			multipart.Add(task_id, "task_id");

			using var response = await _client
				.PostAsync($"https://api.vk.com/method/asr.checkStatus?v=5.236", multipart);

			if (!response.IsSuccessStatusCode)
				return new OneOf<string, Exception>(new InternalServerErrorException());

			var json = await response.Content.ReadAsStreamAsync();
			var obj = await JsonNode.ParseAsync(json);
			if (obj is null)
				return new OneOf<string, Exception>(new InternalServerErrorException());

			Exception? exception = null;
			var text = string.Empty;

#pragma warning disable CS8602 // Разыменование вероятной пустой ссылки.
			if (obj["error"] is not null)
			{
				exception = new(obj["error"]["error_msg"].ToString());
			}
			else if (obj["response"] is not null)
			{
				var status = obj["response"]["status"].ToString();

				switch (status)
				{
					case "processing":
					case "finished":
						text = obj["response"]["text"].ToString();
						break;
					case "transcoding_error":
						exception = new AudioTranscodingException();
						break;
					case "recognition_error":
						exception = new SpeechRecognitionException();
						break;
					case "internal_error":
					default:
						exception = new InternalServerErrorException();
						break;
				}
			}
			else
			{
				exception = new InternalServerErrorException();
			}
#pragma warning restore CS8602 // Разыменование вероятной пустой ссылки.

			if (exception is not null)
				return new OneOf<string, Exception>(exception);
			else
				return new OneOf<string, Exception>(text);
		}

		#endregion

		#region Dispose

		public void Dispose()
		{

		}

		#endregion
	}
}
