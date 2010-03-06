using System;
using System.IO;
using System.Windows.Forms;

using Izume.Mobile.Odeo.Common;
using Izume.Mobile.Odeo.Common.Rss;

namespace Izume.Mobile.Odeo
{
    public class ChannelManager
    {
        private System.Windows.Forms.Control _parent;
        private Channel _channel;
        HttpDownload _download = null;
        byte[] _data = null;

        public ChannelManager(System.Windows.Forms.Control parent)
        {
            _parent = parent;
            _channel = new Channel();
        }


        public Channel Channel
        {
            get { return _channel; }
        }


        public void Synchronize()
        {
            _data = new byte[0];
            _download = new HttpDownload(_parent, Channel.Link, Channel.Link);
            _download.DownloadProgress += new ProgressChangedEventHandler(DownloadProgress);
            _download.DownloadComplete += new RunWorkerCompletedEventHandler(DownloadComplete);
            _download.Start();
        }


        public void CancelSynchronization()
        {
            if (_download != null)
                _download.Cancel();
        }


        void DownloadProgress(object sender, ProgressChangedEventArgs e)
        {
            byte[] data = (byte[])e.UserState;
            if (_data == null)
            {                
                _data = new byte[data.Length];
                Array.Copy(data, 0, _data, 0, _data.Length);
            }
            else
            {
                byte[] b = new byte[_data.Length];
                Array.Copy(_data, 0, b, 0, b.Length);

                _data = new byte[_data.Length + data.Length];
                Array.Copy(b, 0, _data, 0, b.Length);
                Array.Copy(data, 0, _data, b.Length, data.Length);
            }
        }


        private void DownloadComplete(object sender, RunWorkerCompletedEventArgs e)
        {
            MemoryStream stream = null;
            RunWorkerCompletedEventArgs eventArgs = e;

            try
            {
                if (e.Cancelled == false && e.Error == null)
                {
                    if (_data != null)
                    {
                        stream = new MemoryStream(_data);
                        Channel.Process(stream);
                        Channel.Save();
                    }
                }
            }
            catch (Exception ex)
            {
                eventArgs = new RunWorkerCompletedEventArgs(e.Result, e.Cancelled, ex);
            }
            finally
            {
                this.OnSynchronized(this, eventArgs);

                // Dispose of the download helper and close the memory stream.
                if (_download != null)
                    _download = null;

                if (stream != null)
                    stream.Close();
            }
        }


        public event RunWorkerCompletedEventHandler Synchronized;
        protected virtual void OnSynchronized(object sender, RunWorkerCompletedEventArgs e)
        {
            if (this.Synchronized != null)
                this.Synchronized(sender, e);
        }




    }
}
