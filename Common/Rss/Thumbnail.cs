using System;
using System.Text;

namespace Izume.Mobile.Odeo.Common.Rss
{
    public class Thumbnail
    {
        private ChannelItem _parent = null;
        private string _url = String.Empty;
        private int _height = 0;
        private int _width = 0;

        public Thumbnail(ChannelItem parent)
        {
            _parent = parent;
        }

        public ChannelItem Parent
        {
            get { return _parent; }
        }

        public string Url
        {
            get { return _url; }
            set { _url = value; }
        }

        public int Height
        {
            get { return _height; }
            set { _height = value; }
        }

        public int Width
        {
            get { return _width; }
            set { _width = value; }
        }

        public string Xml
        {
            get
            {
                StringBuilder buffer = new StringBuilder();
                buffer.Append("\t\t<media:thumbnail");
                buffer.Append(" url=\"").Append(XmlHelper.Encode(this.Url)).Append("\"");
                buffer.Append(" height=\"").Append(this.Height).Append("\"");
                buffer.Append(" width=\"").Append(this.Width).Append("\"");
                buffer.Append(" />\n");
                return buffer.ToString();
            }
        }


    }
}
