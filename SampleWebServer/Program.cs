using LazyHttpServerLib;
using MimeMapping;
using System.Reflection;

Console.WriteLine(@"
LazyHttpServerLib Sample Web Server Application

Copyright (c) 2022 mkaraki.

THIRD PARTY SOFTWARE NOTICES AND INFORMATION:

MimeMapping

```
MIT License

Copyright (c) 2016 Matthew Little

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED ""AS IS"", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
```


Server will listen on http://127.0.0.1:8080/
");

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

Console.WriteLine("Listen started.");

while (true)
{
    if (Console.ReadLine() == "exit")
    {
        httpServer.Close();
        break;
    }
}
