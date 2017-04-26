using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace R.PressePapier
{
    public class PressePapier
    {
        public PressePapier()
        {
            
        }

        public bool Set(string str = null)
        {
            try
            {
                if (str.IsNullOrEmpty()) return false;
                Clipboard.SetText(str);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public string Get()
        {
            try
            {
                return Clipboard.GetText();
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
