using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoiceAssistant.ViewModels.AssistantCommands
{
	public partial class AssistantScriptParameter : ObservableValidator
	{
		private readonly AssistantScriptParametersBinderVM o_binder;
		private readonly Func<string, bool> _parseFunction;

		private string _value = string.Empty;

		[CustomValidation(typeof(AssistantScriptParameter), nameof(ValidateParameter))]
		public string Value
		{
			get => _value;
			set => SetProperty(ref _value, value, true);
		}

		public string Title { get; }
		public Type IntendedType { get; }

		[RelayCommand]
		private void Clear()
		{
			Value = IntendedType == typeof(bool) ? bool.FalseString : string.Empty;
		}

		#region Constructors

		public AssistantScriptParameter(
			string value,
			string title,
			Type intendedType,
			AssistantScriptParametersBinderVM binder)
			: this(title, intendedType, binder)
		{
			_value = value;
		}

		public AssistantScriptParameter(
			string title,
			Type intendedType,
			AssistantScriptParametersBinderVM binder)
		{
			Title = title;
			IntendedType = intendedType;
			o_binder = binder;

			if (intendedType == typeof(TimeSpan))
			{
				_parseFunction = str => TimeSpan.TryParse(str, out _);
			}
			else
			{
				var typeCode = Type.GetTypeCode(intendedType);

				_parseFunction = typeCode switch
				{
					TypeCode.Boolean => str => bool.TryParse(str, out _),
					TypeCode.SByte or TypeCode.Byte or
						TypeCode.Int16 or TypeCode.UInt16 or
						TypeCode.Int32 or TypeCode.UInt32 or
						TypeCode.Int64 or TypeCode.UInt64 => str => int.TryParse(str, out _),
					TypeCode.Single or TypeCode.Double or
						TypeCode.Decimal => str => double.TryParse(str, out _),
					TypeCode.DateTime => str => DateTime.TryParse(str, out _),
					TypeCode.String => str => true,
					_ => throw new Exception("Unsupported type!"),
				};

				if(typeCode == TypeCode.Boolean)
				{
					_value = bool.FalseString;
				}
			}
		}

		#endregion

		#region Validation

		public void Validate() => this.ValidateAllProperties();

		public static ValidationResult ValidateParameter(string str, ValidationContext context)
		{
			var value = str.Trim();

			if (string.IsNullOrEmpty(value))
				return new($"Property \"{nameof(Value)}\" can't be null or empty!");

			var instance = (AssistantScriptParameter)context.ObjectInstance;

			var binder = instance.o_binder;
			if (value.Length >= 2 &&
				value[0] == '{' && value[^1] == '}')
			{
				var inputs = binder.EntityInputs;

				var paramName = value[1..^1];

				if (inputs.Contains(paramName))
					return ValidationResult.Success!;
				else
					return new("Unknown parameter!");
			}

			var parseFunc = instance._parseFunction;
			if (parseFunc.Invoke(value))
				return ValidationResult.Success!;
			else
				return new("Can't parse value to intended type!");
		}

		#endregion
	}
}
