using System;

namespace BlinkStickClient.Classes
{
    public class Pattern
    {
        public string Name
        {
            get;
            set;
        }

        public Pattern()
        {
        }

        public Pattern(String name)
        {
            this.Name = name;
        }
    }
}

