using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

// ReSharper disable once CheckNamespace

namespace R
{
    public static class RDns
    {
        /// <summary>
        ///     Permet de récupérer le nom de la machine
        /// </summary>
        /// <returns>string</returns>
        public static string GetNameMachine()
        {
            return Dns.GetHostName();
        }

        /// <summary>
        ///     Permet de récupérer les adresses qui sont sur la machine
        /// </summary>
        /// <returns>IPAddress[]</returns>
        public static IPAddress[] GetHostAdress()
        {
            return Dns.GetHostAddresses(GetNameMachine());
        }

        /// <summary>
        ///     Vérifie qu'une ipv6 est de lien local
        /// </summary>
        /// <param name="str"></param>
        /// <returns>bool</returns>
        public static bool IsIpv6Local(this IPAddress str)
        {
            try
            {
                return str.IsIPv6LinkLocal;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        ///     Vérifie qu'une ipv6 est d'un site local
        /// </summary>
        /// <param name="str"></param>
        /// <returns>bool</returns>
        public static bool IsIpv6SiteLocal(this IPAddress str)
        {
            return str.IsIPv6SiteLocal;
        }

        /// <summary>
        ///     Vérifie qu'une ipv6 est de type Teredo
        /// </summary>
        /// <param name="str"></param>
        /// <returns>bool</returns>
        public static bool IsIpv6Teredo(this IPAddress str)
        {
            return str.IsIPv6Teredo;
        }

        /// <summary>
        ///     Permet de récupérer les ipv4 à partir d'une liste d'ip
        /// </summary>
        /// <param name="liste"></param>
        /// <returns>List{IPAddress}</returns>
        public static List<IPAddress> GetIpv4(this IPAddress[] liste)
        {
            try
            {
                return
                    liste.Where(
                        ipAddress =>
                            !ipAddress.IsIPv6LinkLocal && !ipAddress.IsIPv6Multicast && !ipAddress.IsIPv6SiteLocal &&
                            !ipAddress.IsIPv6Teredo).ToList();
            }
            catch (Exception)
            {
                return new List<IPAddress>();
            }
        }

        /// <summary>
        ///     Vérifit si une ip est de type IPV4
        /// </summary>
        /// <param name="ip">L'ip à vérifier</param>
        /// <returns>bool</returns>
        public static bool IsIpv4(this string ip)
        {
            try
            {
                IPAddress address;

                if (ip.IsNullOrEmpty() || !IPAddress.TryParse(ip, out address)) return false;
                return address.AddressFamily == AddressFamily.InterNetwork;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        ///     Permet de vérifier si une IP est valide
        /// </summary>
        /// <param name="addr">L'ip à vérifier</param>
        /// <returns>bool</returns>
        public static bool IsValidIp(string addr)
        {
            try
            {
                IPAddress ip;
                return !addr.IsNullOrEmpty() && IPAddress.TryParse(addr, out ip);
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        ///     Indique si il y a une connexion internet valide
        /// </summary>
        /// <param name="minimumSpeed">Minimum de la connexion possible. Envoyer 0 pour ne pas établir de filtre</param>
        /// <returns>
        ///     <c>true</c>si la connexion est valide; sinon, <c>false</c>.
        /// </returns>
        public static bool IsNetworkAvailable(long minimumSpeed = 0)
        {
            return NetworkInterface.GetIsNetworkAvailable() && (from ni in NetworkInterface.GetAllNetworkInterfaces()
                where
                    (ni.OperationalStatus == OperationalStatus.Up) &&
                    (ni.NetworkInterfaceType != NetworkInterfaceType.Loopback) &&
                    (ni.NetworkInterfaceType != NetworkInterfaceType.Tunnel)
                where ni.Speed >= minimumSpeed
                where
                    (ni.Description.IndexOf("virtual", StringComparison.OrdinalIgnoreCase) < 0) &&
                    (ni.Name.IndexOf("virtual", StringComparison.OrdinalIgnoreCase) < 0)
                select ni).Any(
                    ni => !ni.Description.Equals("Microsoft Loopback Adapter", StringComparison.OrdinalIgnoreCase));
        }

        public static bool HasInternetConnection()
        {
            return NetworkInterface.GetIsNetworkAvailable();
        }
    }
}