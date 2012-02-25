using System;
using System.IO;
using System.Net;
using System.Threading;
using System.Web;
using FubuCore;
using FubuCore.Util;
using FubuMVC.Core.Http;

namespace FubuMVC.OwinHost
{
    public class OwinHttpWriter : IHttpWriter
    {
        private readonly Cache<string, string> _headers;
        private readonly Response _response;

        public OwinHttpWriter(Response response)
        {
            _response = response;
            _headers = new Cache<string, string>(response.Headers);
        }

        public void AppendHeader(string key, string value)
        {
            // TODO -- got to watch this one.  Won't work with dup's
            // cookies won't fly
            _headers[key] = value;
        }

        public void WriteFile(string file)
        {
            using (var fileStream = new FileStream(file, FileMode.Open))
            {
                Write(stream => fileStream.CopyTo(stream, 64000));
            }
        }

        public void WriteContentType(string contentType)
        {
            _response.ContentType = contentType;
        }

        public void Write(string content)
        {
            _response.Write(content);
        }

        public void Redirect(string url)
        {
            // TODO: This is a hack, better way to accomplish this?
            _response.SetStatus(HttpStatusCode.Redirect);
            _response.Headers.Add("Location", url);
            _response.Write(string.Format("<html><head><title>302 Found</title></head><body><h1>Found</h1><p>The document has moved <a href='{0}'>here</a>.</p></body></html>", url));
        }

        public void WriteResponseCode(HttpStatusCode status, string description = null)
        {
            _response.SetStatus(status, description);
        }

        public void AppendCookie(HttpCookie cookie)
        {
            throw new NotImplementedException();
        }

        public void Write(Action<Stream> output)
        {
            Action complete = () => { };
            var stream = new OutputStream((segment, continuation) =>
            {
                _response.BinaryWrite(segment);
                return true;
            }, complete);

            output(stream);
        }
    }

    public class OutputStream : Stream
    {
        private readonly Action complete;
        private readonly Func<ArraySegment<byte>, Action, bool> next;
        private AsyncResult asyncResult;
        private bool completed;

        public OutputStream(Func<ArraySegment<byte>, Action, bool> next, Action complete)
        {
            if (next == null)
                throw new ArgumentNullException("next");
            if (complete == null)
                throw new ArgumentNullException("complete");

            this.next = next;
            this.complete = complete;
        }

        public override bool CanRead
        {
            get { return false; }
        }

        public override bool CanSeek
        {
            get { return false; }
        }

        public override bool CanWrite
        {
            get { return true; }
        }

        public override long Length
        {
            get { throw new NotSupportedException(); }
        }

        public override long Position
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            EnsureNotCompleted();

            next(new ArraySegment<byte>(buffer, offset, count), (Action) null);
        }

        public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback callback,
                                                object state)
        {
            EnsureNotCompleted();

            asyncResult = new AsyncResult(callback, state);

            if (!next(new ArraySegment<byte>(buffer, offset, count), () => asyncResult.SetAsCompleted(null, false)))
                asyncResult.SetAsCompleted(null, true);

            return asyncResult;
        }

        public override void EndWrite(IAsyncResult ar)
        {
            if (asyncResult != ar)
                throw new ArgumentException("Invalid IAsyncResult argument.");

            asyncResult.EndInvoke();
        }

        public override void Close()
        {
            EnsureNotCompleted();
            completed = true;
            complete();
        }

        public override void Flush()
        {
        }

        private void EnsureNotCompleted()
        {
            if (completed) throw new InvalidOperationException("The stream was completed.");
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException();
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException();
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }
    }

    // shamelessly lifted from http://msdn.microsoft.com/en-us/magazine/cc163467.aspx
    internal class AsyncResult : IAsyncResult
    {
        // Fields set at construction which never change while 
        // operation is pending

        // Fields set at construction which do change after 
        // operation completes
        private const Int32 c_StatePending = 0;
        private const Int32 c_StateCompletedSynchronously = 1;
        private const Int32 c_StateCompletedAsynchronously = 2;
        private readonly AsyncCallback m_AsyncCallback;
        private readonly Object m_AsyncState;

        // Field that may or may not get set depending on usage
        private ManualResetEvent m_AsyncWaitHandle;
        private Int32 m_CompletedState = c_StatePending;

        // Fields set when operation completes
        private Exception m_exception;

        public AsyncResult(AsyncCallback asyncCallback, Object state)
        {
            m_AsyncCallback = asyncCallback;
            m_AsyncState = state;
        }

        public Object AsyncState
        {
            get { return m_AsyncState; }
        }

        public Boolean CompletedSynchronously
        {
            get
            {
                return Thread.VolatileRead(ref m_CompletedState) ==
                       c_StateCompletedSynchronously;
            }
        }

        public WaitHandle AsyncWaitHandle
        {
            get
            {
                if (m_AsyncWaitHandle == null)
                {
                    var done = IsCompleted;
                    var mre = new ManualResetEvent(done);
                    if (Interlocked.CompareExchange(ref m_AsyncWaitHandle,
                                                    mre, null) != null)
                    {
                        // Another thread created this object's event; dispose 
                        // the event we just created
                        mre.Close();
                    }
                    else
                    {
                        if (!done && IsCompleted)
                        {
                            // If the operation wasn't done when we created 
                            // the event but now it is done, set the event
                            m_AsyncWaitHandle.Set();
                        }
                    }
                }
                return m_AsyncWaitHandle;
            }
        }

        public Boolean IsCompleted
        {
            get
            {
                return Thread.VolatileRead(ref m_CompletedState) !=
                       c_StatePending;
            }
        }

        public void SetAsCompleted(
            Exception exception, Boolean completedSynchronously)
        {
            // Passing null for exception means no error occurred. 
            // This is the common case
            m_exception = exception;

            // The m_CompletedState field MUST be set prior calling the callback
            var prevState = Interlocked.Exchange(ref m_CompletedState,
                                                 completedSynchronously
                                                     ? c_StateCompletedSynchronously
                                                     : c_StateCompletedAsynchronously);
            if (prevState != c_StatePending)
                throw new InvalidOperationException(
                    "You can set a result only once");

            // If the event exists, set it
            if (m_AsyncWaitHandle != null) m_AsyncWaitHandle.Set();

            // If a callback method was set, call it
            if (m_AsyncCallback != null) m_AsyncCallback(this);
        }

        public void EndInvoke()
        {
            // This method assumes that only 1 thread calls EndInvoke 
            // for this object
            if (!IsCompleted)
            {
                // If the operation isn't done, wait for it
                AsyncWaitHandle.WaitOne();
                AsyncWaitHandle.Close();
                m_AsyncWaitHandle = null; // Allow early GC
            }

            // Operation is done: if an exception occured, throw it
            if (m_exception != null) throw m_exception;
        }
    }

    internal class AsyncResult<TResult> : AsyncResult
    {
        // Field set when operation completes
        private TResult m_result;

        public AsyncResult(AsyncCallback asyncCallback, Object state) :
            base(asyncCallback, state)
        {
        }

        public void SetAsCompleted(TResult result,
                                   Boolean completedSynchronously)
        {
            // Save the asynchronous operation's result
            m_result = result;

            // Tell the base class that the operation completed 
            // sucessfully (no exception)
            base.SetAsCompleted(null, completedSynchronously);
        }

        public new TResult EndInvoke()
        {
            base.EndInvoke(); // Wait until operation has completed 
            return m_result; // Return the result (if above didn't throw)
        }
    }
}