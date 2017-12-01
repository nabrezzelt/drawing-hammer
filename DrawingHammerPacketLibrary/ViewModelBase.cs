using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace DrawingHammerPacketLibrary
{
    [Serializable]
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
