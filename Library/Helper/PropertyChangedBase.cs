using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Library
{
    public class PropertyChangedBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public bool RaisePropertyChanged<T>(ref T member, T val, [CallerMemberName] string propertyName = null)
        {
            if (Equals(member, val))
                return false;

            member = val;
            RaisePropertyChanged(propertyName);
            return true;
        }
    }
}
