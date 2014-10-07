using System;
using System.Collections.Generic;

namespace BlinkStickClient.Classes
{
    public class Pattern
    {
        public string Name
        {
            get;
            set;
        }

        public List<Animation> Animations = new List<Animation>();

        public Pattern()
        {
            Animations.Add(new Animation());
        }

        public Pattern(String name)
        {
            this.Name = name;
        }
    }
}

