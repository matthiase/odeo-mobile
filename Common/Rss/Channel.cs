using System;
using System.Collections;
using System.IO;
using System.Text;
using System.Xml;
using Izume.Mobile.Odeo.Common;

namespace Izume.Mobile.Odeo.Common.Rss
{
    public class Channel
    {        
        private string _name = String.Empty;
        private string _library = String.Empty;
        private string _title = String.Empty;
        private string _link = String.Empty;
        private string _description = String.Empty;
        private string _language = String.Empty;
        private string _ttl = String.Empty;
        private ChannelItemCollection _items = null;


        public Channel()
        {
            _library = String.Empty;
            _name = String.Empty;
            _link = String.Empty;
            _items = new ChannelItemCollection();
        }


        public Channel(string library, string name, string link)
        {
            _library = library;
            _name = name;
            _link = link;
            _items = new ChannelItemCollection();
        }


        public ChannelItem GetItem(string guid)
        {
            foreach (ChannelItem item in this.Items)
                if (item.Guid == guid) return item;

            return null;
        }

        
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }


        public string Library
        {
            get { return _library; }
            set { _library = value; }
        }


        public string Title
        {
            get { return _title; }
            set { _title = value; }
        }


        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }


        public string Language
        {
            get { return _language; }
            set { _language = value; }
        }


        public string TTL
        {
            get { return _ttl; }
            set { _ttl = value; }
        }


        public string Link
        {
            get { return _link; }
            set { _link = value; }
        }


        public string GetDirectory()
        {
            string dir = String.Empty;
            try
            {
                dir = Path.Combine(this.Library, this.Name);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dir;
        }


        public ChannelItemCollection Items
        {
            get { return _items; }
        }
        

        public void Process(Stream stream)
        {
            XmlTextReader rss = null;
            ChannelItem item = null;
            ChannelItemCollection items = new ChannelItemCollection();
            Exception error = null;

            rss = new XmlTextReader(stream);
            rss.WhitespaceHandling = WhitespaceHandling.None;

            try
            {
                while (rss.Read())
                {
                    try
                    {
                        if (rss.IsStartElement())
                        {
                            if (String.Compare(rss.Name, "title", true) == 0)
                                this.Title = XmlHelper.Decode(rss.ReadString());
                            else if (String.Compare(rss.Name, "link", true) == 0)
                                this.Link = XmlHelper.Decode(rss.ReadString());
                            else if (String.Compare(rss.Name, "description", true) == 0)
                                this.Description = XmlHelper.Decode(rss.ReadString());
                            else if (String.Compare(rss.Name, "language", true) == 0)
                                this.Language = XmlHelper.Decode(rss.ReadString());
                            else if (String.Compare(rss.Name, "ttl", true) == 0)
                                this.TTL = XmlHelper.Decode(rss.ReadString());
                            else if (String.Compare(rss.Name, "item", true) == 0)
                            {
                                item = new ChannelItem(this);
                                while ((String.Compare(rss.Name, "item", true) == 0 &&
                                    rss.NodeType == XmlNodeType.EndElement) == false)
                                {
                                    if (rss.Read() && rss.IsStartElement())
                                    {
                                        if (String.Compare(rss.Name, "title", true) == 0)
                                            item.Title = XmlHelper.Decode(rss.ReadString());
                                        else if (String.Compare(rss.Name, "link", true) == 0)
                                            item.Link = XmlHelper.Decode(rss.ReadString());
                                        else if (String.Compare(rss.Name, "description", true) == 0)
                                            item.Description = XmlHelper.Decode(rss.ReadString());
                                        else if (String.Compare(rss.Name, "guid", true) == 0)
                                            item.Guid = XmlHelper.Decode(rss.ReadString());
                                        else if (String.Compare(rss.Name, "pubDate", true) == 0)
                                            item.PublicationDate = XmlHelper.Decode(rss.ReadString());
                                        else if (String.Compare(rss.Name, "enclosure", true) == 0)
                                        {
                                            item.Enclosure.Url = XmlHelper.Decode(rss.GetAttribute("url"));

                                            try { item.Enclosure.Length = long.Parse(rss.GetAttribute("length")); }
                                            catch (Exception) { item.Enclosure.Length = 0; }

                                            item.Enclosure.Type = XmlHelper.Decode(rss.GetAttribute("type"));
                                        }
                                        else if (String.Compare(rss.Name, "itunes:author", true) == 0)
                                            item.Author = XmlHelper.Decode(rss.ReadString());
                                        else if (String.Compare(rss.Name, "itunes:keywords", true) == 0)
                                            item.Keywords = XmlHelper.Decode(rss.ReadString());
                                        else if (String.Compare(rss.Name, "media:thumbnail", true) == 0)
                                        {
                                            item.Thumbnail = new Thumbnail(item);
                                            item.Thumbnail.Url = XmlHelper.Decode(rss.GetAttribute("url"));

                                            try { item.Thumbnail.Height = int.Parse(rss.GetAttribute("height")); }
                                            catch (Exception) { item.Thumbnail.Height = 0; }

                                            try { item.Thumbnail.Width = int.Parse(rss.GetAttribute("width")); }
                                            catch (Exception) { item.Thumbnail.Width = 0; }
                                        }
                                    }
                                }
                                items.Add(item);
                            }
                        }
                    }
                    catch (XmlException ex)
                    {
                        if (error == null)
                        {
                            error = new XmlException(String.Format("An XML error occurred while processing {0}.",
                                item.Title), ex);
                        }
                    }
                    catch (Exception ex)
                    {
                        if (error == null)
                            error = ex;
                    }

                }
            }
            catch (Exception ex)
            {
                error = new Exception(String.Format("An unknown error occurred while processing {0}.",
                    item.Title), ex);
            }
            finally
            {
                if (rss != null)
                    rss.Close();
            }

            if (error != null)
                throw error;
            
            _items.Merge(items);

        }


        public void Load()
        {
            string dir = this.GetDirectory();
            string file = Path.Combine(dir, String.Format("{0}.channel", this.Name));

            try
            {
                if (Directory.Exists(dir) && File.Exists(file))
                {
                    using (FileStream fs = new FileStream(file, FileMode.Open,
                        FileAccess.Read, FileShare.Read))
                    {
                        if (fs != null && fs.CanRead)
                            this.Process(fs);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public void Save()
        {
            // Save the data to file, creating the directory if necessary.
            string dir = this.GetDirectory();
            if (Directory.Exists(dir) == false)
            {
                try
                {
                    Directory.CreateDirectory(dir);
                }
                catch (UnauthorizedAccessException ex)
                {
                    string message = String.Format("Unable to create directory {0}.  Please check your file permissions.", dir);
                    throw new Exception(message, ex);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

            string filename = Path.Combine(dir, String.Format("{0}.channel", this.Name));
            using (FileStream fs = new FileStream(filename, FileMode.Create, FileAccess.Write, FileShare.Write))
            {
                if (fs != null && fs.CanWrite)
                {
                    try
                    {
                        byte[] buffer = Encoding.UTF8.GetBytes(this.Xml);
                        fs.Write(buffer, 0, buffer.Length);
                    }
                    catch (Exception ex)
                    {
                        string message = String.Format("Failed to save channel file {0}.", filename != null ? filename : "");
                        throw new Exception(message, ex);
                    }
                }
            }
        }


        public string Xml
        {
            get
            {
                StringBuilder buffer = new StringBuilder();
                buffer.Append("<?xml version=\"1.0\" encoding=\"UTF-8\" ?>\n"); 
                buffer.Append("<rss xmlns:itunes=\"http://www.itunes.com/dtds/podcast-1.0.dtd\" version=\"2.0\" xmlns:media=\"http://search.yahoo.com/mrss\">\n");
                buffer.Append("\t<channel>\n");
                buffer.Append("\t<title>").Append(XmlHelper.Encode(_title)).Append("</title>\n");
                buffer.Append("\t<link>").Append(XmlHelper.Encode(_link)).Append("</link>\n");
                buffer.Append("\t<description>").Append(XmlHelper.Encode(_description)).Append("</description>\n");
                buffer.Append("\t<language>").Append(XmlHelper.Encode(_language)).Append("</language>\n");
                buffer.Append("\t<ttl>").Append(XmlHelper.Encode(_ttl)).Append("</ttl>\n");

                foreach (ChannelItem item in this.Items)
                    buffer.Append(item.Xml);

                buffer.Append("\t</channel>\n");
                buffer.Append("</rss>");
                return buffer.ToString();

            }
        }

    }
}
