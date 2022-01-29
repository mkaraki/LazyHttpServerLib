using System.Net;

namespace LazyHttpServerLib
{
    public class HttpServer
    {
        private HttpListener listener;

        public HttpServer(IEnumerable<string> prefixes)
        {
            listener = new();
         
            foreach(string prefix in prefixes)
                listener.Prefixes.Add(prefix);
        }

        public void Start()
        {
            listener.Start();
            StartWait();
        }

        public void Stop()
        {
            listener.Close();
        }

        public event EventHandler<IncomingHttpRequestEventArgs>? IncomingHttpRequest;

        private void StartWait()
        {
            listener.BeginGetContext(new AsyncCallback(ListenerCallback), listener);
        }

        private void ListenerCallback(IAsyncResult result)
        {
            HttpListener? listener = result.AsyncState as HttpListener;
            if (listener == null) return;
            HttpListenerContext context = listener.EndGetContext(result);
            StartWait();

            if (IncomingHttpRequest != null)
                IncomingHttpRequest(this, new(context.Request, context.Response));
        }
    }

    public class IncomingHttpRequestEventArgs : EventArgs
    {
        public IncomingHttpRequestEventArgs(HttpListenerRequest request, HttpListenerResponse response)
        {
            Request = request;
            Response = response;
        }

        public HttpListenerRequest Request { get; set; }

        public HttpListenerResponse Response { get; set; }
    }
}