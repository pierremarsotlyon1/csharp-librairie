using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace R.Syst
{
    public static class RScreen
    {
        public static int WidthScreen()
        {
            return Screen.PrimaryScreen.Bounds.Width;
        }

        public static int HeightScreen()
        {
            return Screen.PrimaryScreen.Bounds.Height;
        }
    }
}
