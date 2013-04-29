#region License
// Copyright 2013 by Agile Innovative Ltd
//
// This file is part of BlinkStick.CometClient test application.
//
// BlinkStick.CometClient test application is free software: you can redistribute 
// it and/or modify it under the terms of the GNU General Public License as published 
// by the Free Software Foundation, either version 3 of the License, or (at your option) 
// any later version.
//		
// BlinkStick.CometClient test application is distributed in the hope that it will be useful, but 
// WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or
// FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License along with 
// BlinkStick.CometClient test application. If not, see http://www.gnu.org/licenses/.
#endregion

using System;
using System.Net;
using System.Web;
using System.Text;
using System.IO;
using System.Data.Linq;
using System.Collections.Generic;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Schema;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Bson;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Utilities;
using Newtonsoft.Json;
using BlinkStick.Bayeux;
using BlinkStick.Bayeux.Classes;
using log4net;

namespace BlinkStick.CometClient
{
	class MainClass
	{
		protected static readonly ILog log = LogManager.GetLogger("Main");	

		public static Uri ServerEndpoint;

		public static void Main (string[] args)
		{
			log4net.Config.XmlConfigurator.Configure();

			Console.WriteLine ("1 - subscribe to channel");
			Console.WriteLine ("q - unsubscribe from channel");
			Console.WriteLine ("2 - subscribe to channel");
			Console.WriteLine ("w - unsubscribe from channel");
			Console.WriteLine ("x - exit");
			Console.WriteLine ("");
			Console.WriteLine ("");

			ServerEndpoint = new Uri (args [0]);

			BayeuxClient client = new BayeuxClient (ServerEndpoint);

			client.DataReceived += (object sender, BayeuxClient.DataReceivedEventArgs e) => {
				if (e.Data.ContainsKey("color"))
				{
					Console.WriteLine ("Received color : " + e.Data ["color"]);
				}
				else if (e.Data.ContainsKey("status"))
				{
					Console.WriteLine ("Received status : " + e.Data ["status"]);
				}
				else
				{
					Console.WriteLine("Received unknown message");
				}
			};

			client.Subscribe ("/devices/51251481d988b43c45000002");
			client.Connect ();

			ConsoleKeyInfo key = Console.ReadKey ();

			while (key.KeyChar != 'x') {
				if (key.KeyChar == '1') {
					client.Subscribe ("/devices/51251481d988b43c45000002");
				}

				if (key.KeyChar == 'q') {
					client.Unsubscribe ("/devices/51251481d988b43c45000002");
				}

				if (key.KeyChar == '2') {
					client.Subscribe ("/devices/512518fcd988b43c45000003");
				}

				if (key.KeyChar == 'w') {
					client.Unsubscribe ("/devices/512518fcd988b43c45000003");
				}

				if (key.KeyChar == '*') {
					client.Subscribe ("/dees/*");
				}

				if (key.KeyChar == 'i') {
					client.Unsubscribe ("/devices/*");
				}

				key = Console.ReadKey ();
			}

			client.Disconnect ();
			Console.WriteLine ("Waiting to disconnect");
			client.WaitFor (BayeuxClient.ClientStateEnum.Disconnected);
			Console.WriteLine ("Press any key to exit...");
			Console.ReadKey();
		}
	}
}
