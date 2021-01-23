using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Apollo.Terminal.Common
{
    public abstract class NotifyPropertyChanged : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public void Set<T>(ref T field, T value, [CallerMemberName]string propertyName = "")
        {
            if (!EqualityComparer<T>.Default.Equals(value, field))
            {
                field = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        protected void TriggerPropertyChanged(object sender, [CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
