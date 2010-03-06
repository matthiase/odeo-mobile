using System;
using System.Drawing;
using System.Windows.Forms;

using Izume.Mobile.Odeo;
using Izume.Mobile.Odeo.Common;

namespace Izume.Mobile.Odeo.Smartphone
{
    /// <summary>
    /// Summary description for FrmPreferences.
    /// </summary>
    public class FrmPreferences : System.Windows.Forms.Form
    {
        private MenuItem MenuSave;
        private MenuItem MenuApplication;
        private Label lblUrl;
        private TextBox txtUsername;
        private Label lblUsername;
        private TextBox txtUrl;
        private TextBox txtLibrary;
        private Label lblLibrary;
        private MenuItem MenuBrowse;
        private MenuItem MenuCancel;

        /// <summary>
        /// Main menu for the form.
        /// </summary>
        private System.Windows.Forms.MainMenu MainMenu;

        public FrmPreferences()
        {
            InitializeComponent();
            Common.GuiHelper.SetStyle(this);
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            this.txtUrl.Text = OdeoApplication.Constants.DEFAULT_URL;
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.MainMenu = new System.Windows.Forms.MainMenu();
            this.MenuSave = new System.Windows.Forms.MenuItem();
            this.MenuApplication = new System.Windows.Forms.MenuItem();
            this.MenuBrowse = new System.Windows.Forms.MenuItem();
            this.MenuCancel = new System.Windows.Forms.MenuItem();
            this.lblUrl = new System.Windows.Forms.Label();
            this.txtUsername = new System.Windows.Forms.TextBox();
            this.lblUsername = new System.Windows.Forms.Label();
            this.txtUrl = new System.Windows.Forms.TextBox();
            this.txtLibrary = new System.Windows.Forms.TextBox();
            this.lblLibrary = new System.Windows.Forms.Label();
            // 
            // MainMenu
            // 
            this.MainMenu.MenuItems.Add(this.MenuSave);
            this.MainMenu.MenuItems.Add(this.MenuApplication);
            // 
            // MenuSave
            // 
            this.MenuSave.Text = "Save";
            this.MenuSave.Click += new System.EventHandler(this.MenuSave_Click);
            // 
            // MenuApplication
            // 
            this.MenuApplication.MenuItems.Add(this.MenuBrowse);
            this.MenuApplication.MenuItems.Add(this.MenuCancel);
            this.MenuApplication.Text = "Menu";
            // 
            // MenuBrowse
            // 
            this.MenuBrowse.Enabled = false;
            this.MenuBrowse.Text = "Browse";
            this.MenuBrowse.Click += new System.EventHandler(this.MenuBrowse_Click);
            // 
            // MenuCancel
            // 
            this.MenuCancel.Text = "Cancel";
            this.MenuCancel.Click += new System.EventHandler(this.MenuCancel_Click);
            // 
            // lblUrl
            // 
            this.lblUrl.Font = new System.Drawing.Font("Nina", 9F, System.Drawing.FontStyle.Bold);
            this.lblUrl.Location = new System.Drawing.Point(4, 46);
            this.lblUrl.Size = new System.Drawing.Size(160, 14);
            this.lblUrl.Text = "Feed Url:";
            // 
            // txtUsername
            // 
            this.txtUsername.Font = new System.Drawing.Font("Nina", 9F, System.Drawing.FontStyle.Regular);
            this.txtUsername.Location = new System.Drawing.Point(4, 20);
            this.txtUsername.Size = new System.Drawing.Size(170, 21);
            // 
            // lblUsername
            // 
            this.lblUsername.Font = new System.Drawing.Font("Nina", 9F, System.Drawing.FontStyle.Bold);
            this.lblUsername.Location = new System.Drawing.Point(4, 4);
            this.lblUsername.Size = new System.Drawing.Size(165, 14);
            this.lblUsername.Text = "Username:";
            // 
            // txtUrl
            // 
            this.txtUrl.Font = new System.Drawing.Font("Nina", 9F, System.Drawing.FontStyle.Regular);
            this.txtUrl.Location = new System.Drawing.Point(4, 62);
            this.txtUrl.Size = new System.Drawing.Size(170, 21);
            // 
            // txtLibrary
            // 
            this.txtLibrary.Font = new System.Drawing.Font("Nina", 9F, System.Drawing.FontStyle.Regular);
            this.txtLibrary.Location = new System.Drawing.Point(3, 104);
            this.txtLibrary.Size = new System.Drawing.Size(170, 21);
            this.txtLibrary.LostFocus += new System.EventHandler(this.txtLibrary_LostFocus);
            this.txtLibrary.GotFocus += new System.EventHandler(this.txtLibrary_GotFocus);
            // 
            // lblLibrary
            // 
            this.lblLibrary.Font = new System.Drawing.Font("Nina", 9F, System.Drawing.FontStyle.Bold);
            this.lblLibrary.Location = new System.Drawing.Point(3, 88);
            this.lblLibrary.Size = new System.Drawing.Size(160, 14);
            this.lblLibrary.Text = "Library Location:";
            // 
            // FrmPreferences
            // 
            this.ClientSize = new System.Drawing.Size(176, 180);
            this.Controls.Add(this.lblUrl);
            this.Controls.Add(this.lblUsername);
            this.Controls.Add(this.txtUsername);
            this.Controls.Add(this.txtUrl);
            this.Controls.Add(this.lblLibrary);
            this.Controls.Add(this.txtLibrary);
            this.Font = new System.Drawing.Font("Nina", 9F, System.Drawing.FontStyle.Bold);
            this.Menu = this.MainMenu;
            this.Text = "Syncr Preferences";

        }

        #endregion


        public string Username
        {
            get { return this.txtUsername.Text; }
            set { this.txtUsername.Text = value; }
        }


        public string BaseUrl
        {
            get { return this.txtUrl.Text; }
            set
            { 
                if (value.Length > 0)
                    this.txtUrl.Text = value; 
            }
        }

        public string Library
        {
            get { return this.txtLibrary.Text; }
            set
            {
                if (value.Length > 0)
                    this.txtLibrary.Text = value;
            }
        }

        private void MenuSave_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }


        private void txtLibrary_GotFocus(object sender, EventArgs e)
        {
            this.MenuBrowse.Enabled = true;
        }

        private void txtLibrary_LostFocus(object sender, EventArgs e)
        {
            this.MenuBrowse.Enabled = false;
        }

        private void MenuBrowse_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            if (dialog.ShowDialog() == DialogResult.OK)
                this.txtLibrary.Text = dialog.SelectedPath;

            dialog.Dispose();
            dialog = null;                
        }

        private void MenuCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }


        //private Control GetFocusedControl()
        //{
        //    Control result = null;
        //    foreach (Control control in this.Controls)
        //    {
        //        if (control.Focused)
        //            result = control;
        //    }
        //    return result;
        //}

    }
}
