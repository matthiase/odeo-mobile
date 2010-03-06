using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace Izume.Mobile.Odeo.Common.Rss
{
    public class Enclosure
    {
        private ChannelItem _channelItem;
        private string _url = String.Empty;
        private string _type = String.Empty;
        private long _length = 0;

        public Enclosure(ChannelItem parent)
        {
            _channelItem = parent;
        }

        public ChannelItem ChannelItem
        {
            get { return _channelItem; }
        }

        public string Url
        {
            get { return _url; }
            set { _url = value; }
        }

        public string Type
        {
            get { return _type; }
            set { _type = value; }
        }

        public long Length
        {
            get { return _length; }
            set { _length = value; }
        }

        public String Xml
        {
            get
            {
                StringBuilder buffer = new StringBuilder();
                buffer.Append("\t\t<enclosure");
                buffer.Append(" url=\"").Append(XmlHelper.Encode(this.Url)).Append("\"");
                buffer.Append(" type=\"").Append(XmlHelper.Encode(this.Type)).Append("\"");
                buffer.Append(" length=\"").Append(this.Length).Append("\"");
                buffer.Append(" />");
                buffer.Append("\n");
                return buffer.ToString();
            }
        }


        public string GetFilename()
        {
            string filename = String.Empty;
            string[] tokens = this.Url.Split(new char[] { '/' });
            if (tokens != null && tokens.Length > 0)
                filename = tokens[tokens.Length - 1];

            return filename;
        }

    }
}
