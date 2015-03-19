using System;
using System.Collections.Generic;

namespace BlinkStickClient.DataModel
{
    public class Pattern
    {
        public string Name
        {
            get;
            set;
        }

        public int LedFirstIndex { get; set; }
        public int LedLastIndex { get; set; }

        public List<Animation> Animations = new List<Animation>();

        public Pattern()
        {
            this.Name = "";
        }

        public Pattern(String name)
        {
            this.Name = name;
            this.LedFirstIndex = 0;
            this.LedLastIndex = 0;
        }

        public override String ToString()
        {
            return this.Name;
        }

        public void Assign(Pattern pattern)
        {
            this.LedFirstIndex = pattern.LedFirstIndex;
            this.LedLastIndex = pattern.LedLastIndex;

            foreach (Animation animation in pattern.Animations)
            {
                Animation newAnimation = new Animation();
                newAnimation.Assign(animation);
                this.Animations.Add(newAnimation);
            }
        }
    }
}

