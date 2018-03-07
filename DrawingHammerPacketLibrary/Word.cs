using System;

namespace DrawingHammerPacketLibrary
{
    [Serializable]
    public class Word : ViewModelBase
    {      
        public int Id { get; }

        public string Value { get; }        
        
        public Word(int id, string value)
        {
            Id = id;
            Value = value;
        }
    }
}