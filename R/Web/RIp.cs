using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;

namespace R.Web
{
    public static class RIp
    {
        /// <summary>
        /// Obtient l'ip de l'utilisateur
        /// </summary>
        /// <returns>string</returns>
        public static string GetIp()
        {
            return HttpContext.Current != null ? HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"] : null;
        }

        public static string GetIpAddress()
        {
            try
            {
                HttpContext context = HttpContext.Current;
                if (context.IsNull()) return null;

                string ipAddress = context.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

                if (!ipAddress.IsNotNullOrEmpty()) return context.Request.ServerVariables["REMOTE_ADDR"];
                string[] addresses = ipAddress.Split(',');
                return addresses.Length != 0 ? addresses[0] : context.Request.ServerVariables["REMOTE_ADDR"];
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Obtient la vraie ip de l'utilisateur
        /// </summary>
        /// <returns>string</returns>
        public static string GetIpViaDns()
        {
            try
            {
                // check IP using DynDNS's service
                WebRequest request = WebRequest.Create("http://checkip.dyndns.org");
                if (request.IsNull()) return null;
                // IMPORTANT: set Proxy to null, to drastically INCREASE the speed of request
                request.Proxy = null;

                WebResponse response = request.GetResponse();
                if (response.IsNull()) return null;

                StreamReader stream = new StreamReader(response.GetResponseStream());

                // read complete response
                string ipAddress = stream.ReadToEnd();

                // replace everything and keep only IP
                return ipAddress.
                    Replace("<html><head><title>Current IP Check</title></head><body>Current IP Address: ", string.Empty)
                    .
                    Replace("</body></html>", string.Empty);
            }
            catch (Exception)
            {
                return null;
            }

        }

        /// <summary>
        /// Permet de crypter une ip
        /// </summary>
        /// <param name="ip">string</param>
        /// <returns>string</returns>
        public static string CrypterIp(string ip)
        {
            try
            {
                if (string.IsNullOrEmpty(ip)) return null;
                var tab = ip.ToArray('.');
                //On regarde que l'ip soit correcte

                if (tab.Length > 4) return null;

                string ipTemp = null;
                const int nbAlpha = 26;


                foreach (int i in tab.Select(v => v.ToInt()))
                {
                    //On regarde si 0 < valeur < 256 
                    if (i > 255 || i < 0) return null;

                    bool b = true;
                    int intTemp = 0;

                    while (b)
                    {
                        //On regarde le nombre de tour de l'alphabet
                        if (intTemp + nbAlpha <= i)
                            intTemp += nbAlpha;
                        else
                            b = false;
                    }

                    //On obtient notre chiffre
                    ipTemp += intTemp / nbAlpha;

                    if (intTemp != i)
                    {
                        //On calcul la lettre restante
                        ipTemp += (i - intTemp).ToLettreAlpha();
                    }
                }

                return ipTemp;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Permet de décrypter une adresse ip
        /// </summary>
        /// <param name="ip">string</param>
        /// <returns>string</returns>
        public static string DecrypterIp(string ip)
        {
            try
            {
                if (string.IsNullOrEmpty(ip) || ip.Length == 0) return null;

                //On convertit le string en tableau
                var tab = ip.ToCharArray();
                //Init
                const int nbAlpha = 26;
                ip = null;
                string tempIp = null;
                bool b = false;
                int count = 0;

                foreach (var value in tab)
                {
                    count++;
                    if (value.CheckInt() && !b)
                    {
                        b = true;
                        //Si fin du tableau on ajoute la variable à l'ip
                        if (count == tab.Length)
                        {
                            ip += (nbAlpha * tempIp.ToInt());
                            break;
                        }
                        //Dans le cas ou on aurait une ip avec 0.x
                        if (value.Equals('0') && tab[count].CheckInt())
                        {
                            ip += "0.";
                            tempIp = null;
                            b = false;
                            continue;
                        }
                        //Sinon on ajoute à tempIp la variable
                        tempIp += value;


                    }
                    else
                    {
                        //Si la valeur courante est de type int, cela veut dire que l'on à % == 0 donc on ajoute la variable précédente et on garde la valeur courante en tampon
                        if (value.CheckInt())
                        {
                            ip += (nbAlpha * tempIp.ToInt());
                            tempIp = value.RToString();
                        }
                        //Sinon la valeur est une lettre donc on calcul
                        else
                        {
                            ip += (nbAlpha * tempIp.ToInt()) + value.AlphaToInt();
                            tempIp = null;
                        }

                        if (count < tab.Length)
                            ip += '.';

                        b = false;
                    }
                }

                return ip;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
