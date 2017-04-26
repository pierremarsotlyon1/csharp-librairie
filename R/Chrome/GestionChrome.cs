using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using R.Chrome;

// ReSharper disable once CheckNamespace

namespace R
{
    public class GestionChrome
    {
       

        public bool OpenUrl(string url)
        {
            try
            {
                if (url.IsNullOrEmpty()) return false;
                Process.Start("chrome.exe", url);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}