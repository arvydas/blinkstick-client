using NUnit.Framework;
using System;
using BlinkStickClient.DataModel;
using BlinkStickDotNet;

namespace BlinkStickClient.Tests
{
	[TestFixture ()]
	public class TestPulseAnimation
	{
		DateTime StartTime;
		Animation animation;
		RgbColor targetColor;

		[SetUp]
		public void Init()
		{
			StartTime = DateTime.Now;

			animation = new Animation () { 
				AnimationType = AnimationTypeEnum.Pulse,
				DurationPulse = 512 //Easier number for calculations
			};

			targetColor = RgbColor.FromRgb (255, 0, 0);
			animation.Color = targetColor;
		}

		[Test]
		public void StartsWithBlack ()
		{
			RgbColor newColor = animation.GetColor (StartTime, StartTime);

			Assert.AreEqual(RgbColor.Black().ToString (), newColor.ToString()); 
		}

		[Test]
		public void HalfBrightnessQuarterWay ()
		{
			RgbColor newColor = animation.GetColor (StartTime, StartTime.AddMilliseconds(animation.DurationPulse / 4));

			Assert.AreEqual(RgbColor.FromRgb(128,0,0).ToString(), newColor.ToString()); 
		}

		[Test]
		public void HalfBrightnessThreeQuarterWay ()
		{
			RgbColor newColor = animation.GetColor (StartTime, StartTime.AddMilliseconds(animation.DurationPulse / 4 * 3));

			Assert.AreEqual(RgbColor.FromRgb(127,0,0).ToString(), newColor.ToString()); 
		}

		[Test]
		public void TestFadeIn ([Range(0, 255)] int time)
		{
			RgbColor newColor = animation.GetColor (StartTime, StartTime.AddMilliseconds(time));

			Assert.AreEqual(RgbColor.FromRgb(time,0,0).ToString(), newColor.ToString()); 
		}

		[Test]
		public void TestFadeOut ([Range(256, 511)] int time)
		{
			RgbColor newColor = animation.GetColor (StartTime, StartTime.AddMilliseconds(time));

			Assert.AreEqual(RgbColor.FromRgb(511 - time,0,0).ToString(), newColor.ToString()); 
		}

		[Test]
		public void TargetColorHalfWay ()
		{
			RgbColor newColor = animation.GetColor (StartTime, StartTime.AddMilliseconds(animation.DurationPulse / 2));

			Assert.AreEqual(targetColor.ToString (), newColor.ToString()); 
		}

		[Test]
		public void BlackAtTheEnd ()
		{
			RgbColor newColor = animation.GetColor (StartTime, StartTime.AddMilliseconds(animation.DurationPulse));

			Assert.AreEqual(RgbColor.Black().ToString (), newColor.ToString()); 
		}

		[Test]
		public void TargetColorHalfWaySecondPulse ()
		{
			animation.RepeatPulse = 2;
			RgbColor newColor = animation.GetColor (StartTime, StartTime.AddMilliseconds(animation.DurationPulse + animation.DurationPulse / 2));

			Assert.AreEqual(targetColor.ToString (), newColor.ToString()); 
		}

		[Test]
		public void BlackAtTheEndSecondPulse ()
		{
			animation.RepeatPulse = 2;
			RgbColor newColor = animation.GetColor (StartTime, StartTime.AddMilliseconds(animation.DurationPulse * 2));

			Assert.AreEqual(RgbColor.Black().ToString (), newColor.ToString()); 
			Assert.IsTrue(animation.AnimationFinished);
		}


		[Test]
		public void BeforeAnimationHasFinishedSecondTime ()
		{
			animation.RepeatPulse = 2;
			RgbColor newColor = animation.GetColor (StartTime, StartTime.AddMilliseconds(animation.DurationPulse * 2 - 1));
			Assert.IsTrue(!animation.AnimationFinished);
		}

		[Test]
		public void AfterAnimationHasFinishedSecondTime ()
		{
			animation.RepeatPulse = 2;
			RgbColor newColor = animation.GetColor (StartTime, StartTime.AddMilliseconds(animation.DurationPulse * 2 + 1));
			Assert.IsTrue(animation.AnimationFinished);
		}

		[Test]
		public void AfterAnimationHasFinished ()
		{
			RgbColor newColor = animation.GetColor (StartTime, StartTime.AddMilliseconds(animation.DurationPulse + 1));
			Assert.IsTrue(animation.AnimationFinished);
		}

		[Test]
		public void BlackColorBeyondTimeAndFinished ()
		{
			RgbColor newColor = animation.GetColor (StartTime, StartTime.AddMilliseconds(animation.DurationPulse * 10), RgbColor.Black());

			Assert.AreEqual(RgbColor.Black().ToString(), newColor.ToString()); 
			Assert.IsTrue(animation.AnimationFinished);
		}
	}
}

