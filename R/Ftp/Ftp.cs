using System;
using System.IO;
using System.Net;

namespace R.Ftp
{
    public class Ftp
    {
        public string Hote { get; set; }
        public string Pseudo { get; set; }
        public string Password { get; set; }
        public string Chemin { get; set; }

        public event Action<string> UplaodFileAsynchCompleted;
        public event Action<string> UplaodFileCompleted;

        public Ftp()
        {

        }

        public Ftp(string hote, string pseudo, string password, string chemin)
        {
            Hote = hote;
            Pseudo = pseudo;
            Password = password;
            Chemin = chemin;
        }

        /// <summary>
        /// Event qui se déclenche lorsque le fichier à été uploadé de façon asynchrone
        /// </summary>
        /// <param name="obj">Chemin du fichier</param>
        protected virtual void OnUplaodFileAsynchCompleted(string obj)
        {
            var handler = UplaodFileAsynchCompleted;
            if (handler != null) handler(obj);
        }

        /// <summary>
        /// Event qui se déclenche lorsque le fichier à été uploadé de façon synchrone
        /// </summary>
        /// <param name="obj">Chemin du fichier</param>
        protected virtual void OnUplaodFileCompleted(string obj)
        {
            var handler = UplaodFileCompleted;
            if (handler != null) handler(obj);
        }

        /// <summary>
        /// Permet d'uploader un fichier en asynchrone
        /// </summary>
        /// <param name="filename">Chemin du fichier à uploader</param>
        /// <returns>bool</returns>
        public bool UploadFileAsynch(string filename)
        {
            try
            {
                RThread.DoAsync(() => Upload(ref filename));
                OnUplaodFileAsynchCompleted(filename);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Permet d'uploader un fichier de façon synchrone
        /// </summary>
        /// <param name="filename">Chemin du fichier à uploader</param>
        /// <returns>bool</returns>
        public bool UploadFile(string filename)
        {
            try
            {
                Upload(ref filename);
                OnUplaodFileCompleted(filename);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool DeleteFile(string filename = null)
        {
            try
            {
                if (filename.IsNullOrEmpty()) return false;

                var objFile = new FileInfo(filename);
                var request = (FtpWebRequest)WebRequest.Create(new Uri("ftp://" + Hote + "/" + Chemin + "/" + objFile.Name));

                //If you need to use network credentials
                request.Credentials = new NetworkCredential(Pseudo, Password);
                //additionally, if you want to use the current user's network credentials, just use:
                //System.Net.CredentialCache.DefaultNetworkCredentials

                request.Method = WebRequestMethods.Ftp.DeleteFile;
                FtpWebResponse response = (FtpWebResponse)request.GetResponse();
                response.Close();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Méthode permettant l'upload du fichier
        /// </summary>
        /// <param name="filename">Chemin du fichier</param>
        private void Upload(ref string filename)
        {
            if (filename.IsNullOrEmpty()) throw new Exception();
            try
            {
                var objFile = new FileInfo(filename);

                //Création de l'objet FtpWebRequest
                var objFtpRequest = (FtpWebRequest)WebRequest.Create(new Uri("ftp://" + Hote + "/" + Chemin + "/" + objFile.Name));

                //On ajoute les logs de connexion Credintials
                objFtpRequest.Credentials = new NetworkCredential(Pseudo, Password);

                objFtpRequest.KeepAlive = false;
                objFtpRequest.UseBinary = true;
                objFtpRequest.Timeout = 999999999;
                objFtpRequest.ContentLength = objFile.Length;

                //On défini la méthode de la requête
                objFtpRequest.Method = WebRequestMethods.Ftp.UploadFile;

                const int intBufferLength = 16 * 1024;
                var objBuffer = new byte[intBufferLength];

                //On ouvre le fichier à uploader
                FileStream objFileStream = objFile.OpenRead();

                Stream objStream = objFtpRequest.GetRequestStream();

                int len;

                while ((len = objFileStream.Read(objBuffer, 0, intBufferLength)) != 0)
                {
                    //On écrit le contenu
                    objStream.Write(objBuffer, 0, len);

                }

                objStream.Close();
                objFileStream.Close();
            }
            catch (Exception ex)
            {
                // ReSharper disable once PossibleIntendedRethrow
                throw ex;
            }
        }

        public bool RenameFile(string filename = null, string newFilename = null)
        {
            if (filename.IsNullOrEmpty() || newFilename.IsNullOrEmpty()) return false;
            try
            {
                var reqFtp = (FtpWebRequest)FtpWebRequest.Create(new Uri("ftp://" + Hote + "/" +Chemin+"/"+ filename));
                reqFtp.Method = WebRequestMethods.Ftp.Rename;
                reqFtp.RenameTo = newFilename;
                reqFtp.UseBinary = true;
                reqFtp.Credentials = new NetworkCredential(Pseudo, Password);
                FtpWebResponse response = (FtpWebResponse)reqFtp.GetResponse();
                var ftpStream = response.GetResponseStream();
                if (ftpStream != null) ftpStream.Close();
                response.Close();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
