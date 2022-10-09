using LazyHttpServerLib;
using MimeMapping;

HttpServer httpServer = new(new string[] { "http://127.0.0.1:8080/" });

string wwwrootdir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "wwwroot");

httpServer.IncomingHttpRequest += (sender, e) => {
    Console.WriteLine("=> {0} {1}", e.Request.HttpMethod, e.Request.RawUrl);

    if (e == null || e.Request == null || e.Request.RawUrl == null)
        return;

    string req_file = e.Request.RawUrl;
    if (req_file.EndsWith("/"))
        req_file += "index.html";
    req_file = req_file.TrimStart('/');

    string file = Path.Combine(wwwrootdir, req_file);

    int code = File.Exists(file) ? 200 : 404;

    e.Response.StatusCode = code;

    if (File.Exists(file))
    {
        FileInfo fi = new(file);

        e.Response.ContentLength64 = fi.Length;
        e.Response.ContentType = MimeUtility.GetMimeMapping(file);

        using (var fs = File.OpenRead(file))
        {
            fs.CopyTo(e.Response.OutputStream);
        }
    }

    Console.WriteLine("<= {0}: {1}", code, file);

    e.Response.Close();
};

httpServer.Start();

while (true)
{
    if (Console.ReadLine() == "exit")
    {
        httpServer.Close();
        break;
    }
}
