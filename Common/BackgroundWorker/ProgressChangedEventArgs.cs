using System;

namespace Izume.Mobile.Odeo.Common
{
    public class ProgressChangedEventArgs : System.EventArgs 
    {
        private long _total = 0;
        private long _completed = 0;
        private object _userState = null;

        public ProgressChangedEventArgs(long total, long completed, object userState)
        {
            _total = total;
            _completed = completed;
            _userState = userState;
        }



        public long Total
        {
            get { return _total; }
        }

        public long Completed
        {
            get { return _completed; }
        }

        public object UserState
        {
            get { return _userState; }
        }
    }
}
