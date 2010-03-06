using System;
using System.Drawing;
using System.Collections;
using System.Windows.Forms;
using System.Data;
using System.IO;

using System.Reflection;
using System.Threading;

using Izume.Mobile.Odeo.Common;

namespace Izume.Mobile.Odeo.Smartphone
{
    public delegate void ProgressStepHandler();

    public class FrmStartup : System.Windows.Forms.Form
    {
        private Panel StartupPanel;
        private ProgressBar ProgressMeter;
        private PictureBox OdeoLogo;
    
        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.StartupPanel = new System.Windows.Forms.Panel();
            this.OdeoLogo = new System.Windows.Forms.PictureBox();
            this.ProgressMeter = new System.Windows.Forms.ProgressBar();
            // 
            // StartupPanel
            // 
            this.StartupPanel.Controls.Add(this.ProgressMeter);
            this.StartupPanel.Controls.Add(this.OdeoLogo);
            this.StartupPanel.Location = new System.Drawing.Point(0, 3);
            this.StartupPanel.Size = new System.Drawing.Size(176, 194);
            // 
            // OdeoLogo
            // 
            this.OdeoLogo.Location = new System.Drawing.Point(42, 49);
            this.OdeoLogo.Size = new System.Drawing.Size(92, 39);
            // 
            // ProgressMeter
            // 
            this.ProgressMeter.Location = new System.Drawing.Point(42, 94);
            this.ProgressMeter.Size = new System.Drawing.Size(92, 6);
            // 
            // FrmStartup
            // 
            this.ClientSize = new System.Drawing.Size(176, 200);
            this.Controls.Add(this.StartupPanel);
            this.Text = "Odeo Syncr";

        }

        #endregion
        
        public FrmStartup()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(OdeoApplication.Constants.STARTUP_LOGO))
            {
                if (stream != null)
                    this.OdeoLogo.Image = new Bitmap(stream);
            }

            base.OnLoad(e);
        }


        public int PercentComplete
        {
            get { return this.ProgressMeter.Value; }
            set
            {
                if (value > this.ProgressMeter.Maximum)
                    value = this.ProgressMeter.Maximum;

                this.ProgressMeter.Value = value;
            }
        }


        protected override void Dispose(bool disposing)
        {
            if (this.OdeoLogo.Image != null)
                this.OdeoLogo.Image.Dispose();

            base.Dispose(disposing);
        }

    }
}
