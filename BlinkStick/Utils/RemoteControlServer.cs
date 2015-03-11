using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.ComponentModel;
using System.IO;
using System.Web;
using log4net;
using Newtonsoft.Json;

namespace BlinkStickClient.Utils
{
    public class RemoteControlServer
    {
        #region Events
        public event EventHandler<RequestReceivedEventArgs> RequestReceived;

        protected Boolean OnRequestReceived(HttpListenerContext context)
        {
            if (RequestReceived != null)
            {
                RequestReceivedEventArgs args = new RequestReceivedEventArgs(context);
                RequestReceived(this, args);

                return args.Handled;
            }

            return false;
        }
        #endregion

        ILog log = LogManager.GetLogger("RemoteControlServer");

        protected const String HEADER_RANGE = "range";
	    protected const String HEADER_CONTENT_TYPE = "Content-Type";
	    protected const String HEADER_CONTENT_LENGTH = "Content-Length: ";
	    protected const String HEADER_CONTENT_DISPOSITION = "Content-Disposition";
	    protected const String HEADER_CONTENT_RANGE = "Content-Range";
	    protected const String HEADER_BOUNDARY_DELIMETER = "--";
	    protected const String HEADER_STATUS_PARTIAL_CONTENT = "Partial Content";
	
	    private const char COMMA = ',';
	    private const char EQUALS = '=';
	    private const char NEW_LINE = '\n';
	
	    protected const String QS_OBJECT_ID = "cid";
	 
	    HttpListener listener;

        BackgroundWorker worker = new BackgroundWorker();

        public String ApiBindAddress { get; private set; }
        public int ApiAccessPort { get; private set; }

        #region Constructor
        public RemoteControlServer(String apiBindAddress, int apiAccessPort)
        {
            this.ApiBindAddress = apiBindAddress;
            this.ApiAccessPort = apiAccessPort;
        } 
        #endregion

        #region Responses
        public void SendResponse(int statusCode, String status, String statusMessage, HttpListenerResponse response)
	    {
	    	log.DebugFormat("Response: [{0}] {1} - {2}", statusCode, status, statusMessage);
	    	
	        System.Text.StringBuilder data = new System.Text.StringBuilder();
	
	        data.AppendLine("<html><body>");
	        data.AppendLine("<h1>"+status+"</h1>");
	        data.AppendLine("<p>"+statusMessage+"</p>");
	        data.AppendLine("</body></html>");
	
	        byte[] headerData = System.Text.Encoding.UTF8.GetBytes(data.ToString());
	
	        response.StatusCode = statusCode;
	        response.ContentType = "text/html";
		    response.ContentLength64 = headerData.Length;
	        
	        response.OutputStream.Write(headerData, 0, headerData.Length);
	        response.OutputStream.Close();
	    }

        public void SendResponseJson(int responseCode, object responseObject, HttpListenerResponse response)
	    {
            byte[] outputData;

            if (responseObject == null)
            {
                outputData = new byte[0];
            }
            else
            {
                outputData = System.Text.Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(responseObject));
            }

	        response.StatusCode = 200;
	        response.ContentType = "text/json";
            response.ContentLength64 = outputData.Length;
	
            response.OutputStream.Write(outputData, 0, outputData.Length);
	        response.OutputStream.Close();
	    }
        #endregion
	
        #region Start/Stop
        public void Start()
        {
            if (worker.IsBusy)
                return;

            worker.WorkerSupportsCancellation = true;
            worker.WorkerReportsProgress = false;
            worker.DoWork += DoWork;
            worker.RunWorkerAsync();
        }

		public void Stop ()
		{
            worker.CancelAsync();

            if (listener != null && listener.IsListening)
			{
				listener.Abort();
			}
		}
        #endregion

        #region Main Thread Function
        void DoWork (object sender, DoWorkEventArgs e)
        {
            if (!HttpListener.IsSupported)
            {
                log.ErrorFormat ("Windows XP SP2 or Server 2003 is required to use Remote Control.");
                return;
            }

            // Create a listener.
            listener = new HttpListener();
            // Add the prefixes.
            listener.Prefixes.Add(String.Format("http://{0}:{1}/", ApiBindAddress, ApiAccessPort));
            try
            {
                listener.Start();
            }
            catch (Exception ex)
            {
                log.ErrorFormat("Unable to bind http://{0}:{1}/ for remote control server: {2}", ApiBindAddress, ApiAccessPort, ex.Message);
            }

            HttpListenerContext context = null;

            while (!worker.CancellationPending && listener.IsListening)
            {
                try
                {
                    // Note: The GetContext method blocks while waiting for a request. 
                    context = listener.GetContext();
                }
                catch
                {
                    if (!listener.IsListening || worker.CancellationPending)
                        break;
                }
                HttpListenerRequest request = context.Request;
                // Obtain a response object.
                HttpListenerResponse response = context.Response;

                log.DebugFormat("Remote control request: {0}", request.RawUrl);

                if (!OnRequestReceived(context))
                {
                    SendResponse(404, "404 Not Found", "The resource requested does not exist.", context.Response);
                }
            }

            log.Info("Stopped");
        }
        #endregion
    } 

    public class RequestReceivedEventArgs : EventArgs
    {
        public Boolean Handled;
        public HttpListenerContext Context;

        public RequestReceivedEventArgs(HttpListenerContext context)
        {
            this.Context = context;
        }
    }
}
