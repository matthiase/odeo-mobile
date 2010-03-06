using System;
using System.ComponentModel;
using System.Threading;
using System.Windows.Forms;

namespace Izume.Mobile.Odeo.Common
{
    public delegate void DoWorkEventHandler(object sender, DoWorkEventArgs e);
    public delegate void ProgressChangedEventHandler(object sender, ProgressChangedEventArgs e);
    public delegate void RunWorkerCompletedEventHandler(object sender, RunWorkerCompletedEventArgs e);

    public class BackgroundWorker : System.ComponentModel.Component 
    {
        private Control _parent = null;
        private bool _cancellationPending = false;
        private bool _inUse = false;
        private bool _supportCancellation = false;
        private bool _reportProgress = false;
        private RunWorkerCompletedEventArgs _runWorkerCompletedEventArgs = null;
        private ProgressChangedEventArgs _progressChangedEventArgs = null;

        #region Public Interface

        public event DoWorkEventHandler DoWork;
        public event ProgressChangedEventHandler ProgressChanged;
        public event RunWorkerCompletedEventHandler RunWorkerCompleted;

        public BackgroundWorker() : this(new Control()) { }

        public BackgroundWorker(Control parent) : base()
        {
            _parent = parent;
        }

        public bool WorkerSupportsCancellation
        {
            get { return _supportCancellation; }
            set { _supportCancellation = value; }
        }

        public bool WorkerReportsProgress
        {
            get { return _reportProgress; }
            set { _reportProgress = value; }
        }

        public bool CancellationPending
        {
            get { return _cancellationPending; }
        }

        public void RunWorkerAsync()
        {
            this.RunWorkerAsync(null);
        }

        public void RunWorkerAsync(object obj)
        {
            if (_inUse) throw 
                new InvalidOperationException("Worker already in use.");

            if (this.DoWork == null) 
                throw new InvalidOperationException("DoWork has not been subscribed to.");

            // Get the background worker ready to do it's job.
            _inUse = true;
            _cancellationPending = false;

            // Perform the work on a separate thread from the thread pool.
            ThreadPool.QueueUserWorkItem(new WaitCallback(this.RunWorker), obj);
        }

        public void CancelAsync()
        {
            if (_supportCancellation == false)
                throw new InvalidOperationException("WorkerSupportsCancellation is set to false.");

            _cancellationPending = true;
        }

        public void ReportProgress(long total, long completed, object userState)
        {
            if (_reportProgress == false)
                throw new InvalidOperationException("WorkerReportsProgress is set to false.");

            ProgressChangedEventArgs progressChangedEventArgs = new ProgressChangedEventArgs(
                total, completed, userState);

            ThreadPool.QueueUserWorkItem(new WaitCallback(this.OnProgressChanged), 
                progressChangedEventArgs);
        }

        #endregion

        #region Helper Methods

        protected virtual void OnProgressChanged(object progressChangedEventArgs)
        {
            _progressChangedEventArgs = progressChangedEventArgs as ProgressChangedEventArgs;
            _parent.Invoke(new System.EventHandler(this.NotifyParentProgress));
        }

        private void NotifyParentProgress(object sender, System.EventArgs e)
        {
            if (this.ProgressChanged != null)
                this.ProgressChanged(this, _progressChangedEventArgs);
        }

        private void RunWorker(object obj)
        {
            object result = null;
            bool cancelled = false;
            System.Exception error = null;

            try
            {
                DoWorkEventArgs eventArgs = eventArgs = new DoWorkEventArgs(obj);
                

                if (this.DoWork != null)
                {
                    this.DoWork(this, eventArgs);
                    result = eventArgs.Result;
                    cancelled = eventArgs.Cancel;
                }
            }
            catch (Exception ex)
            {
                error = ex;
            }

            RunWorkerCompletedEventArgs runWorkerCompleteEventArgs = new RunWorkerCompletedEventArgs(
                result, cancelled, error);

            ThreadPool.QueueUserWorkItem(new WaitCallback(this.OnRunWorkerCompleted), 
                runWorkerCompleteEventArgs);

            // Get the background worker ready for the next run.
            _inUse = false;
            _cancellationPending = false;

        }
        
        protected virtual void OnRunWorkerCompleted(object runWorkerCompletedEventArgs)
        {
            _runWorkerCompletedEventArgs = runWorkerCompletedEventArgs as RunWorkerCompletedEventArgs;
            _parent.Invoke(new System.EventHandler(this.NotifyParentCompleted));
        }


        private void NotifyParentCompleted(object sender, System.EventArgs e)
        {
            if (this.RunWorkerCompleted != null)
                this.RunWorkerCompleted(this, _runWorkerCompletedEventArgs);
        }


        #endregion

    }
}
