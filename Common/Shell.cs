using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Izume.Mobile.Odeo.Common
{
	public class Shell
	{
		[DllImport("coredll.dll", SetLastError=true)]
		extern static IntPtr LocalAlloc( int flags, int size );

		[DllImport("coredll")]
		extern static void LocalFree( IntPtr ptr );		

		[DllImport("coredll")]
		extern static int ShellExecuteEx( ref ShellExecuteInfo lpExecInfo );

		[DllImport("CoreDll.dll")]
		private extern static Int32 GetLastError();
		
		public ShellExecuteInfo lpExecInfo;

		public Shell()
		{
			this.lpExecInfo = new ShellExecuteInfo();
		}

		public void Execute(string filename)
		{
			int nSize = filename.Length * 2 + 2;
			IntPtr pData = LocalAlloc(0x40, nSize);
			Marshal.Copy(Encoding.Unicode.GetBytes(filename), 0, pData, nSize - 2);

			this.lpExecInfo.cbSize = 60;
			this.lpExecInfo.dwHotKey = 0;
			this.lpExecInfo.fMask = 0;
			this.lpExecInfo.hIcon = IntPtr.Zero;
			this.lpExecInfo.hInstApp = IntPtr.Zero;
			this.lpExecInfo.hProcess = IntPtr.Zero;;
			this.lpExecInfo.lpClass = IntPtr.Zero;
			this.lpExecInfo.lpDirectory = IntPtr.Zero;
			this.lpExecInfo.lpIDList = IntPtr.Zero;
			this.lpExecInfo.lpParameters = IntPtr.Zero;
			this.lpExecInfo.lpVerb = IntPtr.Zero;
			this.lpExecInfo.nShow = 0;
			this.lpExecInfo.lpFile = pData;
			int result = ShellExecuteEx(ref this.lpExecInfo);
		}
	}


	public struct ShellExecuteInfo
	{
		public UInt32 cbSize; 
		public UInt32 fMask; 
		public IntPtr hwnd; 
		public IntPtr lpVerb; 
		public IntPtr lpFile; 
		public IntPtr lpParameters; 
		public IntPtr lpDirectory; 
	
		public int nShow; 
		public IntPtr hInstApp; 
		// Optional members 
		public IntPtr lpIDList; 
		public IntPtr lpClass; 
		public IntPtr hkeyClass; 
		public UInt32 dwHotKey; 
		public IntPtr hIcon; 
		public IntPtr hProcess; 
	
	}	
}


