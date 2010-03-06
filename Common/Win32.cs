using System;
using System.Runtime.InteropServices;


namespace Izume.Mobile.Odeo.Common
{
    public class Win32
    {
        public static readonly int WM_NULL = 0x0000;
        public static readonly int WM_CREATE = 0x0001;
        public static readonly int WM_DESTROY = 0x0002;
        public static readonly int WM_MOVE = 0x0003;
        public static readonly int WM_SIZE = 0x0005;
        public static readonly int WM_ACTIVATE = 0x0006;
        public static readonly int WM_SETFOCUS = 0x0007;
        public static readonly int WM_KILLFOCUS = 0x0008;
        public static readonly int WM_ENABLE = 0x000A;
        public static readonly int WM_SETREDRAW = 0x000B;
        public static readonly int WM_SETTEXT = 0x000C;
        public static readonly int WM_GETTEXT = 0x000D;
        public static readonly int WM_GETTEXTLENGTH = 0x000E;
        public static readonly int WM_PAINT = 0x000F;
        public static readonly int WM_CLOSE = 0x0010;
        public static readonly int WM_QUERYENDSESSION = 0x0011;
        public static readonly int WM_QUIT = 0x0012;
        public static readonly int WM_QUERYOPEN = 0x0013;
        public static readonly int WM_ERASEBKGND = 0x0014;

        public static readonly int LVM_FIRST = 0x1000;
        public static readonly int LVM_GETITEMRECT = (LVM_FIRST + 14);
        public static readonly int LVM_HITTEST = (LVM_FIRST + 18);
        public static readonly int LVM_GETSUBITEMRECT = (LVM_FIRST + 56);
        public static readonly int LVM_SUBITEMHITTEST = (LVM_FIRST + 57);

        public static readonly int LVIR_BOUNDS = 0x0000;
        public static readonly int LVIR_ICON = 0x0001;
        public static readonly int LVIR_LABEL = 0x0002;

        public static readonly int LVHT_NOWHERE = 0x0001;
        public static readonly int LVHT_ONITEMICON = 0x0002;
        public static readonly int LVHT_ONITEMLABEL = 0x0004;
        public static readonly int LVHT_ONITEMSTATEICON = 0x0008;
        public static readonly int LVHT_ONITEM = (LVHT_ONITEMICON | LVHT_ONITEMLABEL | LVHT_ONITEMSTATEICON);

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }

        //[DllImport("coredll.dll", SetLastError = true)]
        //public static extern bool ValidateRect(IntPtr hwnd, ref RECT rect);

        [DllImport("coredll.dll")]
        public static extern IntPtr GetCapture(); 

        [DllImport("coredll.dll", SetLastError = true)]
        public static extern int SendMessage(IntPtr hwnd, int msg, int wParam, ref RECT rect);

    }
}
