using CommunityToolkit.Mvvm.ComponentModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace HandfulOfBreads.ViewModels
{
    //INotifyPropertyChanged
    public class BaseViewModel :  ObservableObject
    {
        //public event PropertyChangedEventHandler PropertyChanged;

        //protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        //{
        //    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        //}

        //protected bool SetProperty<T>(ref T backingStore, T value, [CallerMemberName] string propertyName = "")
        //{
        //    if (EqualityComparer<T>.Default.Equals(backingStore, value))
        //        return false;

        //    backingStore = value;
        //    OnPropertyChanged(propertyName);
        //    return true;
        //}
    }
}
