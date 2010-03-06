/*
	The .NET Compact Framework does not support owner drawn listboxes. 
	This is a custom list control that display information in a PocketPC-friendly 
	way. Stores list of tasklist objects and displays each item on multiple 
	lines. Responsible for all drawing, selection highlight, scrollbar, etc.

	Draws priority icons. Uses transparency to draw icons since background 
	color changes depending if the task is selected or not.	

	Parent is notified through Click event. This control does not do anything 
	special, uses derived Control implementation.
	
	Uses ellipsis (...) at end of strings if they are too long.	Draws to memory 
	bitmap to avoid flashing. 
*/

using System;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Imaging;
using System.Collections;
using System.Reflection;

namespace Izume.Mobile.Odeo.Common
{
    /// <summary>
    /// Custom list control that displays list of tasks in a Pocket-PC way.
    /// </summary>
    public class Tasklist : Control
    {
        class Const
        {
            public static Color TitleColor = Constants.Colors.HotPink;
            public static Color CaptionColor = Constants.Colors.DarkBlue;           
            public static Color BackColor = Color.White;
            public static Color SelBackColor = Constants.Colors.LightBlue;
            public static Color SelTitleColor = TitleColor;
            public static Color SelCaptionColor = CaptionColor;
            public static Color ClosedTitleColor = Color.Gray;
            public static Color ClosedCaptionColor = CaptionColor;
            public static Color SepLine = Constants.Colors.MediumBlue;
            public static Color ProgressColor = Color.Lime;
            public static Color ProgressCompleteColor = Constants.Colors.MediumBlue;
            public static Color ProgressFrameColor = Constants.Colors.MediumBlue;
            public const int IconSize = 13;

            public const int NameTextLimit = 30;
            public const int ByteProgressTextLimit = 20;
            public const int ColTask = 15;
            public const int ColUserName = 20;
            public const int ColDate = 120; //155
            public const int ScrollBarWidth = 8;
            public const int ProgressHeight = 8;
            public const int ProgressWidth = 40;

            public const string StatusOpen = "Open";
        }

        // internal fields
        ArrayList m_list;
        int m_topItem;
        int m_visibleCount;
        int m_itemHeight;
        int m_textHeight;
        int m_selItem = -1;

        // scrollbar
        VScrollBar m_scrollBar;
        int m_scrollValue;
        bool m_scrollDirty = true;

        // gdi objects
        Bitmap m_bmp;
        Bitmap[] m_icons;

        ImageAttributes m_iconAttributes;
        Font m_fontSummary, m_fontDetails;
        Pen m_penSep, m_penProgress;
        SolidBrush m_brushTitle, m_brushClosedTitle, m_brushSelTitle, m_brushCaption,
            m_brushClosedCaption, m_brushSelCaption,m_brushSelBack, m_brushProgress, 
            m_brushProgressComplete;


        //
        // properties
        //


        /// <summary>
        /// Return ID of task that is selected.
        /// </summary>
        public string SelectedTask
        {
            get
            {
                if (m_selItem == -1)
                    return String.Empty;

                TasklistItem item = m_list[m_selItem] as TasklistItem;
                return item.TaskId;
            }
        }

        public TasklistItem GetTaskListItem(string taskID)
        {
            TasklistItem ret = null;

            foreach (TasklistItem item in m_list)
            {
                if (String.Compare(item.TaskId, taskID) == 0)
                {
                    ret = item;
                    break;

                }
            }
            return ret;
        }

        /// <summary>
        /// Index of selected item.
        /// </summary>
        public int SelectedItem
        {
            get { return m_selItem; }
            set
            {
                if (value >= 0 && value < m_list.Count && value != m_selItem)
                {
                    m_selItem = value;
                    Invalidate();
                }
            }
        }

        static public Bitmap LoadImage(string imageName)
        {
            return new Bitmap(System.Reflection.Assembly.GetExecutingAssembly().
                GetManifestResourceStream("Izume.Mobile.Odeo.Common.Resource." + imageName));
        }


        // ctor		
        public Tasklist()
        {
            // init colors
            this.BackColor = Const.BackColor;
            this.ForeColor = Const.CaptionColor;

            // gdi objects
            CreateGdiObjects();

            // state icons
            m_icons = new Bitmap[5];
            m_icons[0] = LoadImage("new.bmp");
            m_icons[1] = LoadImage("inProcess.bmp");
            m_icons[2] = LoadImage("complete.bmp");
            m_icons[3] = LoadImage("fail.bmp");
            m_icons[4] = LoadImage("cancelled.bmp");

            // need to draw transparent
            m_iconAttributes = new ImageAttributes();
            m_iconAttributes.SetColorKey(Color.Yellow, Color.Yellow);

            // store list of task items			
            m_list = new ArrayList();

            // scroll bar
            m_scrollBar = new VScrollBar();
            m_scrollBar.Bounds = new Rectangle(0, 0, 4, 100);
            m_scrollBar.ValueChanged += new System.EventHandler(this.scrollBar_ValueChanged);
            this.Controls.Add(m_scrollBar);
        }




        //
        // public methods
        //

        /// <summary>
        /// Add task item to list.
        /// </summary>
        public void AddItem(TasklistItem item)
        {
            // add item to list and mark as dirty, 
            // will upate next time it needs to draw list
            m_list.Add(item);
            m_scrollDirty = true;
        }


        public void RemoveItem(TasklistItem item)
        {
            m_list.Remove(item);
            m_scrollDirty = true;
        }

        /// <summary>
        /// Clear task list.
        /// </summary>
        public void ClearList()
        {
            m_list.Clear();
            m_selItem = -1;
            m_scrollDirty = true;
            Invalidate();
        }

        /// <summary>
        /// Reset scroll position to top. Remove any selected item.
        /// </summary>
        public void ResetScrollPos()
        {
            m_scrollValue = 0;
            m_selItem = -1;
            m_scrollDirty = true;
            Invalidate();
        }


        //
        // overrides / events
        //

        protected override void OnPaint(PaintEventArgs e)
        {
            // draw on memory bitmap
            CreateMemoryBitmap();

            // calculate fields required to layout and draw list
            RecalcItems(e.Graphics);

            // draw list
            Graphics g = Graphics.FromImage(m_bmp);
            DrawItems(g);

            // blit memory bitmap to screen
            e.Graphics.DrawImage(m_bmp, 0, 0);

            //			g.Dispose();
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            // don't pass to base since we paint everything, avoid flashing
        }

        public void EnsureVisible(int index)
        {
            if (index < this.m_scrollBar.Value)
            {
                this.m_scrollBar.Value = index;
            }
            else if (index >= this.m_scrollBar.Value + this.DrawCount)
            {
                this.m_scrollBar.Value = index - this.DrawCount + 1;
            }
        }

        protected int DrawCount
        {
            get
            {
                if (this.m_scrollBar.Value + this.m_scrollBar.LargeChange > this.m_scrollBar.Maximum)
                    return this.m_scrollBar.Maximum - this.m_scrollBar.Value + 1;
                else
                    return this.m_scrollBar.LargeChange;
            }
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            // determine what item was selected
            switch (e.KeyCode)
            {
                case Keys.Down:
                    if (m_selItem < this.m_list.Count - 1)
                    {
                        m_selItem++;
                    }
                    break;
                case Keys.Up:
                    if (m_selItem > 0)
                    {
                        m_selItem--;
                    }
                    break;
            }
            EnsureVisible(m_selItem);
            Invalidate();
            Update();
        }

        // scrollbar event
        private void scrollBar_ValueChanged(object sender, System.EventArgs e)
        {
            // save new scoll value and redraw list
            m_scrollValue = m_scrollBar.Value;
            Invalidate();
        }


        //
        // private helper methods
        //

        // calculate info required to layout items
        private void RecalcItems(Graphics g)
        {
            // calculate and store info that is required to 
            // layout and draw list items
            m_textHeight = g.MeasureString("W", m_fontSummary).ToSize().Height;
            m_itemHeight = (m_textHeight * 2) + 5;
            m_topItem = m_scrollValue;
            m_visibleCount = (this.Height + m_itemHeight - 1) / m_itemHeight;

            // see if scrollbar needs to be updated
            if (m_scrollDirty)
                RecalcScrollBar();
        }

        // calculate and update scrollbar values
        private void RecalcScrollBar()
        {
            // update scrollbar 
            m_scrollBar.Maximum = m_list.Count;
            m_scrollBar.LargeChange = Math.Max(m_visibleCount - 1, 1);
            m_scrollBar.Enabled = (m_visibleCount < m_list.Count) ? true : false;
            m_scrollBar.Value = m_scrollValue;

            // clear flag, scrollbar has been recalclated
            m_scrollDirty = false;
        }

        // function that does all of the drawing
        // loops through list of visible items and draws each item
        private void DrawItems(Graphics g)
        {
            // background
            g.Clear(this.BackColor);

            // calculate drawing area
            Rectangle rc = new Rectangle(0, 0, this.Width - Const.ScrollBarWidth, this.Height - 1);

            SolidBrush brushTitle;
            SolidBrush brushCaption;

            // draw items that are visible
            int curPos = 0;
            int curItem = 0;
            for (int i = 0; i < m_visibleCount; i++)
            {
                // get next visible item in list
                curItem = i + m_topItem;
                if (curItem < m_list.Count)
                {
                    // determine if this item is selected, draw selected background
                    bool selected = (m_selItem == curItem) ? true : false;
                    if (selected)
                        g.FillRectangle(m_brushSelBack, 0, curPos, rc.Width, m_itemHeight);

                    // get details of item						
                    TasklistItem item = m_list[curItem] as TasklistItem;

                    // Get state...
                    bool newInstance = (item.State == TasklistItem.ItemState.New);
                    bool complete = (item.State == TasklistItem.ItemState.Complete);
                    //bool deleted = (item.State == TasklistItem.ItemState.Deleted);
                    bool stopped = (item.State == TasklistItem.ItemState.Cancelled);
                    bool failed = (item.State == TasklistItem.ItemState.Failed);
                    bool inProcess = (item.State == TasklistItem.ItemState.Incomplete);

                    // determine if this items is complete or selected.
                    if (complete)
                    {
                        brushTitle = m_brushClosedTitle;
                        brushCaption = m_brushClosedCaption;
                    }
                    else if (selected)
                    {
                        brushTitle = m_brushSelTitle;
                        brushCaption = m_brushSelCaption;
                    }
                    else
                    {
                        brushTitle = m_brushTitle;
                        brushCaption = m_brushCaption;
                    }

                    // draw summary
                    string name = item.Name;
                    if (name.Length > Const.NameTextLimit)
                        name = name.Substring(0, Const.NameTextLimit) + "...";

                    g.DrawString(name, m_fontSummary, brushTitle, Const.ColTask, curPos + 2);

                    // draw item title
                    string description = item.Description;
                    if (description.Length > Const.ByteProgressTextLimit)
                        description = description.Substring(0, Const.ByteProgressTextLimit) + "...";

                    int secondLinePos = curPos + m_textHeight + 3;

                    // KMC
                    //					g.DrawString(item.TaskId.ToString(), m_fontDetails, 
                    //						selected ? m_brushSelText : brushText, Const.ColTask, secondLinePos);

                    // draw item caption
                    g.DrawString(description, m_fontDetails, brushCaption, Const.ColUserName, secondLinePos);


                    // draw progress
                    DrawProgress(g, item.PercentComplete, !complete, Const.ColDate, secondLinePos);

                    Image image = null;
                    if (newInstance)
                        image = m_icons[0];
                    else if (inProcess)
                        image = m_icons[1];
                    else if (complete)
                        image = m_icons[2];
                    else if (failed)
                        image = m_icons[3];
                    else
                        image = m_icons[4];

                    g.DrawImage(image,
                        new Rectangle(0, curPos + 2, Const.IconSize, Const.IconSize),
                        0, 0, Const.IconSize, Const.IconSize,
                        GraphicsUnit.Pixel, m_iconAttributes);

                    // draw separator line
                    curPos += m_itemHeight;
                    if (curPos < this.Height)
                        g.DrawLine(m_penSep, 0, curPos, rc.Width, curPos);
                }
            }

            // frame around list
            g.DrawRectangle(new Pen(Color.Black), rc);
        }

        // draws a small progress bar
        private void DrawProgress(Graphics g, int progress, bool enabled, int x, int y)
        {
            // bar
            g.FillRectangle(enabled ? m_brushProgress : m_brushProgressComplete,
                x, y + 2, (progress * Const.ProgressWidth) / 100, Const.ProgressHeight);

            // bar frame
            g.DrawRectangle(m_penProgress, x, y + 2,
                Const.ProgressWidth, Const.ProgressHeight);
        }


        // determine if what item is under x / y position
        // return -1 if no items
        private int HitTest(int x, int y)
        {
            // loop through visible items
            int itemPos = 0;
            int curItem = 0;
            for (int i = 0; i < m_visibleCount; i++)
            {
                // went past list, no items are in hit area
                curItem = i + m_topItem;
                if (curItem >= m_list.Count)
                    return -1;

                // found item in hit area					
                if (y > itemPos && y < (itemPos + m_itemHeight))
                    return curItem;

                // move to next item
                itemPos += m_itemHeight;

                // moved outside of list control, no items are in hit area
                if (itemPos > this.Height)
                    return -1;
            }

            // did not find any items in hit area
            return -1;
        }

        private void CreateGdiObjects()
        {
            m_fontSummary = Constants.Fonts.SmallEmphasis;
            m_fontDetails = Constants.Fonts.Small;
            m_brushTitle = new SolidBrush(Const.TitleColor);
            m_brushClosedTitle = new SolidBrush(Const.ClosedTitleColor);
            m_brushSelTitle = new SolidBrush(Const.SelTitleColor);
            m_brushCaption = new SolidBrush(Const.CaptionColor);
            m_brushClosedCaption = new SolidBrush(Const.ClosedCaptionColor);
            m_brushSelCaption = new SolidBrush(Const.SelCaptionColor);            
            m_brushSelBack = new SolidBrush(Const.SelBackColor);
            m_penSep = new Pen(Const.SepLine);

            // probress bar
            m_brushProgress = new SolidBrush(Const.ProgressColor);
            m_brushProgressComplete = new SolidBrush(Const.ProgressCompleteColor);
            m_penProgress = new Pen(Const.ProgressFrameColor);
        }


        // create memory bitmap to draw on
        private void CreateMemoryBitmap()
        {
            // only create if don't have one of size changed
            if (m_bmp == null)
            {
                // memory bitmap to draw on
                m_bmp = new Bitmap(this.Width - Const.ScrollBarWidth, this.Height);

                // init scrollbar size
                m_scrollBar.Left = this.Width - Const.ScrollBarWidth;
                m_scrollBar.Top = 0;
                m_scrollBar.Width = Const.ScrollBarWidth;
                m_scrollBar.Height = this.Height;
            }
        }

    }
}
