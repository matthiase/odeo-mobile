using System;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Windows.Forms;


namespace Izume.Mobile.Odeo.Common
{   
    public class HttpDownload
    {
        private static readonly int BufferSize = 10240;
        private static readonly int ResponseTimeout = 60000;

        private Control _parent;
        private System.Threading.Timer _timeout;
        private string _guid;
        private string _url;
        private long _length;
        private long _received;
        private byte[] _buffer;
        private int _bufferIndex;
        private HttpWebRequest _request;
        private DownloadState _state;
        private bool _cancelled = false;

        public event ProgressChangedEventHandler ResponseReceived;
        public event ProgressChangedEventHandler DownloadProgress;
        public event RunWorkerCompletedEventHandler DownloadComplete;
        private ProgressChangedEventArgs _progressEventArgs = null;
        private RunWorkerCompletedEventArgs _completedEventArgs = null;

        public HttpDownload(Control parent, string guid, string url)
        {
            _parent = parent;
            _guid = guid;
            _url = url;
            _buffer = new byte[BufferSize];
        }


        public string Guid
        {
            get { return _guid; }
        }


        public string Url
        {
            get { return _url; }
        }


        public long Length
        {
            get { return _length; }
        }


        public long ReceivedLength
        {
            get { return _received; }
        }


        public void Start()
        {
            try
            {
                _state = new DownloadState();
                _request = WebRequest.Create(this.Url) as HttpWebRequest;

                _timeout = new System.Threading.Timer(new TimerCallback(this.OnTimeout), _state,
                    ResponseTimeout, System.Threading.Timeout.Infinite);

                _request.BeginGetResponse(new AsyncCallback(this.HttpResponseReceived), _state);
            }
            catch (Exception ex)
            {
                this.NotifyParentOfCompletion(false, ex);
            }
        }


        public void Start(string filename)
        {
            try
            {
                _state = new DownloadState();
                _state.Filestream = new FileStream(filename, FileMode.Append, FileAccess.Write, FileShare.Write);
                _received = _state.Filestream.Length;

                _request = WebRequest.Create(this.Url) as HttpWebRequest;
                _request.AddRange((int)_state.Filestream.Length);
               
                _timeout = new System.Threading.Timer(new TimerCallback(this.OnTimeout), _state,
                    ResponseTimeout, System.Threading.Timeout.Infinite);

                _request.BeginGetResponse(new AsyncCallback(this.HttpResponseReceived), _state);
            }
            catch (Exception ex)
            {
                this.NotifyParentOfCompletion(false, ex);
            }
        }


        public void Cancel()
        {
            _cancelled = true;
        }


        public void Dispose()
        {
            if (_timeout != null)
                _timeout.Dispose();

            if (_state != null)
                _state.Dispose();
        }


        public override bool Equals(object obj)
        {
            bool isEqual = false;
            if (obj is HttpDownload)
                isEqual = (obj as HttpDownload).Guid.Equals(this.Guid);
            else if (obj is string)
                isEqual = (obj as string).Equals(this.Guid);

            return isEqual;
        }


        public override int GetHashCode()
        {
            return this.Guid.GetHashCode();
        }



        
        protected virtual void OnResponseReceived(object sender, EventArgs e)
        {
            if (this.ResponseReceived != null)
                this.ResponseReceived(this, _progressEventArgs);
        }


        protected virtual void OnDownloadProgress(object sender, EventArgs e)
        {
            if (this.DownloadProgress != null)
                this.DownloadProgress(this, _progressEventArgs);
        }


        protected virtual void OnDownloadComplete(object sender, EventArgs e)
        {
            if (this.DownloadComplete != null)
                this.DownloadComplete(this, _completedEventArgs);
        }       




        private void HttpResponseReceived(IAsyncResult result)
        {
            HttpWebResponse response = null;
            DownloadState state = null;

            try
            {
                if (_timeout != null)
                    _timeout.Change(System.Threading.Timeout.Infinite, System.Threading.Timeout.Infinite);

                if (_cancelled)
                {
                    if (_request != null)
                        _request = null;

                    this.NotifyParentOfCompletion(true, null);
                }
                else
                {
                    response = _request.EndGetResponse(result) as HttpWebResponse;
                    state = result.AsyncState as DownloadState;
                    state.ResponseStream = response.GetResponseStream();
                    _length = (response.ContentLength > 0) ? response.ContentLength : -1;

                    // Notify the parent that a response has been received and start reading the response stream.
                    this.NotifyParentOfResponse(this.Length, this.ReceivedLength);

                    _timeout.Change(ResponseTimeout, System.Threading.Timeout.Infinite);
                    state.ResponseStream.BeginRead(state.ReadBuffer, 0, state.ReadBuffer.Length, new AsyncCallback(this.HttpDataReceived), state);
                }
            }
            catch (Exception ex)
            {
                this.NotifyParentOfCompletion(false, ex);
            }
        }


        private void HttpDataReceived(IAsyncResult result)
        {
            DownloadState state = null;
            try
            {
                if (_timeout != null)
                    _timeout.Change(System.Threading.Timeout.Infinite, System.Threading.Timeout.Infinite);

                state = result.AsyncState as DownloadState;
                int length = state.ResponseStream.EndRead(result);

                if (length > 0)
                {
                    _received += length;
                    int available = _buffer.Length - _bufferIndex;

                    // If the received bytes exceed the available space in the buffer, it's time to 
                    // flush out the buffer and notify anyone listening that progress had been made.
                    if (length > available)
                    {                        
                        // Copy as much of the data as will fit into the buffer.  Then copy the
                        // the entire buffer to a temporary array and notify the parent.
                        byte[] data = new byte[_buffer.Length];
                        Array.Copy(state.ReadBuffer, 0, _buffer, _bufferIndex, available);
                        Array.Copy(_buffer, 0, data, 0, data.Length);


                        // Re-initialize the buffer, and copy the remainder of the data there.
                        _buffer = new byte[_buffer.Length];
                        Array.Copy(state.ReadBuffer, available, _buffer, 0, (length - available));
                        _bufferIndex = length - available;

                        if (state.Filestream != null)
                            state.Filestream.Write(data, 0, data.Length);

                        this.NotifyParentOfProgress(this.Length, this.ReceivedLength, data);
                    }
                    else
                    {
                        // If there's enough space in the buffer, append it.
                        Array.Copy(state.ReadBuffer, 0, _buffer, _bufferIndex, length);
                        _bufferIndex += length;
                    }

                    // If a request to cancel the download has been submitted, don't continue reading from the response
                    // stream and cancel the request.
                    if (_cancelled == true)
                    {
                        if (_request != null)
                            _request = null;

                        this.NotifyParentOfCompletion(true, null);
                    }
                    else
                    {
                        // Start the next invocation of the reading the response stream.
                        state.ReadBuffer = new byte[state.ReadBuffer.Length];
                        _timeout.Change(ResponseTimeout, System.Threading.Timeout.Infinite);
                        state.ResponseStream.BeginRead(state.ReadBuffer, 0, state.ReadBuffer.Length, new AsyncCallback(HttpDataReceived), state);
                    }
                }
                else
                {                    
                    // If there's data in the buffer, flush it out.
                    if (_bufferIndex > 0)
                    {
                        byte[] data = new byte[this._bufferIndex];
                        Array.Copy(_buffer, 0, data, 0, _bufferIndex);

                        if (state.Filestream != null)
                            state.Filestream.Write(data, 0, data.Length);

                        this.NotifyParentOfProgress(this.Length, this.ReceivedLength, data);
                    }
                    
                    this.NotifyParentOfCompletion(false, null);
                    state.Dispose();
                }
            }
            catch (Exception ex)
            {
                this.NotifyParentOfCompletion(false, ex);
                state.Dispose();
            }
        }


        private void NotifyParentOfResponse(long total, long completed)
        {
            try
            {
                _progressEventArgs = new ProgressChangedEventArgs(total, completed, null);
                if (_parent != null)
                    _parent.Invoke(new System.EventHandler(this.OnResponseReceived));
                else
                    this.OnResponseReceived(this, _progressEventArgs);
            }
            catch (Exception)
            {
                if (_state != null)
                    _state.Dispose();
            }
        }


        private void NotifyParentOfProgress(long total, long completed, byte[] data)
        {
            try
            {
                _progressEventArgs = new ProgressChangedEventArgs(total, completed, data);
                if (_parent != null)
                    _parent.Invoke(new System.EventHandler(this.OnDownloadProgress));
                else
                    this.OnDownloadProgress(this, _progressEventArgs);
            }
            catch (Exception)
            {
                if (_state != null)
                    _state.Dispose();
            }
        }


        private void NotifyParentOfCompletion(bool cancelled, Exception error)
        {
            try
            {
                _completedEventArgs = new RunWorkerCompletedEventArgs(null, cancelled, error);
                if (_parent != null)
                    _parent.Invoke(new System.EventHandler(this.OnDownloadComplete));
                else
                    this.OnDownloadComplete(this, _completedEventArgs);
            }
            catch (Exception)
            {
                if (_state != null)
                    _state.Dispose();
            }
        }


        private void OnTimeout(object state)
        {
            try
            {
                if (_timeout != null)
                    _timeout.Change(System.Threading.Timeout.Infinite, System.Threading.Timeout.Infinite);

                if (_request != null)
                    _request.Abort();

                DownloadState downloadState = state as DownloadState;
                if (downloadState != null)
                    downloadState.Dispose();

            }
            finally
            {
                NotifyParentOfCompletion(false, new Exception("A timeout occurred while waiting for a response."));
            }
        }

    }
}
