#region Using directives

using System;

#endregion

namespace Izume.Mobile.Odeo.Common
{
    /// <summary>
    /// Summary description for XmlHelper.
    /// </summary>
    public class XmlHelper
    {
        private static string[] _hdf5 = new string[] {"\"", "'", "&", "<", ">" };
        private static string[] _escape = new string[] { "&quot;", "&apos;", "&amp;", "&lt;", "&gt" };

        private XmlHelper()
        {

        }


        public static string Encode(string text)
        {
           string result = text;
           for (int i = 0; i < _hdf5.Length; i++)
               result = result.Replace(_hdf5[i], _escape[i]);

           return result;
        }


        public static string Decode(string xml)
        {
            string result = xml;
            for (int i = 0; i < _escape.Length; i++)
                result = result.Replace(_escape[i], _hdf5[i]);

            return result;
        }


    }
}
