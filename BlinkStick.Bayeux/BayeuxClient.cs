#region License
// Copyright 2013 by Agile Innovative Ltd
//
// This file is part of BlinkStick.Bayeux library.
//
// BlinkStick.Bayeux library is free software: you can redistribute it and/or modify 
// it under the terms of the GNU General Public License as published by the Free 
// Software Foundation, either version 3 of the License, or (at your option) any 
// later version.
//		
// BlinkStick.Bayeux library is distributed in the hope that it will be useful, but 
// WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or
// FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License along with 
// BlinkStick.Bayeux library. If not, see http://www.gnu.org/licenses/.
#endregion

using System;
using System.ComponentModel;
using System.Threading;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Collections.Generic;
using BlinkStick.Bayeux.Classes;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using log4net;

namespace BlinkStick.Bayeux
{
	public class BayeuxClient
	{
		#region Events
        // -------------- DataReceived ---------------
	    public class DataReceivedEventArgs
	    {
			public Dictionary<string, string> Data;
			public String Channel;

	        public DataReceivedEventArgs(Dictionary<string, string> data, String channel)
	        {
				this.Data = data;
				this.Channel = channel;
	        }
	    }

        public delegate void DataReceivedEventHandler(object sender, DataReceivedEventArgs e);

        public event DataReceivedEventHandler DataReceived;

        protected void OnDataReceived(Dictionary<string, string> data, String channel)
        {
            if (DataReceived != null)
            {
                DataReceived(this, new DataReceivedEventArgs(data, channel));
            }
        }

        // -------------- ChannelUnsubscribed ---------------
		public event EventHandler ChannelUnsubscribed;

		protected void OnChannelUnsubscribed()
		{
			if (ChannelUnsubscribed != null)
			{
				ChannelUnsubscribed(this, new EventArgs());
			}
		}

        // -------------- Disconnected ---------------
		public event EventHandler Disconnected;

		protected void OnDisconnected()
		{
			if (Disconnected != null)
			{
				Disconnected(this, new EventArgs());
			}
		}
		#endregion

		#region Public Properties
		public Uri ServerUri {
			get;
			set;
		}

		public ClientStateEnum _ClientState; 
		public ClientStateEnum ClientState {
			get {
				return _ClientState;
			}
			private set {
				if (_ClientState != value)
				{
					_ClientState = value;
					log.DebugFormat("ClientState changed to {0}", _ClientState.ToString());
				}
			}
		}

		public Boolean Connected {
			get;
			private set;
		}

		public Boolean Working {
			get;
			private set;
		}

		public Boolean HasSubscribedChannels {
			get {
				return (ChannelSubscriptions.Count + PendingChannelSubscriptions.Count) > 0;
			}
		}
		#endregion

		#region Private Properties
		protected ILog log = LogManager.GetLogger("Main");	
		private BackgroundWorker BayeuxWorker;
		private Advice ServerAdvice;
		private Boolean LongPolling = false;
		private String ClientId;
		private List<String> PendingChannelSubscriptions = new List<String>();
		private List<String> PendingChannelUnsubscriptions = new List<String>();
		private List<String> ChannelSubscriptions = new List<String>();
		private TcpClient client;
		private const int DelayBetweenFailedRequests = 5000;
		#endregion

		#region Enums
		public enum ClientStateEnum
		{
			Disconnected,
			Handshaking,
			Connected,
			Disconnecting
		}
		#endregion

		#region Constructor
		public BayeuxClient (Uri serverUri)
		{
			this.ServerUri = serverUri;

			//Setup default server advice
			ServerAdvice = new Advice();
			ServerAdvice.interval = 0;
			ServerAdvice.reconnect = "retry";
			ServerAdvice.timeout = 30000;

			//Setup background worker
			BayeuxWorker = new BackgroundWorker();
			BayeuxWorker.WorkerSupportsCancellation = true;
			BayeuxWorker.DoWork += ProcessRequests;

			ClientState = ClientStateEnum.Disconnected;
			log = LogManager.GetLogger(serverUri.Host);	
		}

		public BayeuxClient (String serverAddress) : this (new Uri(serverAddress))
		{
		}
		#endregion

		#region Main Thread
		void ProcessRequests (object sender, DoWorkEventArgs e)
		{
			Working = true;

			log.Info("Bayeux client started.");
			BackgroundWorker worker = (BackgroundWorker)sender;

			while (!worker.CancellationPending) {
				try
				{
					switch (ClientState) {
					case ClientStateEnum.Connected:
						if (PendingChannelSubscriptions.Count > 0)
						{
							ProcessSubscribeToChannel(PendingChannelSubscriptions[0]);
						}
						else if (PendingChannelUnsubscriptions.Count > 0)
						{
							ProcessUnsubscribeFromChannel(PendingChannelUnsubscriptions[0]);
						}
						else
						{
							log.Info("Going into connect loop.");
							while (ProcessConnect() && !worker.CancellationPending
							       && ClientState == ClientStateEnum.Connected)
							{
								WaitMiliseconds(worker, ServerAdvice.interval);
							}
						}
						break;
					case ClientStateEnum.Disconnected:
						Thread.Sleep(100);		
						break;
					case ClientStateEnum.Handshaking:
						if (ProcessHandshake())
						{
							lock(this)
							{
								ClientState = ClientStateEnum.Connected;
							}
						}
						break;
					case ClientStateEnum.Disconnecting:

						try
						{
							if (ProcessDisconnect())
							{
								lock(this)
								{
									ClientState = ClientStateEnum.Disconnected;
								}
							}
						}
						catch (Exception ex)
						{
							log.InfoFormat("Failed to disconnect: {0}", ex.Message);
						}

						OnDisconnected();	
						break;
					default:
					break;
					}
				}
				catch (Exception ex)
				{
					log.Error(ex.Message + "\r\n" + ex.StackTrace);

					if (ClientState == ClientStateEnum.Handshaking)
					{
						WaitMiliseconds(worker, DelayBetweenFailedRequests);
					}
				}
			}

			log.Info("Bayeux client stopped.");

			Working = false;
		}

		Boolean ProcessHandshake ()
		{
			log.Info("Processing handshake");

			JObject o = new JObject (
				new JProperty ("channel", "/meta/handshake"),
				new JProperty ("version", "1.0"),
				new JProperty ("supportedConnectionTypes", new JArray ("long-polling"))); 

			String response = SendMessage (o);

			List<HandshakeResponse> hResponses = JsonConvert.DeserializeObject<List<HandshakeResponse>> (response);
			log.Debug("Acquired CIENT-ID: " + hResponses[0].clientId);

			ClientId = hResponses [0].clientId;

			if (hResponses [0].advice != null && hResponses [0].advice.reconnect == "retry") {
				this.ServerAdvice = hResponses [0].advice;
			}

			ResubscribeToChannels();

			return true;
		}

		void ResubscribeToChannels()
		{
			//move all subscribed channels to pending state
			lock (PendingChannelSubscriptions) {
				PendingChannelSubscriptions.AddRange(ChannelSubscriptions);
			}

			ChannelSubscriptions.Clear();
		}

		Boolean ProcessConnect ()
		{
			JObject o = new JObject (
				new JProperty ("channel", "/meta/connect"),
				new JProperty ("clientId", ClientId),
				new JProperty ("connectionType", "long-polling")); 

			LongPolling = true;
			String output = "";
			try {
				output = SendMessage (o);
			} catch (Exception ex) {
				if (ex.Message.Contains ("blocking operation was interrupted by a call to WSACancelBlockingCall")) {
					return false;
				}
				else if (ex.Message.Contains ("No connection could be made because the target machine actively refused it")) {
					//Means something went wrong on the server and we need to rehandshake
					//also forcing it to go into delayed connection state
					ClientState = ClientStateEnum.Handshaking;
					return false;
				}
				else if (ex.Message.Contains ("A connection attempt failed because the connected party did not properly respond after a period of time, or established connection failed because connected host has failed to respond")) {
					//Means that the server or internet connectivity may be down 
					//also forcing it to go into delayed connection state
					ClientState = ClientStateEnum.Handshaking;
					return false;
				}
				else
				{
					throw ex;
				}
			} finally {
				LongPolling = false;
			}

			List<ServerResponse> responses = JsonConvert.DeserializeObject<List<ServerResponse>> (output);

			foreach (ServerResponse response in responses) {
				ProcessServerResponse(response);
			}

			return true;
		}


		Boolean ProcessDisconnect ()
		{
			log.Info("Processing disconnect");

			JObject o = new JObject (
				new JProperty ("channel", "/meta/disconnect"),
				new JProperty ("clientId", ClientId)); 

			String output = SendMessage (o);

			List<ServerResponse> responses = JsonConvert.DeserializeObject<List<ServerResponse>> (output);

			foreach (ServerResponse response in responses) {
				ProcessServerResponse(response);
			}

			return true;
		}

		void ProcessSubscribeToChannel (String channelName)
		{
			log.Info("Processing subscription to channel " + channelName);

			JObject o = new JObject(
				new JProperty("channel", "/meta/subscribe"),
				new JProperty("clientId", ClientId),
				new JProperty("subscription", channelName)); 

			String output = SendMessage (o);

			List<ServerResponse> responses = JsonConvert.DeserializeObject<List<ServerResponse>> (output);

			foreach (ServerResponse response in responses) {
				ProcessServerResponse(response);
			}
		}

		void ProcessUnsubscribeFromChannel (String channelName)
		{
			log.Info("Processing unsubscription from channel " + channelName);

			JObject o = new JObject(
				new JProperty("channel", "/meta/unsubscribe"),
				new JProperty("clientId", ClientId),
				new JProperty("subscription", channelName)); 

			String output = SendMessage (o);

			List<ServerResponse> responses = JsonConvert.DeserializeObject<List<ServerResponse>> (output);

			foreach (ServerResponse response in responses) {
				ProcessServerResponse(response);
			}
		}

		void ProcessServerResponse (ServerResponse response)
		{
			if (response.data != null) {
				log.Debug("Received data");
				OnDataReceived(response.data, response.channel);
			} else {
				if (response.channel == "/meta/subscribe")
				{
					if (response.successful)
					{
						log.Info ("Subscription successfull to " + response.subscription);
						ChannelSubscriptions.Add(response.subscription);
					}
					else
					{
						log.Error("Subscription failed to " + response.subscription);
					}
					//TODO: Add an event here for failed or successful channel subscription

					lock(PendingChannelSubscriptions)
					{
						PendingChannelSubscriptions.Remove(response.subscription);
					}
				}
				else if (response.channel == "/meta/unsubscribe")
				{
					if (response.successful)
					{
						log.Info( "Unsubscription successfull from " + response.subscription);
						ChannelSubscriptions.Remove(response.subscription);
						lock(PendingChannelUnsubscriptions)
						{
							PendingChannelUnsubscriptions.Remove(response.subscription);
						}
						OnChannelUnsubscribed();
					}
					else
					{
						log.Error("Subscription failed to " + response.subscription);
					}
				}
				else if (response.channel == "/meta/connect")
				{
					if (!response.successful && response.error != null && 
					    response.advice != null && response.advice.reconnect == "handshake")
					{
						log.Info ("Received request to rehandshake");

						lock(this)
						{
							ClientState = ClientStateEnum.Handshaking;
						}
					}
				}

				if (response.advice != null && response.advice.reconnect == "retry")
				{
					log.Debug("Server advice received " + response.advice.ToString());
					this.ServerAdvice = response.advice;
				}
			}
		}

		public String SendMessage (JObject o)
		{
			return SendData("message=" + o.ToString());
		}

		public String SendData(String postData)
		{
			client = new TcpClient(ServerUri.Host, ServerUri.Port);

			var head = new WebHeaderCollection();
			head[HttpRequestHeader.Host] = ServerUri.Host;
			head[HttpRequestHeader.Connection] = "keep-alive";
			head[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";

			var hhd = "POST " + ServerUri.PathAndQuery + " HTTP/1.0\r\n" + head + postData;

		    NetworkStream stream = client.GetStream();
		    byte[] send = Encoding.ASCII.GetBytes(hhd);
		    stream.Write(send, 0, send.Length);

		    byte[] bytes = new byte[client.ReceiveBufferSize];
		    int count = stream.Read(bytes, 0, (int)client.ReceiveBufferSize);
		    String data = Encoding.ASCII.GetString(bytes);
		    char[] unused = {(char)data[count]};

			String output = data.TrimEnd(unused);
		    stream.Close();
		    client.Close();

			String[] responseData = output.Split(new string[] { "\r\n" }, StringSplitOptions.None);

			if (!responseData[0].StartsWith("HTTP/1.1 200"))
			{
				throw new Exception("Bad response received :" + output);
			}

			output = output.Remove(0, output.IndexOf("\r\n\r\n") + 4);

			log.Debug("Response: " + output);

			return output;
		}
		#endregion

		#region Methods
		public Boolean Connect ()
		{
			if (Working)
				return false;

			lock (this) {
				ClientState = ClientStateEnum.Handshaking;
			}

			if (!BayeuxWorker.IsBusy) {
				BayeuxWorker.RunWorkerAsync();
			}
			return false;
		}

		public Boolean Disconnect()
		{
			lock(this) {
				ClientState = ClientStateEnum.Disconnecting;
			}	

			if (LongPolling) {
				client.Close();
			}

			return false;
		}

		public void CloseThread ()
		{
			BayeuxWorker.CancelAsync();

			if (LongPolling) {
				client.Close();
			}
		}

		public Boolean Subscribe (String channelName)
		{
			lock (PendingChannelUnsubscriptions) {
				PendingChannelUnsubscriptions.Remove (channelName);
			}

			lock (PendingChannelSubscriptions) {
				PendingChannelSubscriptions.Add (channelName);
			}

			if (LongPolling) {
				client.Close();
			}

			return false;
		}

		public Boolean Unsubscribe (String channelName)
		{
			lock (PendingChannelUnsubscriptions) {
				PendingChannelUnsubscriptions.Add (channelName);
			}

			lock (PendingChannelSubscriptions) {
				PendingChannelSubscriptions.Remove (channelName);
			}

			if (LongPolling) {
				client.Close();
			}

			return false;
		}

		public Boolean WaitFor (ClientStateEnum state)
		{
			while (ClientState != state) {
				Thread.Sleep(100);
			}

			return true;
		}
		#endregion

		#region Helper functions
        void WaitMiliseconds(BackgroundWorker worker, int miliseconds)
        {
            DateTime start = DateTime.Now;
            while (!worker.CancellationPending && 
			       start.AddMilliseconds(miliseconds) > DateTime.Now)
            {
                Thread.Sleep(10);
            }
        }
		#endregion
	}
}

