using System;
using System.ComponentModel;

namespace DrawingHammerPackageLibrary
{
    [Serializable]
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
