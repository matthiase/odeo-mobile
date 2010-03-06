//
// OpenFileDialog implementation for Smartphone
// Designed to follow object model of desktop framework control
// (c) 2004 Peter Foot, OpenNETCF
//
using System;
using System.ComponentModel;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Text;

namespace Izume.Mobile.Odeo.Common
{
	/// <summary>
	/// Represents a common dialog box that displays the control that allows the user to open a file, specifically implemented for Smartphone devices.
	/// </summary>
	public class OpenFileDialog : CommonDialog
	{
		private const int MaxPath = 255;

		private FileListDialog m_fl;

		/// <summary>
		/// Initializes a new instance of OpenFileDialog
		/// </summary>
		public OpenFileDialog()
		{
			m_fl = new FileListDialog();
			//set startup in users My Documents folder
			m_fl.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
			m_fl.Filter = "All Files|*.*";
			m_fl.FilterIndex = 1;
		}

		/// <summary>
		/// Runs the common dialog box.
		/// </summary>
		/// <returns></returns>
		public new DialogResult ShowDialog()
		{
			//show the file list
			return m_fl.ShowDialog();
		}

		/// <summary>
		/// Resets all properties to their default values.
		/// </summary>
		public void Reset()
		{
			m_fl.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
			m_fl.Filter = "All Files|*.*";
			m_fl.FilterIndex = 1;
			m_fl.RefreshList();
		}

		/// <summary>
		/// Gets or sets a string containing the file name selected in the file dialog box.
		/// </summary>
		public string FileName
		{
			get
			{
				return m_fl.FileName;
			}
		}

		/// <summary>
		/// Gets or sets the current file name filter string, which determines what type of files will be displayed.
		/// </summary>
		public string Filter
		{
			get
			{
				return m_fl.Filter;
			}
			set
			{
				m_fl.Filter = value;
			}
		}

		/// <summary>
		///  Gets or sets the index of the filter currently selected in the file dialog box.
		/// </summary>
		public int FilterIndex
		{
			get
			{
				return m_fl.FilterIndex;
			}
			set
			{
				m_fl.FilterIndex = value;
			}
		}

		/// <summary>
		/// Gets or sets the initial directory displayed by the file dialog box.
		/// </summary>
		public string InitialDirectory
		{
			get
			{
				return m_fl.InitialDirectory;
			}
			set
			{
				m_fl.InitialDirectory = value;
			}
		}

		/// <summary>
		/// Gets or sets the file dialog box title.
		/// </summary>
		public string Title
		{
			get
			{
				return m_fl.Text;
			}
			set
			{
				m_fl.Text = value;
			}
		}
	}
}
