using System;
using System.Threading;

namespace R
{
    public static class RThread
    {
        public static event Action<Thread> Finished;

        private static void OnFinished(Thread obj)
        {
            Action<Thread> handler = Finished;
            if (handler != null) handler(obj);
        }

        /// <summary>
        ///     Permet de faire un Thread.Sleep
        /// </summary>
        /// <param name="i">Millisecondes à attendre</param>
        public static void Sleep(this int i)
        {
            Thread.Sleep(i);
        }

        /// <summary>
        ///     Permet de faire un Thread.Sleep
        /// </summary>
        /// <param name="i">Minutes à attendre</param>
        public static void SleepToMin(this int i)
        {
            Thread.Sleep(i*60*1000);
        }

        public static void SleepToHour(this int i)
        {
            Thread.Sleep(i * 60 * 60 * 1000);
        }

        /// <summary>
        ///     Permet de faire un Thread.Sleep
        /// </summary>
        /// <param name="i">Secondes à attendre</param>
        public static void SleepToSec(this int i)
        {
            Thread.Sleep(i*1000);
        }

        /// <summary>
        ///     Permet de créer un Thread et de le lancer
        /// </summary>
        /// <param name="action">Méthode à executer par le Thread</param>
        public static void DoAsync(Action action)
        {
            new Thread(() => action()).Start();
        }

        /// <summary>
        ///     Permet de créer un Thread
        /// </summary>
        /// <param name="action">Méthode à executer par le Thread</param>
        public static Thread DeclareAsync(Action action)
        {
            return new Thread(() =>
            {
                Thread currentThread = GetCurrentThread();
                try
                {
                    action();
                }
                catch (Exception)
                {
                    OnFinished(currentThread);
                }
            });
        }

        /// <summary>
        ///     Permet de récupérer le thread courant
        /// </summary>
        /// <returns>Thread</returns>
        public static Thread GetCurrentThread()
        {
            try
            {
                return Thread.CurrentThread;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}