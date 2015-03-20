using NUnit.Framework;
using System;
using BlinkStickClient.DataModel;
using BlinkStickDotNet;

namespace BlinkStickClient.Tests
{
	[TestFixture ()]
	public class TestSetColorAnimation
	{
		DateTime StartTime;
		Animation animation;
		RgbColor targetColor;

		[SetUp]
		public void Init()
		{
			StartTime = DateTime.Now;

			animation = new Animation () { 
				AnimationType = AnimationTypeEnum.SetColor,
				DelaySetColor = 100
			};

			targetColor = RgbColor.FromRgb (255, 0, 0);

			animation.Color = targetColor;
		}

		[Test]
		public void ColorShouldBeSameAtStartOfAnimation ()
		{
			RgbColor newColor = animation.GetColor (StartTime, StartTime);

			Assert.AreEqual (targetColor.ToString(), newColor.ToString()); 
		}

		[Test]
		public void ColorShouldBeSameAtEndOfAnimation ()
		{
			RgbColor newColor = animation.GetColor (StartTime, StartTime.AddMilliseconds(animation.DelaySetColor));

			Assert.AreEqual (targetColor.ToString(), newColor.ToString()); 
		}

		[Test]
		public void ColorShouldBeSameDuringAnimation ()
		{
			for (int i = 0; i < animation.DelaySetColor; i++) {
				RgbColor newColor = animation.GetColor (StartTime, StartTime.AddMilliseconds (i));

				Assert.AreEqual (targetColor.ToString (), newColor.ToString ()); 
			}
		}

		[Test]
		public void ColorShouldBeSameInTheMiddleOfAnimation ()
		{
			RgbColor newColor = animation.GetColor (StartTime, StartTime.AddMilliseconds(animation.DelaySetColor / 2));

			Assert.AreEqual (targetColor.ToString(), newColor.ToString()); 
		}

		[Test]
		public void AfterAnimationHasFinished ()
		{
			RgbColor newColor = animation.GetColor (StartTime, StartTime.AddMilliseconds(animation.DelaySetColor + 1));

			Assert.IsTrue(animation.AnimationFinished);
		}
	}
}

