using System;
using System.Windows.Forms;


namespace Izume.Mobile.Odeo.Common
{
    public class GuiHelper
    {

        private GuiHelper() { /* hide default constructor */}


        public static void SetStyle(System.Windows.Forms.Control control)
        {
            foreach (Control child in control.Controls)
            {
                if (child.Controls.Count > 0)
                    SetStyle(child);

                if (child is Label || child is TextBoxBase)
                    child.Font = Common.Constants.Fonts.SmallEmphasis;

                if (child is TextBoxBase)
                {
                    child.ForeColor = Common.Constants.Colors.DarkBlue;
                    child.Font = Common.Constants.Fonts.Small;
                }

                if (child is TreeView)
                    child.Font = Common.Constants.Fonts.Small;
            }
        }

    }
}
