using System;
using System.Diagnostics;
using System.Net;
using System.Text;
using System.Threading;
using FubuCore;

namespace FubuMVC.Core.Endpoints
{
    public static class WebRequestResponseExtensions
    {
        public static HttpResponse ToHttpCall(this WebRequest request)
        {
            var reset = new ManualResetEvent(false);
            IAsyncResult result = null;


            var thread = new Thread(() =>
            {
                result = request.BeginGetResponse(r => { }, null);
                reset.Set();
            });
            thread.Start();
            thread.Join();
            reset.WaitOne();

            try
            {
                var response = request.EndGetResponse(result).As<HttpWebResponse>();
                return new HttpResponse(response);
            }
            catch (WebException e)
            {
                if (e.Response == null)
                {
                    throw;
                }

                var errorResponse = new HttpResponse(e.Response.As<HttpWebResponse>());

                return errorResponse;
            }
        }

        public static void WriteText(this WebRequest request, string content)
        {
            request.ContentLength = content.Length;
            var stream = request.GetRequestStream();

            var array = Encoding.Default.GetBytes(content);
            stream.Write(array, 0, array.Length);
            stream.Close();
        }
    }
}