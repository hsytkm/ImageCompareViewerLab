using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ShapeIcons.ViewModels
{
    // https://blog.okazuki.jp/entry/2015/05/09/124333
    class BindableBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual bool SetProperty<T>(ref T field, T value, [CallerMemberName]string propertyName = null)
        {
            if (Equals(field, value)) return false;
            field = value;
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            return true;
        }
    }
}
