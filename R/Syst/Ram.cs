using System.Diagnostics;

// ReSharper disable once CheckNamespace
namespace R
{
    public static class Ram
    {
        /// <summary>
        /// Permet de récupèrer la mémoire allouée de l'application
        /// </summary>
        /// <returns>long</returns>
        public static long GetMemoryApp()
        {
            Process proc = Process.GetCurrentProcess();
            return proc.PrivateMemorySize64;
        }
    }
}
