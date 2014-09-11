#region License
// Copyright 2013 by Agile Innovative Ltd
//
// This file is part of BlinkStick application.
//
// BlinkStick application is free software: you can redistribute 
// it and/or modify it under the terms of the GNU General Public License as published 
// by the Free Software Foundation, either version 3 of the License, or (at your option) 
// any later version.
//		
// BlinkStick application is distributed in the hope that it will be useful, but 
// WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or
// FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License along with 
// BlinkStick application. If not, see http://www.gnu.org/licenses/.
#endregion

using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using BlinkStick.Classes;
using BlinkStick.Hid;

namespace BlinkStick
{
	public class LedController
	{
		#region Events
		public event EventHandler BlinkStart;
		public BlinkstickHid Device;
		
		protected void OnBlinkStart()
		{
			if (BlinkStart != null)
			{
				BlinkStart(this, new EventArgs());
			}
		}

		public event EventHandler BlinkEnd;
		
		protected void OnBlinkEnd()
		{
			if (BlinkEnd != null)
			{
				BlinkEnd(this, new EventArgs());
			}
		}
		#endregion

		#region Private properties
		BackgroundWorker moodWorker;

		RgbColor TargetColor;

		int TargetPulses;
		
		RgbColor VisibleColor = RgbColor.FromRgb(0, 0, 0);
		
        private AnimationTypeEnum AnimationType = AnimationTypeEnum.CycleRandomColors;

        private FadeModeEnum FadeMode;

		private double AnimationSpeed = 0.01;

		public Boolean Animating = false;
		#endregion

		public DeviceEntity DataEntity;

		public List<CustomNotification> ActiveNotifications = new List<CustomNotification>();

		public int ActiveNotificationIndex = -1;

		public DateTime? LastNotificationTimestamp;

		public Boolean Checked = false;

		#region Private enums
        enum AnimationTypeEnum
        {
            CycleRandomColors,
            PulsateRandomColors,
			PulsateSingleColor,
			MorphToColor,
			BlinkColor
        }

        enum FadeModeEnum
        {
            FadeIn,
            FadeOut
        }
		#endregion

		#region Public Properties
		public int ColorDisplayTime = 3;

		public String DeviceVisibleName {
			get {
				return Device.Serial;
			}
		}

		#endregion
		
		#region Constructor
		public LedController ()
		{
			//AnimationType = AnimationTypeEnum.PulsateRandomColors;
		}
		#endregion

		#region Device Helpers
		public void SendColor(byte r, byte g, byte b)
		{
			if (Device.Connected)
			{
				if (DataEntity.Control == DeviceControlEnum.Normal)
				{
					Device.SetLedColor(r, g, b);
				}
				else
				{
					Device.SetLedColor((byte)(255 - r), (byte)(255 - g), (byte)(255 - b));
				}
			}
		}

        public void SendColor(RgbColor newColor)
        {
			if (Device.Connected)
			{
                try
                {
                    if (DataEntity.Control == DeviceControlEnum.Normal)
    				{
    					Device.SetLedColor(newColor.R, newColor.G, newColor.B);
    				}
    				else
    				{
    					Device.SetLedColor((byte)(255 - newColor.R), (byte)(255 - newColor.G), (byte)(255 - newColor.B));
    				}
                }
                catch {
                }
			}

			VisibleColor = newColor;
        }
		
		public void SendColor(Gdk.Color newColor)
        {
			SendColor (RgbColor.FromGdkColor(newColor.Red, newColor.Green, newColor.Blue));
        }
		#endregion

		#region Animation Functions
		public Boolean CanExecuteEvent ()
		{
			return ActiveNotifications.Count > 0;
		}

		public Boolean NextNotification ()
		{
			if (CanExecuteEvent()) {
				ActiveNotificationIndex += 1;

				if (ActiveNotificationIndex > ActiveNotifications.Count - 1)
					ActiveNotificationIndex = 0;
				return true;
			} else {
				return false;
			}
		}

		public void ExecuteEvent ()
		{
			ExecuteEvent(ActiveNotifications[ActiveNotificationIndex]);
		}

 		public void ExecuteEvent (CustomNotification e)
		{
			if (Animating)
            {
                return;
            }

            if (e.NotificationType == NotificationTypeEnum.Pulse) {
				switch (e.BlinkSpeed) {
				case BlinkSpeedEnum.VerySlow:
					AnimationSpeed = 0.001;
					break;
				case BlinkSpeedEnum.Slow:
					AnimationSpeed = 0.005;
					break;
				case BlinkSpeedEnum.Normal:
					AnimationSpeed = 0.01;
					break;
				case BlinkSpeedEnum.Fast:
					AnimationSpeed = 0.05;
					break;
				case BlinkSpeedEnum.VeryFast:
					AnimationSpeed = 0.1;
					break;
				case BlinkSpeedEnum.VeryVeryFast:
					AnimationSpeed = 0.15;
					break;
				default:
					break;
				}

				PulsateSingleColor (e.BlinkCount, e.VisibleColor);
            } else if (e.NotificationType == NotificationTypeEnum.Blink) {
				switch (e.BlinkSpeed) {
				case BlinkSpeedEnum.VerySlow:
					AnimationSpeed = 5000;
					break;
				case BlinkSpeedEnum.Slow:
					AnimationSpeed = 2000;
					break;
				case BlinkSpeedEnum.Normal:
					AnimationSpeed = 500;
					break;
				case BlinkSpeedEnum.Fast:
					AnimationSpeed = 200;
					break;
				case BlinkSpeedEnum.VeryFast:
					AnimationSpeed = 100;
					break;
				case BlinkSpeedEnum.VeryVeryFast:
					AnimationSpeed = 50;
					break;
				default:
					break;
				} 

				BlinkColor(e.BlinkCount, e.VisibleColor);
			} else {
				MorphToColor(e.VisibleColor);
			}
		}

		public void PulsateSingleColor(int numberOfTimes, RgbColor targetColor)
		{
			if (this.Animating)
				this.Stop();

			this.AnimationType = AnimationTypeEnum.PulsateSingleColor;
			this.TargetColor = targetColor;
			this.TargetPulses = numberOfTimes;
					
			this.Start();
		}

		public void MorphToColor(RgbColor targetColor)
		{
			if (this.AnimationType != AnimationTypeEnum.MorphToColor && this.Animating)
				this.Stop();

			this.AnimationType = AnimationTypeEnum.MorphToColor;
			this.TargetColor = targetColor;

			this.Start();
		}

		public void BlinkColor(int numberOfTimes, RgbColor targetColor)
		{
			if (this.Animating)
				this.Stop();

			this.AnimationType = AnimationTypeEnum.BlinkColor;
			this.TargetColor = targetColor;
			this.TargetPulses = numberOfTimes;
					
			this.Start();
		}
		#endregion

		#region Animation start/stop
		public void Start()
		{
            if (moodWorker == null)
            {
                moodWorker = new BackgroundWorker();
                moodWorker.DoWork += new DoWorkEventHandler(moodWorker_DoWork);
                moodWorker.WorkerSupportsCancellation = true;
                moodWorker.RunWorkerCompleted += (object sender, RunWorkerCompletedEventArgs e) => {
                    moodWorker = null;
                };
                moodWorker.RunWorkerAsync();
            }
		}
		
		public void Stop()
		{
			if (moodWorker != null)
			{
				moodWorker.CancelAsync();
			}
		}
		#endregion

		#region Animation Thread Functions
        void moodWorker_DoWork(object sender, DoWorkEventArgs e)
        {
			Animating = true;
			OnBlinkStart();

            switch (AnimationType)
            {
            case AnimationTypeEnum.CycleRandomColors:
				AnimateCycleRandomColors(sender);
                break;
            case AnimationTypeEnum.PulsateRandomColors:
				AnimatePulsateRandomColors(sender);
                break;
			case AnimationTypeEnum.PulsateSingleColor:
				AnimatePulsateSingleColor(sender);
				break;
			case AnimationTypeEnum.MorphToColor:
				AnimateMorphToColor(sender);
				break;
			case AnimationTypeEnum.BlinkColor:
				AnimateBlinkColor(sender);
				break;
            default:
                break;
            }

			OnBlinkEnd();
			Animating = false;
        }

		void AnimateBlinkColor (object sender)
		{
			int timesBlinked = 0;
			SendColor(0, 0, 0);

			while (!((BackgroundWorker)sender).CancellationPending) {
				if (TargetPulses != 0 && timesBlinked >= TargetPulses)
					break;

				SendColor(TargetColor);

				WaitMiliseconds((int)AnimationSpeed);
				if (((BackgroundWorker)sender).CancellationPending) 
					break;

				SendColor(0, 0, 0);

				WaitMiliseconds((int)AnimationSpeed);

				timesBlinked++;
			}

			SendColor(0, 0, 0);
		}

		void AnimatePulsateRandomColors (object sender)
		{
            Random r = new Random();

            HSLColor targetColor = new HSLColor(RgbColor.FromRgb(r.Next(256), r.Next(256), r.Next(256)));
			HSLColor currentColor = new HSLColor(RgbColor.FromRgb(0, 0, 0));
            FadeMode = FadeModeEnum.FadeOut;

			while (!((BackgroundWorker)sender).CancellationPending) {
                if (FadeMode == FadeModeEnum.FadeIn)
                {
                    if (currentColor.Lightness >= 0.5 || currentColor.Lightness >= targetColor.Lightness)
                    {
                        //fade out
                        FadeMode = FadeModeEnum.FadeOut; 

                        WaitSeconds(ColorDisplayTime);
                    }
                    else
                    {
                        currentColor.Lightness += 0.01;
                        Thread.Sleep(30);

                        SendColor(currentColor.Color);
                    }
                }
                else
                {
                    if (currentColor.Lightness <= 0)
                    {
                        targetColor = new HSLColor(RgbColor.FromRgb(r.Next(256), r.Next(256), r.Next(256)));

                        currentColor = targetColor;
                        //drop down lightness to see nicer and more rich colors
                        currentColor.Lightness = 0.0;

                        FadeMode = FadeModeEnum.FadeIn;
                    }
                    else
                    {
                        currentColor.Lightness -= 0.01;
                        Thread.Sleep(30);

                        SendColor(currentColor.Color);
                    }
                }
			}
		}

		void AnimateCycleRandomColors (object sender)
		{
            Random r = new Random();

            HSLColor targetColor = new HSLColor(RgbColor.FromRgb(r.Next(256), r.Next(256), r.Next(256)));
			targetColor.Lightness = 0.5;

			RgbColor currentColor = RgbColor.FromRgb(0, 0, 0);
            FadeMode = FadeModeEnum.FadeOut;

			while (!((BackgroundWorker)sender).CancellationPending) {
                if (targetColor.Color.R == currentColor.R && 
                    targetColor.Color.G == currentColor.G && 
                    targetColor.Color.B == currentColor.B)
                {
                    WaitSeconds(ColorDisplayTime);

                    targetColor = new HSLColor(RgbColor.FromRgb(r.Next(256), r.Next(256), r.Next(256)));

                    //drop down lightness to see nicer and more rich colors
                    targetColor.Lightness = 0.5;
                }
                else
                {
                    currentColor.R += (byte)Math.Sign(targetColor.Color.R - currentColor.R); 
                    currentColor.G += (byte)Math.Sign(targetColor.Color.G - currentColor.G); 
                    currentColor.B += (byte)Math.Sign(targetColor.Color.B - currentColor.B);

                    SendColor(currentColor);

                    Thread.Sleep(10);
                }
			}
		}
		
		void AnimatePulsateSingleColor (object sender)
		{
			HSLColor currentColor = new HSLColor(TargetColor);
			double targetLightness = currentColor.Lightness;
			currentColor.Lightness = 0;
			
			FadeMode = FadeModeEnum.FadeIn;
			
			int pulseCount = 0;
				
			while (!((BackgroundWorker)sender).CancellationPending) {
				if (FadeMode == FadeModeEnum.FadeIn)
                {
                    if (currentColor.Lightness >= targetLightness)
                    {
                        //fade out
                        FadeMode = FadeModeEnum.FadeOut; 
                    }
                    else
                    {
                        currentColor.Lightness += AnimationSpeed;
                        Thread.Sleep(5);

                        SendColor(currentColor.Color);
                    }
                }
                else
                {
                    if (currentColor.Lightness <= 0)
                    {
						FadeMode = FadeModeEnum.FadeIn;
                        
						pulseCount++;
						
						if (pulseCount >= TargetPulses)
						{
							break;
						}
                    }
                    else
                    {
                        currentColor.Lightness -= AnimationSpeed;
                        Thread.Sleep(5);

                        SendColor(currentColor.Color);
                    }
                }
			}
		}
		
		void AnimateMorphToColor(object sender)
		{
            while (!((BackgroundWorker)sender).CancellationPending) {
                
				if (TargetColor.R == VisibleColor.R && 
                    TargetColor.G == VisibleColor.G && 
                    TargetColor.B == VisibleColor.B)
                {
					break;
				}
                else
                {
                    RgbColor newColor = new RgbColor();
					newColor.R = (byte)(VisibleColor.R + Math.Sign(TargetColor.R - VisibleColor.R)); 
                    newColor.G = (byte)(VisibleColor.G + Math.Sign(TargetColor.G - VisibleColor.G)); 
                    newColor.B = (byte)(VisibleColor.B + Math.Sign(TargetColor.B - VisibleColor.B));

                    SendColor(newColor);

                    Thread.Sleep(5);
                }
			}			
		}
		#endregion

		/*
        void moodWorker_DoWork(object sender, DoWorkEventArgs e)
        {

            switch (AnimationType)
            {
                case AnimationTypeEnum.CycleRandomColors:
					AnimateCycleRandomColors(sender);
                    break;
                case AnimationTypeEnum.PulsateRandomColors:
					AnimatePulsateRandomColors(sender);
                    break;
                default:
                    break;
            }
        }

		void AnimateCycleRandomColors(object sender)
		{
            Random r = new Random();

            HSLColor targetColor = new HSLColor(RgbColor.FromRgb((byte)r.Next(256), (byte)r.Next(256), (byte)r.Next(256)));
            //drop down lightness to see nicer and more rich colors
            targetColor.Lightness = 0.5;

			byte tred = (byte)(targetColor.Color.Red * 1.0 / ushort.MaxValue * 255);
			byte tgreen = (byte)(targetColor.Color.Green * 1.0 / ushort.MaxValue * 255);
			byte tblue = (byte)(targetColor.Color.Blue * 1.0 / ushort.MaxValue * 255);

			byte red = 0;
			byte green = 0;
			byte blue = 0;

            while (!((BackgroundWorker)sender).CancellationPending)
            {
                if (tred == red && tgreen == green && tblue == blue)
                {
                    Wait(ColorDisplayTime);

                    targetColor = new HSLColor(new Color((byte)r.Next(256), (byte)r.Next(256), (byte)r.Next(256)));

                    //drop down lightness to see nicer and more rich colors
                    targetColor.Lightness = 0.5;

					tred = (byte)(targetColor.Color.Red * 1.0 / ushort.MaxValue * 255);
					tgreen = (byte)(targetColor.Color.Green * 1.0 / ushort.MaxValue * 255);
					tblue = (byte)(targetColor.Color.Blue * 1.0 / ushort.MaxValue * 255);
                }
                else
                {
					red = (byte)(red + Math.Sign(tred - red));
					green = (byte)(green + Math.Sign(tgreen - green));
					blue = (byte)(blue + Math.Sign(tblue - blue));

                    SendColor(red, green, blue);

                    Thread.Sleep(10);
                }
			}
		}

		void AnimatePulsateRandomColors (object sender)
		{
            Random r = new Random();

            HSLColor targetColor = new HSLColor(new Color((byte)r.Next(256), (byte)r.Next(256), (byte)r.Next(256)));
			HSLColor currentColor = new HSLColor(new Color(255, 0, 0));
            FadeMode = FadeModeEnum.FadeOut;

			while (!((BackgroundWorker)sender).CancellationPending) {
				if (FadeMode == FadeModeEnum.FadeIn) {
					if (currentColor.Lightness >= 0.5 || currentColor.Lightness >= targetColor.Lightness) {
						//fade out
						FadeMode = FadeModeEnum.FadeOut; 

						Wait (ColorDisplayTime);
					} else {
						currentColor.Lightness += 0.01;
						Thread.Sleep (30);

						SendColor (currentColor.Color);
					}
				} else {
					if (currentColor.Lightness <= 0) {
						targetColor = new HSLColor (new Color ((byte)r.Next (256), (byte)r.Next (256), (byte)r.Next (256)));

						currentColor = targetColor;
						//drop down lightness to see nicer and more rich colors
						currentColor.Lightness = 0.0;

						FadeMode = FadeModeEnum.FadeIn;
					} else {
						currentColor.Lightness -= 0.01;
						Thread.Sleep (30);

						SendColor (currentColor.Color);
					}
				}
			}
		}
*/

		#region Animation Thread Helpers
        void WaitSeconds(int seconds)
        {
            DateTime start = DateTime.Now;
            while (moodWorker != null && !moodWorker.CancellationPending && 
			       start.AddSeconds(seconds) > DateTime.Now)
            {
                Thread.Sleep(10);
            }
        }

        void WaitMiliseconds(int miliseconds)
        {
            DateTime start = DateTime.Now;
            while (moodWorker != null && !moodWorker.CancellationPending && 
			       start.AddMilliseconds(miliseconds) > DateTime.Now)
            {
                Thread.Sleep(10);
            }
        }
		#endregion

        public static LedController[] AllControllers ()
		{
			List<LedController> result = new List<LedController>();

            foreach (BlinkstickHid hidDevice in BlinkstickHid.AllDevices()) {
				LedController controller = new LedController();
				controller.Device = hidDevice;
				controller.Device.OpenDevice();
				result.Add (controller);
			}

			return result.ToArray();
        }

        public void Close(){
            if (Device != null)
                Device.CloseDevice();
        }
	}
}
