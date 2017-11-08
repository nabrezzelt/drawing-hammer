using System;

namespace DrawingHammerPacketLibrary
{
    [Serializable]
    public class Word : ViewModelBase
    {
        private int _id;
        private string _value;

        public int Id
        {
            get => _id;
            set
            {
                _id = value;
                OnPropertyChanged();
            }
        }

        public string Value
        {
            get => _value;
            set
            {
                _value = value;
                OnPropertyChanged();
            }
        }
        
        public Word(int id, string value)
        {
            _id = id;
            Value = value;
        }
    }
}