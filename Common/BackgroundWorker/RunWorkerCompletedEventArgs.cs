using System;

namespace Izume.Mobile.Odeo.Common
{
    public class RunWorkerCompletedEventArgs : System.EventArgs
    {
        private object _result = null;
        private Boolean _cancelled = false;
        private Exception _error = null;

        public RunWorkerCompletedEventArgs(object result, bool cancelled, Exception error)
        {
            _result = result;
            _cancelled = cancelled;
            _error = error;
        }

        public object Result
        {
            get { return _result; }
        }

        public bool Cancelled
        {
            get { return _cancelled; }
        }

        public Exception Error
        {
            get { return _error; }
        }
    }
}
