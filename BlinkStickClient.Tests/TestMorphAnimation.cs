using NUnit.Framework;
using System;
using BlinkStickClient.DataModel;
using BlinkStickDotNet;

namespace BlinkStickClient.Tests
{
    [TestFixture]
    public class TestMorphAnimation
    {
        DateTime StartTime;
        Animation animation;
        RgbColor targetColor;
        RgbColor refColor;

        [SetUp]
        public void Init()
        {
            StartTime = DateTime.Now;

            animation = new Animation () { 
                AnimationType = AnimationTypeEnum.Morph,
                DurationMorph = 256 //Easier number for calculations
            };

            targetColor = RgbColor.FromRgb (255, 0, 0);
            animation.Color = targetColor;
            refColor = RgbColor.FromRgb(0, 255, 0);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ThrowsExceptionIfReferenceColorNotSet ()
        {
            RgbColor newColor = animation.GetColor (StartTime, StartTime.AddMilliseconds(animation.DurationMorph));
        }

        [Test]
        public void StartWithReferenceColor ()
        {
            RgbColor newColor = animation.GetColor (StartTime, StartTime, refColor);

            Assert.AreEqual(refColor.ToString (), newColor.ToString()); 
        }

        [Test]
        public void GreenToRed ([Range(0, 255)] int time)
        {
            refColor = RgbColor.FromRgb(0, 255, 0);
            RgbColor newColor = animation.GetColor (StartTime, StartTime.AddMilliseconds(time), refColor);

            Assert.AreEqual(RgbColor.FromRgb(time, 255 - time, 0).ToString(), newColor.ToString()); 
        }

        [Test]
        public void RedToBlue ([Range(0, 255)] int time)
        {
            refColor = RgbColor.FromRgb(255, 0, 0);
            targetColor = RgbColor.FromRgb (0, 0, 255);
            animation.Color = targetColor;

            RgbColor newColor = animation.GetColor (StartTime, StartTime.AddMilliseconds(time), refColor);

            Assert.AreEqual(RgbColor.FromRgb(255 - time, 0, time).ToString(), newColor.ToString()); 
        }

        [Test]
        public void BlueToGreen ([Range(0, 255)] int time)
        {
            refColor = RgbColor.FromRgb(0, 0, 255);
            targetColor = RgbColor.FromRgb (0, 255, 0);
            animation.Color = targetColor;
            RgbColor newColor = animation.GetColor (StartTime, StartTime.AddMilliseconds(time), refColor);

            Assert.AreEqual(RgbColor.FromRgb(0, time, 255 - time).ToString(), newColor.ToString()); 
            Assert.IsTrue(!animation.AnimationFinished);
        }

        [Test]
        public void TargetColorAtTheEnd ()
        {
            RgbColor newColor = animation.GetColor (StartTime, StartTime.AddMilliseconds(animation.DurationMorph), RgbColor.Black());

            Assert.AreEqual(targetColor.ToString (), newColor.ToString()); 
        }

        [Test]
        public void TargetColorBeyondTimeAndFinished ()
        {
            RgbColor newColor = animation.GetColor (StartTime, StartTime.AddMilliseconds(animation.DurationMorph * 10), RgbColor.Black());

            Assert.AreEqual(targetColor.ToString (), newColor.ToString()); 
            Assert.IsTrue(animation.AnimationFinished);
        }

        [Test]
        public void AfterAnimationHasFinished ()
        {
            RgbColor newColor = animation.GetColor (StartTime, StartTime.AddMilliseconds(animation.DurationMorph), RgbColor.Black());
            Assert.IsTrue(animation.AnimationFinished);
        }
    }
}

