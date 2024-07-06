using CommunityToolkit.Mvvm.ComponentModel;
using PluginsSystem.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PluginsSystem.Settings.ViewModels
{
	public partial class BindableProperty : ObservableValidator
	{
		private readonly Regex? _regex;
		private string? _value;

		[CustomValidation(typeof(BindableProperty), nameof(Validate))]
		public string? Value
		{
			get => _value;
			set=> SetProperty(ref _value, value, true);
		}

		public bool Changed { get; private set; }
		public ParameterInfoEntity ParameterInfo { get; }

		public BindableProperty(ParameterInfoEntity parameterInfo, string value)
		{
			_value = value;
			ParameterInfo = parameterInfo;

			PropertyChanged += (sender, e) =>
			{
				Changed = true;
			};

			_regex = GetRegex(parameterInfo.ParameterType);
		}

		public static ValidationResult Validate(string? value, ValidationContext context)
		{
			var property = context.ObjectInstance as BindableProperty;
			if (property is null)
				return new ValidationResult("Incorrect type!");

			var regex = property._regex;

			if (property.ParameterInfo.ParameterType == typeof(string) ||
				(!string.IsNullOrWhiteSpace(value) && value.Length <= 20 &&
				(regex is null || regex.IsMatch(value!))))
			{
				return ValidationResult.Success!;
			}
			else
			{
				return new ValidationResult("Value cannot be null or empty!");
			}
		}

		private static Regex? GetRegex(Type type)
		{
			var typeCode = Type.GetTypeCode(type);

			switch (typeCode)
			{
				case TypeCode.Decimal:
				case TypeCode.Int64:
				case TypeCode.Int32:
				case TypeCode.Int16:
				case TypeCode.SByte:
					return GetIntegerRegex();

				case TypeCode.UInt64:
				case TypeCode.UInt32:
				case TypeCode.UInt16:
				case TypeCode.Byte:
					return GetUIntegerRegex();

				case TypeCode.Single:
				case TypeCode.Double:
					return GetFloatRegex();

				default:
					return null;
			}
		}

		[GeneratedRegex("^((-?[1-9][0-9]*)|0)$")]
		private static partial Regex GetIntegerRegex();

		[GeneratedRegex("^(([1-9][0-9]*)|0)$")]
		private static partial Regex GetUIntegerRegex();

		[GeneratedRegex("^-?([0-9]+([.][0-9]*)?|[.][0-9]+)$")]
		private static partial Regex GetFloatRegex();
	}
}
