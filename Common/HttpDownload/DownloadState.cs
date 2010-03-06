using System;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace Izume.Mobile.Odeo.Common
{
    internal class DownloadState : IDisposable
    {
        private static readonly int BufferSize = 2048;
        internal byte[] ReadBuffer;
        internal Stream ResponseStream = null;
        internal FileStream Filestream = null;

        internal DownloadState()
        {
            this.ReadBuffer = new byte[BufferSize];
        }


        public void Dispose()
        {
            if (ResponseStream != null)
                ResponseStream.Close();

            if (Filestream != null)
                Filestream.Close();
        }
    }
}
