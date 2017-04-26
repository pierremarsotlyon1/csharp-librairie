using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;

namespace R
{
    public class Monitor
    {
        private readonly FileSystemWatcher _monitor;
        private readonly string _nameDossierBase;

        public event Action<string, byte[]> Create;

        protected virtual void OnCreate(string arg1, byte[] arg2)
        {
            Action<string, byte[]> handler = Create;
            if (handler != null) handler(arg1, arg2);
        }

        public Monitor(string path = null, string nameDossierBase = null)
        {
            if (path.IsNullOrEmpty() || nameDossierBase.IsNullOrEmpty()) return;
            _monitor = new FileSystemWatcher(path)
            {
                NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite
                               | NotifyFilters.FileName | NotifyFilters.DirectoryName | NotifyFilters.Size |
                               NotifyFilters.CreationTime
            };
            _nameDossierBase = nameDossierBase;
            Run();
        }

        [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
        public void Run()
        {
            _monitor.Changed += Changed;
            _monitor.Created += Created;
            _monitor.Deleted += Deleted;
            _monitor.Renamed += Renamed;
            _monitor.EnableRaisingEvents = true;
        }

        private void Renamed(object sender, RenamedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void Deleted(object sender, FileSystemEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void Created(object sender, FileSystemEventArgs e)
        {
            try
            {
                //On recup le path du fichier sans la racine
                string path = null;
                var b = false;
                var tab = e.FullPath.Split('\\').ToList();
                foreach (var s in tab)
                {
                    if (b)
                        path += tab.CheckLastElement(s) ? s : (s + "\\");

                    if (s.Equals(_nameDossierBase))
                        b = true;
                }
                
                //Get du contenu fichier
                var data = File.ReadAllBytes(e.FullPath);

                //Event (envoi par socket)
                Create(path, data);
            }
            catch (Exception)
            {
                
            }
        }

        private void Changed(object sender, FileSystemEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
