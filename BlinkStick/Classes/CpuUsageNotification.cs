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
using System.Diagnostics;
#if !__MonoCS__
using System.Management;
#endif
using log4net;
using BlinkStickDotNet;

namespace BlinkStickClient.Classes
{
	public class CpuUsageNotification : CustomNotification
	{
		PerformanceCounter cpuCounter;
		float cpuUsage;

		public CpuUsageNotification ()
		{
			cpuCounter = new PerformanceCounter ();
			cpuCounter.CategoryName = "Processor";
			cpuCounter.CounterName = "% Processor Time";
			cpuCounter.InstanceName = "_Total";

			log = LogManager.GetLogger("CPU Usage");
		}

		public override bool Check ()
		{
			cpuUsage = cpuCounter.NextValue ();
			log.DebugFormat("CPU usage: {0}", cpuUsage);
			return true;
		}

		public override string GetTypeName ()
		{
			return "CPU Usage";
		}

		public override int GetCheckFrequency ()
		{
			return 2;
		}

		public override RgbColor GetVisibleColor ()
		{
			/*
			HSLColor color = new HSLColor (Color);
			color.Lightness = (double)(color.Lightness * cpuUsage / 100);
			return color.Color;
			*/
			RgbColor color = new RgbColor ();
			color.R = (byte)(255 * cpuUsage / 100);
			color.G = (byte)(255 - 255 * cpuUsage / 100);

			return color;
		}
	}
}

