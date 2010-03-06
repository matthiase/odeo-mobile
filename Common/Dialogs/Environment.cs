//==========================================================================================
//
//		OpenNETCF.Environment
//		Copyright (c) 2003, OpenNETCF.org
//
//		This library is free software; you can redistribute it and/or modify it under 
//		the terms of the OpenNETCF.org Shared Source License.
//
//		This library is distributed in the hope that it will be useful, but 
//		WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or 
//		FITNESS FOR A PARTICULAR PURPOSE. See the OpenNETCF.org Shared Source License 
//		for more details.
//
//		You should have received a copy of the OpenNETCF.org Shared Source License 
//		along with this library; if not, email licensing@opennetcf.org to request a copy.
//
//		If you wish to contact the OpenNETCF Advisory Board to discuss licensing, please 
//		email licensing@opennetcf.org.
//
//		For general enquiries, email enquiries@opennetcf.org or visit our website at:
//		http://www.opennetcf.org
//
//==========================================================================================
using System;
using System.Text;
using System.Runtime.InteropServices;

namespace Izume.Mobile.Odeo.Common
{
	/// <summary>
	/// Extends the functionality of <see cref="T:System.Environment"/>
	/// </summary>
	/// <seealso cref="T:System.Environment">System.Environment Class</seealso>
	public class Environment
	{
		//maximum supported length of a file path
		private const int MaxPath = 260;
		
		private Environment(){}

		#region New Line
		/// <summary>
		/// Gets the newline string defined for this environment.
		/// </summary>
		/// <value>A string containing "\r\n".</value>
		/// <remarks>The property value is a constant customized specifically for the current platform.
		/// This value is automatically appended to text when using WriteLine methods, such as <see cref="M:T:System.Console.WriteLine(System.String)">Console.WriteLine</see>.</remarks>
		/// <seealso cref="P:System.Environment.NewLine">System.Environment.NewLine Property</seealso>
		public static string NewLine
		{
			get
			{
				return "\r\n";
			}
		}
		#endregion

		#region System Directory
		/// <summary>
		/// Gets the fully qualified path of the system directory.
		/// </summary>
		/// <value>A string containing a directory path.</value>
		/// <remarks>An example of the value returned is the string "\Windows".</remarks>
		/// <seealso cref="P:System.Environment.SystemDirectory">System.Environment.SystemDirectory Property</seealso>
		public static string SystemDirectory
		{
			get
			{
				return "\\Windows";
			}
		}
		#endregion

		#region Get Folder Path
		/// <summary>
		/// Gets the path to the system special folder identified by the specified enumeration.
		/// </summary>
		/// <param name="folder">An enumerated constant that identifies a system special folder.</param>
		/// <returns>The path to the specified system special folder, if that folder physically exists on your computer; otherwise, the empty string ("").  A folder will not physically exist if the operating system did not create it, the existing folder was deleted, or the folder is a virtual directory, such as My Computer, which does not correspond to a physical path.</returns>
		/// <seealso cref="M:System.Environment.GetFolderPath(System.Environment.SpecialFolder)">System.Environment.GetFolderPath Method</seealso>
		public static string GetFolderPath(SpecialFolder folder)
		{
			StringBuilder path = new StringBuilder(MaxPath);

			if(! Convert.ToBoolean(SHGetSpecialFolderPath(IntPtr.Zero, path, (int)folder, 0)))
			{
				throw new Exception("Cannot get folder path!");
			}

			return path.ToString();
		}

		[DllImport("coredll", EntryPoint="SHGetSpecialFolderPath", SetLastError=false)]
		internal static extern int SHGetSpecialFolderPath(IntPtr hwndOwner, StringBuilder lpszPath, int nFolder, int fCreate);

		#endregion

		#region Special Folder
		/// <summary>
		/// Specifies enumerated constants used to retrieve directory paths to system special folders.
		/// </summary>
		/// <remarks>Not all platforms support all of these constants.</remarks>
		/// <seealso cref="T:System.Environment.SpecialFolder">System.Environment.SpecialFolder Enumeration</seealso>
		public enum SpecialFolder
		{
			/// <summary>
			/// "\"
			/// </summary>
			VirtualRoot		= 0x00,
			/// <summary>
			/// The directory that contains the user's program groups.
			/// </summary>
			Programs		= 0x02,       
			/// <summary>
			/// control panel icons
			/// </summary>
			Controls		= 0x03,
			/// <summary>
			/// printers folder
			/// </summary>
			Printers		= 0x04,
			/// <summary>
			/// The directory that serves as a common repository for documents. (Not supported in Pocket PC and Pocket PC 2002 - "\My Documents").
			/// </summary>
			Personal		= 0x05,
			/// <summary>
			/// The directory that serves as a common repository for the user's favorite items.
			/// </summary>
			Favorites		= 0x06,	
			/// <summary>
			/// The directory that corresponds to the user's Startup program group.   The system starts these programs whenever a user starts Windows CE.
			/// </summary>
			Startup			= 0x07,
			/// <summary>
			/// The directory that contains the user's most recently used documents.
			/// </summary>
			Recent			= 0x08,	
			/// <summary>
			/// The directory that contains the Send To menu items.
			/// </summary>
			SendTo			= 0x09,
			/// <summary>
			/// Recycle bin.
			/// </summary>
			RecycleBin		= 0x0A,
			/// <summary>
			/// The directory that contains the Start menu items.
			/// </summary>
			StartMenu		= 0x0B,     
			/// <summary>
			/// The directory used to physically store file objects on the desktop.   Do not confuse this directory with the desktop folder itself, which is a virtual folder.
			/// </summary>
			DesktopDirectory = 0x10,
			/// <summary>
			/// The "My Computer" folder.
			/// </summary>
			MyComputer		= 0x11,
			/// <summary>
			/// Network Neighbourhood
			/// </summary>
			NetNeighborhood = 0x12,
			/// <summary>
			/// "\Windows\Fonts"
			/// </summary>
			Fonts			= 0x14
		}
		#endregion

	}
}
