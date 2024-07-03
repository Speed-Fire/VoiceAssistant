using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoiceAssistant.Common
{
    public class Provider<T> : INotifyPropertyChanged
        where T : class
    {
        private T? _value;
        public T? Value
        {
            get => _value;
            set
            {
                if (_value == value)
                    return;

                if(_value is not null && _value is IDisposable disposable)
                    disposable.Dispose();

                _value = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Value)));
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
    }
}
