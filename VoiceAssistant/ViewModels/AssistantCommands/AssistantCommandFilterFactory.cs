using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoiceAssistant.UI.Common.Collections.Filter;

namespace VoiceAssistant.ViewModels.AssistantCommands
{
	public partial class AssistantCommandFilterFactory : ObservableObject
	{
		[ObservableProperty]
		private string _name = string.Empty;

		[ObservableProperty]
		private bool _strictName = false;

		[ObservableProperty]
		private int _userConfirmationOptionId = 0;

		[ObservableProperty]
		private int _statusOptionId = 0;

		public ICollectionFilter<AssistantCommandEntity> Create()
		{
			var name = Name[..];
			var strictName = StrictName;
			var userConfirmationOptionId = UserConfirmationOptionId;
			var statusOptionId = StatusOptionId;

			return new FuncCollectionFilter<AssistantCommandEntity>((action) =>
			{
				var firstFlag = strictName ? name == action.Name : action.Name.StartsWith(name);

				var secondFlag = userConfirmationOptionId switch
				{
					1 => !action.NeedsConfirmation, // doesn't need confirmation
					2 => action.NeedsConfirmation,  // does need confirmation
					_ => true                       // both
				};

				var thirdFlag = statusOptionId switch
				{
					1 => action.IsEnabled,          // enabled only
					2 => !action.IsEnabled,         // disabled only
					_ => true                       // both
				};

				return firstFlag && secondFlag && thirdFlag;
			});
		}

		public void Clear()
		{
			Name = string.Empty;
			StrictName = false;
			UserConfirmationOptionId = 0;
			StatusOptionId = 0;
		}
	}
}
