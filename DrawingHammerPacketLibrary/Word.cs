using System;

namespace DrawingHammerPacketLibrary
{
    [Serializable]
    public class Word : ViewModelBase
    {      
        public int Id { get; set; }

        public string Value { get; set; }        
        
        public Word(int id, string value)
        {
            Id = id;
            Value = value;
        }
    }
}