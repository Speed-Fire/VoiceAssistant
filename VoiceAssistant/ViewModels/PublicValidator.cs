using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace VoiceAssistant.ViewModels
{
    public abstract class PublicValidator : ObservableValidator
    {
        public void ValidateAll()
        {
            ValidateAllProperties();
        }

        protected static T GetResource<T>(object key)
        {
            return (T)Application.Current.FindResource(key);
        }

        protected static T? TryGetResource<T>(object key)
        {
            return (T?)Application.Current.TryFindResource(key);
        }
    }
}
