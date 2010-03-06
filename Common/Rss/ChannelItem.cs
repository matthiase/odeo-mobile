using System;
using System.Text;


namespace Izume.Mobile.Odeo.Common.Rss
{
    public class ChannelItem : IComparable
    {
        private Channel _channel;
        private string _title = String.Empty;
        private string _link = String.Empty;
        private string _description = String.Empty;
        private string _publicationDate = String.Empty;
        private string _guid = String.Empty;
        private Enclosure _enclosure = null;
        private string _author = String.Empty;
        private string _keywords = String.Empty;
        private Thumbnail _thumbnail = null;
        private string _visible = String.Empty;


        public ChannelItem(Channel channel)
        {
            _channel = channel;
            _enclosure = new Enclosure(this);
        }

        public Channel Channel
        {
            get { return _channel; }
        }

        public string Title
        {
            get { return _title; }
            set { _title = value; }
        }

        public string Link
        {
            get { return _link; }
            set { _link = value; }
        }

        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }

        public string PublicationDate
        {
            get { return _publicationDate; }
            set { _publicationDate = value; }
        }

        public string Guid
        {
            get { return _guid; }
            set { _guid = value; }
        }

        public string Author
        {
            get { return _author; }
            set { _author = value; }
        }

        public string Keywords
        {
            get { return _keywords; }
            set { _keywords = value; }
        }

        public Thumbnail Thumbnail
        {
            get { return _thumbnail; }
            set { _thumbnail = value; }
        }

        public String Visible
        {
            get { return _visible; }
            set { _visible = value; }
        }

        public Enclosure Enclosure
        {
            get { return _enclosure; }
        }

        public String Xml
        {
            get
            {
                StringBuilder buffer = new StringBuilder();
                buffer.Append("\t<item>\n");
                buffer.Append("\t\t<title>").Append(XmlHelper.Encode(this.Title)).Append("</title>\n");
                buffer.Append("\t\t<link>").Append(XmlHelper.Encode(this.Link)).Append("</link>\n");
                buffer.Append("\t\t<description>").Append(XmlHelper.Encode(this.Description)).Append("</description>\n");
                buffer.Append("\t\t<pubDate>").Append(XmlHelper.Encode(this.PublicationDate)).Append("</pubDate>\n");
                buffer.Append("\t\t<guid>").Append(XmlHelper.Encode(this.Guid)).Append("</guid>\n");
                
                if (this.Enclosure != null)
                    buffer.Append(this.Enclosure.Xml);

                if (this.Author != null && this.Author.Length > 0)
                    buffer.Append("\t\t<itunes:author>").Append(XmlHelper.Encode(this.Author)).Append("</itunes:author>\n");

                if (this.Keywords != null && this.Keywords.Length > 0)
                    buffer.Append("\t\t<itunes:keywords>").Append(XmlHelper.Encode(this.Keywords)).Append("</itunes:keywords>\n");
                
                if (this.Thumbnail != null && this.Thumbnail.Url.Length > 0)
                    buffer.Append(this.Thumbnail.Xml);

                buffer.Append("\t</item>\n");
                return buffer.ToString();
            }
        }

        public override bool Equals(object obj)
        {
            bool isEqual = false;
            if (obj is ChannelItem)
                isEqual = (obj as ChannelItem).Guid.Equals(this.Guid);
            else if (obj is String)
                isEqual = (obj as String).Equals(this.Guid);

            return isEqual;
        }

        public override int GetHashCode()
        {
            return this.Guid.GetHashCode();
        }

        public override string ToString()
        {
            return this.Title;
        }

        #region IComparable Implementation

        int IComparable.CompareTo(object other)
        {
            ChannelItem channelItem = other as ChannelItem;
            int result = 0;
            
            if (other != null)
                result = channelItem.Title.CompareTo(this.Title);

            return result;
        }

        #endregion

    }
}
