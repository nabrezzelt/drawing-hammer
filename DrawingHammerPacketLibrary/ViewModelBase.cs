using System;
using System.ComponentModel;

namespace DrawingHammerPacketLibrary
{
    [Serializable]
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
