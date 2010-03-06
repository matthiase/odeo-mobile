using System;
using System.Collections;
using System.IO;

namespace Izume.Mobile.Odeo
{
    public class Preferences
    {
        private Hashtable _values;

        public string Library
        {
            get 
            {
                string libdir = this.GetValue("libdir");
                if (libdir.Length == 0)
                    libdir = OdeoApplication.RootDirectory;

                return libdir;
            }
            set { this.SetValue("libdir", value); }
        }

        //public string RootDirectory
        //{
        //    get { return this.GetValue("rootdir"); }
        //    set { this.SetValue("rootdir", value); }
        //}

        public string Username
        {
            get { return this.GetValue("username"); }
            set { this.SetValue("username", value); }
        }

        public string BaseUrl
        {
            get { return this.GetValue("baseurl"); }
            set { this.SetValue("baseurl", value); }
        }

        public Preferences()
        {
            _values = new Hashtable();
        }

        public void Load(string filename)
        {
            using (FileStream fs = new FileStream(filename, FileMode.OpenOrCreate, FileAccess.Read, FileShare.Read)) 
            {
                if (fs != null && fs.CanRead)
                {
                    string[] tokens = null;
                    StreamReader reader = new StreamReader(fs);

                    while (reader.Peek() != -1)
                    {
                        tokens = reader.ReadLine().Split(new char[] { '=' });
                        if (tokens.Length == 2)
                            this.SetValue(tokens[0], tokens[1]);
                    }
                }
            }
        }

        public void Save(string filename)
        {
            using (FileStream fs = new FileStream(filename, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Write))
            {
                if (fs != null && fs.CanWrite)
                {
                    StreamWriter writer = new StreamWriter(fs);
                    foreach (string key in _values.Keys)
                        writer.WriteLine(String.Format("{0}={1}", key, _values[key]));

                    writer.Flush();
                }
            }
        }

        public string GetValue(string key)
        {
            try
            {
                if (_values != null && _values.Contains(key))
                    return Convert.ToString(_values[key]);
                else
                    return String.Empty;
            }
            catch (Exception)
            {
                return String.Empty;
            }
        }

        public void SetValue(string key, string val)
        {
            _values[key] = val;
        }
    }
}