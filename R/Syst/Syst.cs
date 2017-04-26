using System;
using System.Diagnostics;
using System.Security.Principal;
using System.Windows.Forms;

// ReSharper disable once CheckNamespace
namespace R
{
    public static class Syst
    {
        public static void ExitConsole(int code = 0)
        {
            Environment.Exit(code);
        }

        public static string NameSession()
        {
            try
            {
                var windowsIdentity = WindowsIdentity.GetCurrent();
                if (windowsIdentity == null) return null;

                string tempusername = windowsIdentity.Name;
                int i = tempusername.IndexOf(@"\", StringComparison.Ordinal);
                return tempusername.Substring(i + 1, tempusername.Length - i - 1);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static bool Shutdown(int time = 0)
        {
            try
            {
                return Process.Start("shutdown", "-r -t "+time).IsNotNull();
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool StopShutdown()
        {
            try
            {
                return Process.Start("shutdown", "/a").IsNotNull();
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool RestartApplication()
        {
            try
            {
                Application.Restart();
                Process.GetCurrentProcess().Kill();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
