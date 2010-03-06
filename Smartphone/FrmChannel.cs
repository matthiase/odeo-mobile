using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;

using Izume.Mobile.Odeo.Common;
using Izume.Mobile.Odeo.Common.Rss;
using Izume.Mobile.Odeo.Common.Logging;
using Microsoft.WindowsCE.Forms;


namespace Izume.Mobile.Odeo.Smartphone
{
    public class FrmChannel : System.Windows.Forms.Form
    {
        private ChannelManager _channelManager;
        private DownloadManager _downloadManager;

        private ImageList SmallIcons;
        private MenuItem MenuSynchronize;
        private MenuItem MenuApplication;
        private System.Windows.Forms.MainMenu MainMenu;        
        private MenuItem MenuDownload;
        private MenuItem MenuCancel;
        private MenuItem MenuPlay;        
        private BackgroundWorker StartupWorker;
        private FrmStartup StartupForm;

        private Panel MainPanel;
        private MenuItem MenuPreferences;
        private MenuItem MenuRemove;
        private MenuItem MenuExit;
        private Tasklist DownloadList = null;

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmChannel));
            this.MainMenu = new System.Windows.Forms.MainMenu();
            this.MenuSynchronize = new System.Windows.Forms.MenuItem();
            this.MenuApplication = new System.Windows.Forms.MenuItem();
            this.MenuDownload = new System.Windows.Forms.MenuItem();
            this.MenuCancel = new System.Windows.Forms.MenuItem();
            this.MenuPlay = new System.Windows.Forms.MenuItem();
            this.MenuRemove = new System.Windows.Forms.MenuItem();
            this.MenuPreferences = new System.Windows.Forms.MenuItem();
            this.SmallIcons = new System.Windows.Forms.ImageList();
            this.MainPanel = new System.Windows.Forms.Panel();
            this.MenuExit = new System.Windows.Forms.MenuItem();
            // 
            // MainMenu
            // 
            this.MainMenu.MenuItems.Add(this.MenuSynchronize);
            this.MainMenu.MenuItems.Add(this.MenuApplication);
            // 
            // MenuSynchronize
            // 
            this.MenuSynchronize.Text = "Sync";
            this.MenuSynchronize.Click += new System.EventHandler(this.MenuSynchronize_Click);
            // 
            // MenuApplication
            // 
            this.MenuApplication.MenuItems.Add(this.MenuDownload);
            this.MenuApplication.MenuItems.Add(this.MenuCancel);
            this.MenuApplication.MenuItems.Add(this.MenuPlay);
            this.MenuApplication.MenuItems.Add(this.MenuRemove);
            this.MenuApplication.MenuItems.Add(this.MenuPreferences);
            this.MenuApplication.MenuItems.Add(this.MenuExit);
            this.MenuApplication.Text = "Menu";
            // 
            // MenuDownload
            // 
            this.MenuDownload.Text = "Download";
            this.MenuDownload.Click += new System.EventHandler(this.MenuDownload_Click);
            // 
            // MenuCancel
            // 
            this.MenuCancel.Text = "Cancel";
            this.MenuCancel.Click += new System.EventHandler(this.MenuCancel_Click);
            // 
            // MenuPlay
            // 
            this.MenuPlay.Text = "Play";
            this.MenuPlay.Click += new System.EventHandler(this.MenuPlay_Click);
            // 
            // MenuRemove
            // 
            this.MenuRemove.Text = "Remove";
            this.MenuRemove.Click += new System.EventHandler(this.MenuRemove_Click);
            // 
            // MenuPreferences
            // 
            this.MenuPreferences.Text = "Preferences";
            this.MenuPreferences.Click += new System.EventHandler(this.MenuPreferences_Click);
            this.SmallIcons.Images.Clear();
            this.SmallIcons.Images.Add(((System.Drawing.Image)(resources.GetObject("resource"))));
            this.SmallIcons.Images.Add(((System.Drawing.Image)(resources.GetObject("resource1"))));
            this.SmallIcons.Images.Add(((System.Drawing.Image)(resources.GetObject("resource2"))));
            this.SmallIcons.Images.Add(((System.Drawing.Image)(resources.GetObject("resource3"))));
            this.SmallIcons.Images.Add(((System.Drawing.Image)(resources.GetObject("resource4"))));
            this.SmallIcons.Images.Add(((System.Drawing.Image)(resources.GetObject("resource5"))));
            this.SmallIcons.Images.Add(((System.Drawing.Image)(resources.GetObject("resource6"))));
            this.SmallIcons.Images.Add(((System.Drawing.Image)(resources.GetObject("resource7"))));
            this.SmallIcons.Images.Add(((System.Drawing.Image)(resources.GetObject("resource8"))));
            this.SmallIcons.Images.Add(((System.Drawing.Image)(resources.GetObject("resource9"))));
            this.SmallIcons.Images.Add(((System.Drawing.Image)(resources.GetObject("resource10"))));
            this.SmallIcons.Images.Add(((System.Drawing.Image)(resources.GetObject("resource11"))));
            this.SmallIcons.Images.Add(((System.Drawing.Image)(resources.GetObject("resource12"))));
            // 
            // MainPanel
            // 
            this.MainPanel.Location = new System.Drawing.Point(-1, -1);
            this.MainPanel.Size = new System.Drawing.Size(179, 181);
            this.MainPanel.Visible = false;
            // 
            // MenuExit
            // 
            this.MenuExit.Text = "Exit";
            this.MenuExit.Click += new System.EventHandler(this.MenuExit_Click);
            // 
            // FrmChannel
            // 
            this.ClientSize = new System.Drawing.Size(176, 180);
            this.Controls.Add(this.MainPanel);
            this.Menu = this.MainMenu;
            this.Text = "Odeo Syncr";

        }

        #endregion


        public static void Main()
        {
            OdeoApplication.Initialize(new FrmChannel());
        }


        public FrmChannel()
        {
            InitializeComponent();

            OdeoApplication.RootDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetName().CodeBase);
            TextLogger.DirectoryName = OdeoApplication.RootDirectory;

            try
            {
                // The startup worker initializes the application.  Among other things, it reads the 
                // saved user preferences from disk and creates the OdeoApplication.Preferences object.

                this.StartupWorker = new BackgroundWorker(this);
                this.StartupWorker.WorkerReportsProgress = true;
                this.StartupWorker.DoWork += new DoWorkEventHandler(StartupWorker_DoWork);
                this.StartupWorker.ProgressChanged += new ProgressChangedEventHandler(StartupWorker_ProgressChanged);
                this.StartupWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(StartupWorker_RunWorkerCompleted);

                _channelManager = new ChannelManager(this);
                _channelManager.Channel.Library = OdeoApplication.Preferences.Library;
                _channelManager.Channel.Name = OdeoApplication.Preferences.Username;
                _channelManager.Channel.Link = String.Format(OdeoApplication.Preferences.BaseUrl, OdeoApplication.Preferences.Username);
                _channelManager.Synchronized += new RunWorkerCompletedEventHandler(Synchronized);

                _downloadManager = new DownloadManager(this);
                _downloadManager.HttpResponseReceived += new ProgressChangedEventHandler(HttpResponseReceived);
                _downloadManager.HttpDataReceived += new ProgressChangedEventHandler(HttpDataReceived);
                _downloadManager.HttpDownloadComplete += new RunWorkerCompletedEventHandler(HttpDownloadComplete);

                this.DownloadList = new Tasklist();
                this.DownloadList.BackColor = Color.White;
                this.DownloadList.Location = new Point(0, 0);
                this.DownloadList.Size = new Size(this.MainPanel.Width, this.MainPanel.Height);
                this.MainPanel.Controls.Add(this.DownloadList);
            }
            catch (Exception ex)
            {
                OdeoApplication.TraceException("FrmChannel::Constructor", ex);
            }
        }


        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            try
            {
                this.DownloadList.Bounds = new Rectangle(2, 2, this.MainPanel.Width - 2, this.MainPanel.Height - 2);
                this.StartupForm = new FrmStartup();
                StartupWorker.RunWorkerAsync();
                this.StartupForm.ShowDialog();
            }
            catch (Exception ex)
            {
                OdeoApplication.TraceException("FrmChannel::OnLoad", ex);
            }
        }


        protected override void Dispose(bool disposing)
        {
            try
            {
                if (this.DownloadList != null)
                    this.DownloadList.Dispose();
            }
            finally
            {
                base.Dispose(disposing);
            }
        }


        protected override void OnClosing(CancelEventArgs e)
        {
            try
            {
                if (_downloadManager != null)
                    _downloadManager.Dispose();
            }
            catch (Exception ex)
            {
                OdeoApplication.TraceException("FrmChannel::OnClosing", ex);
            }
            finally
            {
                base.OnClosing(e);
            }
        }


        private string GetProgressDescription(TasklistItem item)
        {

            string completed = ConvertFileSize(item.Completed);
            string total = ConvertFileSize(item.Total);
            return String.Format("{0} of {1}", completed, total);
        }


        private string ConvertFileSize(long size)
        {
            string convertedSize = null;
            if (size > 1024000000)
            {
                convertedSize = (size >> 10).ToString();
                convertedSize = convertedSize.Substring(0, convertedSize.Length - 6) + "." + convertedSize.Substring(convertedSize.Length - 6, 2) + " Gb";
            }
            else if (size > 1024000)
            {
                convertedSize = (size >> 10).ToString();
                convertedSize = convertedSize.Substring(0, convertedSize.Length - 3) + "." + convertedSize.Substring(convertedSize.Length - 3, 2) + " Mb";
            }
            else if (size > 1024)
                convertedSize = (size >> 10).ToString() + " Kb";
            else
                convertedSize = size.ToString() + " b";

            return convertedSize;
        }


        #region Startup Worker

        void StartupWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.MainPanel.Visible = true;
            
            if (this.StartupForm != null)
                this.StartupForm.Close();
            
            if (OdeoApplication.Preferences.Username.Length == 0)
                this.ShowPreferences();

            OdeoApplication.TraceMessage("Application initialization complete.");

        }

        void StartupWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (this.StartupForm != null)
                this.StartupForm.PercentComplete += (int) e.Completed;
        }

        void StartupWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                Thread.Sleep(500);
                StartupWorker.ReportProgress(100, 10, null);

                OdeoApplication.Preferences.Load(Path.Combine(OdeoApplication.RootDirectory, OdeoApplication.Constants.PREFERENCE_FILE));
                StartupWorker.ReportProgress(100, 30, null);

                _channelManager.Channel.Load();
                StartupWorker.ReportProgress(100, 60, null);

                this.Invoke(new EventHandler(this.UpdateUI));
                Thread.Sleep(200);
                StartupWorker.ReportProgress(100, 80, null);

                Thread.Sleep(500);
                StartupWorker.ReportProgress(100, 100, null);
                Thread.Sleep(500);
            }
            catch (Exception ex)
            {
                OdeoApplication.TraceException("FrmChannel::StartupWorker_DoWork", ex);
            }
        }

        #endregion


        #region Download Event Handlers


        private void HttpResponseReceived(object sender, ProgressChangedEventArgs e)
        {
            try
            {
                string guid = sender as string;
                TasklistItem tasklistItem = this.DownloadList.GetTaskListItem(guid);

                if (tasklistItem != null)
                {
                    tasklistItem.State = TasklistItem.ItemState.Incomplete;
                    if (tasklistItem.Total < e.Total)
                        tasklistItem.Total = e.Total;

                    this.DownloadList.Invalidate();
                }

                ChannelItem channelItem = _channelManager.Channel.GetItem(guid);
                if (channelItem != null && channelItem.Enclosure.Length < e.Total)
                {
                    channelItem.Enclosure.Length = e.Total;
                    _channelManager.Channel.Save();
                }
            }
            catch (Exception ex)
            {
                OdeoApplication.TraceException("FrmChannel::HttpResponseReceived", ex);
            }
        }


        private void HttpDataReceived(object sender, ProgressChangedEventArgs e)
        {
            string guid = sender as string;
            if (guid != null)
            {
                try
                {
                    TasklistItem listItem = this.DownloadList.GetTaskListItem(guid);
                    listItem.Completed = e.Completed;
                    listItem.Description = this.GetProgressDescription(listItem);
                    this.DownloadList.Invalidate();
                }
                catch (Exception ex)
                {
                    OdeoApplication.TraceException("FrmChannel::HttpDataReceived", ex);
                }
            } 
        }


        private void HttpDownloadComplete(object sender, RunWorkerCompletedEventArgs e)
        {
            string guid = sender as string;
            if (guid != null)
            {
                try
                {
                    TasklistItem listItem = this.DownloadList.GetTaskListItem(guid);
                    if (listItem != null)
                    {
                        if (e.Error != null)
                        {
                            OdeoApplication.TraceMessage("[{0}]: An error occurred while downloading.", listItem.Name);
                            listItem.State = TasklistItem.ItemState.Failed;
                        }
                        else if (e.Cancelled)
                        {
                            OdeoApplication.TraceMessage("[{0}]: Download cancelled by user.", listItem.Name);
                            listItem.State = TasklistItem.ItemState.Cancelled;
                        }
                        else
                        {
                            listItem.Completed = listItem.Total;
                            listItem.State = TasklistItem.ItemState.Complete;
                            listItem.Description = this.GetProgressDescription(listItem);
                            OdeoApplication.TraceMessage("[{0}]: Downloaded {1}", listItem.Name, this.GetProgressDescription(listItem));
                        }

                        this.DownloadList.Invalidate();
                    }
                }
                catch (Exception ex)
                {
                    OdeoApplication.TraceException("FrmChannel::HttpDownloadComplete", ex);
                }
            }
        }


        #endregion


        #region Synchronization Event Handlers


        private void Synchronized(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                if (e.Error == null && e.Cancelled == false)
                    this.Invoke(new EventHandler(this.UpdateUI));
                else if (e.Error != null)
                    OdeoApplication.TraceException("FrmChannel::Synchronized", e.Error);                    
            }
            catch (Exception ex)
            {
                OdeoApplication.TraceException("FrmChannel::Synchronized", ex);
            }
            finally
            {
                this.Invoke(new EventHandler(this.EnableUI));
            }
        }


        private void EnableUI(object sender, EventArgs e)
        {
            this.MenuSynchronize.Text = "Sync";
            this.MenuApplication.Enabled = true;
            this.MenuSynchronize.Enabled = true;
            Cursor.Current = Cursors.Default;
            Application.DoEvents();
        }


        private void UpdateUI(object sender, EventArgs e)
        {
            try
            {
                lock (this)
                {                   
                    this.DownloadList.ClearList();
                    TasklistItem listItem = null;
                    FileInfo info = null;
                    string dir = _channelManager.Channel.GetDirectory();
                    string file = null;
                    
                    foreach (ChannelItem channelItem in _channelManager.Channel.Items)
                    {
                        // Channel items that do not have a title have been removed by the user.
                        if (channelItem.Title.Length > 0)
                        {
                            file = Path.Combine(dir, channelItem.Enclosure.GetFilename());
                            listItem = new TasklistItem(channelItem.Guid, channelItem.Description);
                            listItem.Total = channelItem.Enclosure.Length;

                            // Check if the user has already downloaded this file.
                            if (File.Exists(file))
                            {
                                info = new FileInfo(file);
                                listItem.Completed = info.Length;

                                //if (listItem.Total < listItem.Completed)
                                //    listItem.Total = listItem.Completed;

                            }

                            listItem.Description = this.GetProgressDescription(listItem);
                            if (listItem.Completed == listItem.Total)
                                listItem.State = TasklistItem.ItemState.Complete;
                            else if (listItem.Completed > 0)
                                listItem.State = TasklistItem.ItemState.Incomplete;

                            this.DownloadList.AddItem(listItem);
                        }
                    }

                    this.DownloadList.SelectedItem = 0;
                    this.DownloadList.Invalidate();
                }
            }
            catch (Exception ex)
            {
                OdeoApplication.TraceException("FrmChannel::UpdateUI", ex);
            }
        }


        #endregion



        private void MenuDownload_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.DownloadList.SelectedItem >= 0)
                {
                    //Get the channel item based on it's guid and kick off the download
                    string guid = this.DownloadList.SelectedTask;
                    TasklistItem tasklistItem = this.DownloadList.GetTaskListItem(guid);
                    tasklistItem.State = TasklistItem.ItemState.Failed;
                    this.DownloadList.Invalidate();

                    ChannelItem channelItem = _channelManager.Channel.GetItem(guid);
                    _downloadManager.Start(channelItem);
                }
            }
            catch (Exception ex)
            {
                OdeoApplication.TraceException("FrmChannel::MenuDownload_Click", ex);
            }
        }


        private void MenuCancel_Click(object sender, EventArgs e)
        {
            try
            {
                string itemGuid = this.DownloadList.SelectedTask;
                _downloadManager.Stop(itemGuid);
            }
            catch (Exception ex)
            {
                OdeoApplication.TraceException("MenuCancel_Click", ex);
            }
        }


        private void MenuPreferences_Click(object sender, EventArgs e)
        {
            this.ShowPreferences();
        }


        private void MenuSynchronize_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.MenuSynchronize.Text == "Sync")
                {
                    // The channel update operation is performed asynchronously.  Set the wait cursor and
                    // block the current thread.
                    this.MenuApplication.Enabled = false;
                    this.MenuSynchronize.Text = "Cancel";
                    Cursor.Current = Cursors.WaitCursor;

                    Application.DoEvents();
                    _channelManager.Synchronize();
                }
                else
                {
                    _channelManager.CancelSynchronization();
                    this.MenuSynchronize.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                OdeoApplication.TraceException("FrmChannel::MenuSynchronize_Click", ex);
            }
        }


        private void MenuCancelSychronization_Click(object sender, EventArgs e)
        {
            _channelManager.CancelSynchronization();
            Application.DoEvents();
        }


        private void MenuPlay_Click(object sender, EventArgs e)
        {
            try
            {
                string itemGuid = this.DownloadList.SelectedTask;
                ChannelItem channelItem = _channelManager.Channel.GetItem(itemGuid);
                if (channelItem != null)
                {
                    string filename = Path.Combine(channelItem.Channel.GetDirectory(), channelItem.Enclosure.GetFilename());
                    Shell sh = new Shell();
                    sh.Execute(filename);
                }
            }
            catch (Exception ex)
            {
                OdeoApplication.TraceException("FrmChannel::MenuPlay_Click", ex);
            }
        }


        private void MenuRemove_Click(object sender, EventArgs e)
        {
            try
            {
                bool success = true;
                string taskId = this.DownloadList.SelectedTask;                
                ChannelItem channelItem = _channelManager.Channel.GetItem(taskId);
                
                if (channelItem != null)
                {
                    string filename = channelItem.Enclosure.GetFilename();
                    string path = Path.Combine(channelItem.Channel.GetDirectory(), filename);
                    if (File.Exists(path))
                    {
                        string message = String.Format("Would you also like to delete the file {0}?", filename);
                        if (MessageBox.Show(message, "Confirm deletion", MessageBoxButtons.YesNo, 
                            MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.Yes )
                        {
                            try
                            {
                                File.Delete(path);
                            }
                            catch (Exception)
                            {
                                success = false;
                                MessageBox.Show(String.Format("Unable to delete file {0}.  Please close all applications using this file.", filename), 
                                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                            }
                        }                        
                    }

                    if (success)
                    {
                        channelItem.Title = String.Empty;
                        channelItem.Channel.Save();
                    }
                }

                if (success)
                {
                    TasklistItem tasklistItem = this.DownloadList.GetTaskListItem(taskId);
                    if (tasklistItem != null)
                    {
                        this.DownloadList.RemoveItem(tasklistItem);
                        this.DownloadList.Invalidate();
                    }
                }
            }
            catch (Exception ex)
            {
                OdeoApplication.TraceException("FrmChannel::MenuRemove_Click", ex);
            }
        }


        private void MenuExit_Click(object sender, EventArgs e)
        {
            OdeoApplication.Exit();
        }


        private void ShowPreferences()
        {
            FrmPreferences frmPreferences = null;
            try
            {
                Preferences preferences = OdeoApplication.Preferences;
                frmPreferences = new FrmPreferences();
                frmPreferences.BaseUrl = preferences.BaseUrl;
                frmPreferences.Username = preferences.Username;
                frmPreferences.Library = preferences.Library;

                if (frmPreferences.ShowDialog() == DialogResult.OK)
                {
                    Cursor.Current = Cursors.WaitCursor;
                    preferences.BaseUrl = frmPreferences.BaseUrl;
                    preferences.Username = frmPreferences.Username;
                    preferences.Library = frmPreferences.Library;
                    preferences.Save(Path.Combine(OdeoApplication.RootDirectory,
                        OdeoApplication.Constants.PREFERENCE_FILE));

                    _channelManager.Channel.Library = preferences.Library;
                    _channelManager.Channel.Name = preferences.Username;
                    _channelManager.Channel.Link = String.Format(preferences.BaseUrl, preferences.Username);
                    _channelManager.Channel.Load();
                    this.Invoke(new EventHandler(this.UpdateUI));
                }
            }
            catch (Exception ex)
            {
                OdeoApplication.TraceException("FrmChannel::MenuPreferences_Click", ex);
            }
            finally
            {
                Cursor.Current = Cursors.Default;
                if (frmPreferences != null)
                    frmPreferences.Dispose();
            }
        }

    }
}
