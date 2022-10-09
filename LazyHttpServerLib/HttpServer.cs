using System.Net;

namespace LazyHttpServerLib
{
    public class HttpServer
    {
        private HttpListener listener;

        public HttpServer(IEnumerable<string> prefixes)
        {
            if (!HttpListener.IsSupported)
                throw new NotSupportedException("This platform doesn't support HttpListener class.");

            listener = new();
         
            foreach(string prefix in prefixes)
                listener.Prefixes.Add(prefix);
        }

        public bool IsListening { get => listener.IsListening; }

        public void Start()
        {
            listener.Start();
            StartWait();
        }

        /// <summary>
        /// Call HttpListener.Close()
        /// See: https://learn.microsoft.com/ja-jp/dotnet/api/system.net.httplistener.close
        /// </summary>
        public void Close()
        {
            listener.Close();
        }

        /// <summary>
        /// Call HttpListener.Abort()
        /// See: https://learn.microsoft.com/ja-jp/dotnet/api/system.net.httplistener.abort
        /// </summary>
        public void Abort()
        {
            listener.Abort();
        }

        /// <summary>
        /// Call HttpListener.Stop()
        /// See: https://learn.microsoft.com/ja-jp/dotnet/api/system.net.httplistener.stop
        /// </summary>
        public void Stop()
        {
            listener.Stop();
        }

        public void Pause()
            => Stop();

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