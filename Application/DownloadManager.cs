using System;
using System.Collections;
using System.IO;
using System.Windows.Forms;

using Izume.Mobile.Odeo.Common;
using Izume.Mobile.Odeo.Common.Rss;


namespace Izume.Mobile.Odeo
{
    public class DownloadManager : IDisposable
    {
        public event ProgressChangedEventHandler HttpResponseReceived;
        public event ProgressChangedEventHandler HttpDataReceived;
        public event RunWorkerCompletedEventHandler HttpDownloadComplete;

        private System.Windows.Forms.Control _parent;
        private Hashtable _downloads;


        public DownloadManager(Control parent)
        {
            _parent = parent;
            _downloads = new Hashtable();
        }


        public void Start(ChannelItem channelItem)
        {
            HttpDownload download = new HttpDownload(_parent, channelItem.Guid, channelItem.Enclosure.Url);
            download.ResponseReceived += new ProgressChangedEventHandler(ResponseReceived);
            download.DownloadProgress += new ProgressChangedEventHandler(this.DownloadProgress);
            download.DownloadComplete += new RunWorkerCompletedEventHandler(this.DownloadComplete);  
            
            if (_downloads.Contains(download.Guid) == false)
            {
                string filename = Path.Combine(channelItem.Channel.GetDirectory(), channelItem.Enclosure.GetFilename());
                OdeoApplication.TraceMessage("Downloading file {0}...", channelItem.Enclosure.Url);

                _downloads.Add(download.Guid, download);
                download.Start(filename);
            }
        }

     

        public void Stop(string guid)
        {
            if (_downloads != null && _downloads.Contains(guid))
                (_downloads[guid] as HttpDownload).Cancel();
        }


        void ResponseReceived(object sender, ProgressChangedEventArgs e)
        {
            HttpDownload download = sender as HttpDownload;
            if (download != null && this.HttpResponseReceived != null)
                this.HttpResponseReceived(download.Guid, e);                
        }


        private void DownloadProgress(object sender, ProgressChangedEventArgs e)
        {
            HttpDownload download = sender as HttpDownload;
            if (download != null && this.HttpDataReceived != null)
                this.HttpDataReceived(download.Guid, e);
        }

        
        private void DownloadComplete(object sender, RunWorkerCompletedEventArgs e)
        {
            HttpDownload download = sender as HttpDownload;
            if (download != null)
            {
                if (_downloads != null && _downloads.Contains(download.Guid))
                    _downloads.Remove(download.Guid);

                if (this.HttpDownloadComplete != null)
                    this.HttpDownloadComplete(download.Guid, e);
            }                        
        }


        public void Dispose()
        {
            // If there are pending downloads we can't dispose the control until they've
            // been cancelled.
            if (_downloads != null && _downloads.Count > 0)
            {
                IDictionaryEnumerator iterator = _downloads.GetEnumerator();
                HttpDownload download = null;

                while (iterator.MoveNext())
                {
                    download = iterator.Value as HttpDownload;
                    if (download != null)
                        download.Cancel();
                }
            }
        }


    }
}
