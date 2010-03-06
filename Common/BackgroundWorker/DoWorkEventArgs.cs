using System;

namespace Izume.Mobile.Odeo.Common
{
    public class DoWorkEventArgs : System.ComponentModel.CancelEventArgs 
    {
        private object _argument = null;
        private object _result = null;

        public DoWorkEventArgs(object argument)
        {
            _argument = argument;
        }

        public object Argument
        {
            get { return _argument; }
        }

        public object Result
        {
            get { return _result; }
            set { _result = value; }
        }
    }
}
