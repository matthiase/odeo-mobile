using System;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using Izume.Mobile.Odeo.Common;
using Izume.Mobile.Odeo.Common.Rss;
using Izume.Mobile.Odeo.Common.Logging;

namespace Izume.Mobile.Odeo
{
    public class OdeoApplication
    {
        public static Form MainForm;
        private static string _rootdir = "";
        private static Preferences _preferences;
        
        
        public static void Initialize(Form form)
        {
            MainForm = form;
            MainForm.Closed += new EventHandler(MainForm_Closed);
            Application.Run(MainForm);
        }


        public static void Exit()
        {
            if (MainForm != null)
                MainForm.Close();
        }


        private static void MainForm_Closed(object sender, EventArgs e)
        {
            if (MainForm != null)
                MainForm.Dispose();

            Application.Exit();
        }


        private OdeoApplication()
        {

        }

        
        public static string RootDirectory
        {
            get { return _rootdir; }
            set { _rootdir = value; }
        }


        public static Preferences Preferences
        {
            get
            {
                if (_preferences == null)
                {
                    _preferences = new Preferences();
                    _preferences.Load(Path.Combine(OdeoApplication.RootDirectory, 
                        OdeoApplication.Constants.PREFERENCE_FILE));
                }

                return _preferences;
            }
        }


        public class Constants
        {
            public static readonly string STARTUP_LOGO = "Izume.Mobile.Odeo.Smartphone.Resource.odeo.bmp";
            public static readonly string PREFERENCE_FILE = "syncr.config";
            public static readonly string DEFAULT_URL = "http://www.odeo.com/profile/{0}/rss";
            public static readonly int DOWNLOAD_BUFFER_SIZE = 10240;
        }


        public static void TraceException(string source, Exception ex)
        {
            System.Text.StringBuilder buffer = new System.Text.StringBuilder();
            buffer.Append("[Exception in ").Append(source).Append("]\n");
            buffer.Append("\t").Append(ex.Message).Append("\n");
            if (ex.InnerException != null)
                buffer.Append("\t").Append(ex.InnerException.Message).Append("\n");

            TextLogger.WriteLine(buffer.ToString());
            System.Windows.Forms.MessageBox.Show(buffer.ToString());
        }


        public static void TraceMessage(string message, params object[] args)
        {
            TextLogger.WriteLine(message, args);
        }

    }
}
