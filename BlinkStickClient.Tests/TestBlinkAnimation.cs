using NUnit.Framework;
using System;
using BlinkStickClient.DataModel;
using BlinkStickDotNet;

namespace BlinkStickClient.Tests
{
	[TestFixture ()]
	public class TestBlinkAnimation
	{
		DateTime StartTime;
		Animation animation;
		RgbColor targetColor;

		[SetUp]
		public void Init()
		{
			StartTime = DateTime.Now;

			animation = new Animation () { 
				AnimationType = AnimationTypeEnum.Blink,
				DurationBlink = 100
			};

			targetColor = RgbColor.FromRgb (255, 0, 0);
			animation.Color = targetColor;
		}

		[Test]
		public void ColorShouldBeSameAtStartOfAnimation ()
		{
			RgbColor newColor = animation.GetColor (StartTime, StartTime);

			Assert.AreEqual (targetColor.ToString(), newColor.ToString()); 
		
			Assert.IsTrue(!animation.AnimationFinished);
		}

		[Test]
		public void TurnOnFirstHalf ()
		{
			for (int i = 0; i < animation.DurationBlink / 2; i++) {
				RgbColor newColor = animation.GetColor (StartTime, StartTime.AddMilliseconds(i));

				Assert.AreEqual (targetColor.ToString (), newColor.ToString ()); 
			}
			Assert.IsTrue(!animation.AnimationFinished);
		}

		[Test]
		public void TurnOffHalfWayThrough ()
		{
			for (int i = animation.DurationBlink / 2; i < animation.DurationBlink; i++) {
				RgbColor newColor = animation.GetColor (StartTime, StartTime.AddMilliseconds(i));

				Assert.AreEqual (RgbColor.Black ().ToString (), newColor.ToString ()); 
			}
			Assert.IsTrue(!animation.AnimationFinished);
		}

		[Test]
		public void TurnOnSecondBlink ()
		{
			animation.RepeatBlink = 2;
			for (int i = animation.DurationBlink; i < animation.DurationBlink * 2 - animation.DurationBlink / 2; i++) {
				RgbColor newColor = animation.GetColor (StartTime, StartTime.AddMilliseconds(i));

				Assert.AreEqual (targetColor.ToString (), newColor.ToString ()); 
			}
			Assert.IsTrue(!animation.AnimationFinished);
		}


		[Test]
		public void TurnOffHalfWayThroughSecondBlink ()
		{
			animation.RepeatBlink = 2;
			for (int i = animation.DurationBlink + animation.DurationBlink / 2; i < animation.DurationBlink * 2; i++) {
				RgbColor newColor = animation.GetColor (StartTime, StartTime.AddMilliseconds(i));

				Assert.AreEqual (RgbColor.Black ().ToString (), newColor.ToString ()); 
			}
			Assert.IsTrue(!animation.AnimationFinished);
		}

		[Test]
		public void AfterAnimationHasFinishedTwice ()
		{
			animation.RepeatBlink = 2;
			RgbColor newColor = animation.GetColor (StartTime, StartTime.AddMilliseconds(animation.DurationBlink * 2 + 1));
			Assert.IsTrue(animation.AnimationFinished);
		}

		[Test]
		public void AfterAnimationHasFinished ()
		{
			RgbColor newColor = animation.GetColor (StartTime, StartTime.AddMilliseconds(animation.DurationBlink + 1));
			Assert.IsTrue(animation.AnimationFinished);
		}

		[Test]
		public void AfterAnimationHasFinishedBeyondTime ()
		{
			RgbColor newColor = animation.GetColor (StartTime, StartTime.AddMilliseconds(animation.DurationBlink * 10));
			Assert.AreEqual (RgbColor.Black ().ToString (), newColor.ToString ()); 
			Assert.IsTrue(animation.AnimationFinished);
		}
	}
}

