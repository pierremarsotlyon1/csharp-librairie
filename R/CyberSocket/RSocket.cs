using System;
using System.Collections.Generic;
using System.Configuration.Internal;
using System.Net;
using System.Net.Sockets;
using System.Text;

// ReSharper disable once CheckNamespace
namespace R
{
    public class RSocket
    {
        private readonly TcpClient _client;
        private readonly NetworkStream _clientStream;
        private readonly ASCIIEncoding _encoder;
        private byte[] _buffer;
        private const int TailleBuffer = 4092;

        public bool IsConneted;
        public CyberData Reponse;

        public RSocket(string ip , int port)
        {
            try
            {
                //Init socket
                _client = new TcpClient();
                _encoder = new ASCIIEncoding();
                _buffer = new byte[TailleBuffer];
                Reponse = null;

                //Connexion au serveur
                IPEndPoint serverEndPoint = new IPEndPoint(IPAddress.Parse(ip), port);
                _client.Connect(serverEndPoint);

                //Get du flux
                _clientStream = _client.GetStream();
                IsConneted = true;
            }
            catch (Exception)
            {
                IsConneted = false;
                // ReSharper disable once RedundantJumpStatement
                return;
            }
            
        }

        public bool Disconnect()
        {
            try
            {
                _client.Close();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool Send(string clef)
        {
            try
            {
                if (clef.IsNullOrEmpty()) return false;
                var message = new CyberData { Clef = clef, Objet = null }.ToByte();
                if (message.IsNull()) return false;

                _clientStream.Write(message, 0, message.Length);
                _clientStream.Flush();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool Send<T>(string clef, T obj)
        {
            try
            {
                if (clef.IsNullOrEmpty()) return false;
                var message = new CyberData {Clef = clef, Objet = obj}.ToByte();
                if (message.IsNull()) return false;
                
                _clientStream.Write(message, 0, message.Length);
                _clientStream.Flush();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool Read()
        {
            try
            {
                _buffer = new byte[TailleBuffer];
                Reponse = null;

                if (CheckSocket().IsFalse()) return false;
                var listeByte = new List<byte>();

                //On commence la lecture des données
                do
                {
                    var numberOfBytesRead = _clientStream.Read(_buffer, 0, _buffer.Length);
                    if (numberOfBytesRead < 1) break;
                    listeByte.AddRange(_buffer);
                }
                while (_clientStream.DataAvailable);

                //On deserialize les données
                Reponse = listeByte.ToArray().BinaryUnserialize<CyberData>();
                
                return !Reponse.IsNull();
            }
            catch (Exception )
            {
                return false;
            }
        }

        private bool CheckSocket()
        {
            try
            {
                return _client.IsNotNull() && _client.Connected;
            }
            catch (Exception)
            {
                return false;
            }
        }


        /*public Socket Socket;

        public RSocket()
        {
            Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        /// <summary>
        /// Permet à un socket de ce connecter
        /// </summary>
        /// <param name="ip">L'ip du serveur</param>
        /// <param name="port">Le port du serveur</param>
        /// <returns>bool</returns>
        public bool Connexion(string ip, int port)
        {
            try
            {
                Socket.Connect(ip, port);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Permet de déconnecter un socket
        /// </summary>
        /// <returns>bool</returns>
        public bool Deconnexion()
        {
            try
            {
                if (!CheckSocket()) return false;
                Socket.Close();
                Socket.Dispose();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Permet d'envoyer des données
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="clef">La clef</param>
        /// <param name="obj">L'objet à envoyer</param>
        /// <returns></returns>
        public bool Send<T>(string clef, T obj)
        {
            try
            {
                if (!CheckSocket()) return false;
                var reponse = Socket.Send(new CyberData {Clef = clef, Objet = obj}.ToByte());
                return reponse > 0;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Méthode qui permet de recevoir des données
        /// </summary>
        /// <returns>byte{}</returns>
        public byte[] Receive()
        {
            try
            {
                if(!CheckSocket())return new byte[0];
                var buffer = new byte[128];
                Socket.Receive(buffer);
                return buffer;
            }
            catch (Exception)
            {
                return new byte[0];
            }
        }
        /// <summary>
        /// Vérifie si le socket est non null ainsi que connecté
        /// </summary>
        /// <returns>bool</returns>
        private bool CheckSocket()
        {
            try
            {
                return !Socket.IsNull() && Socket.Connected;
            }
            catch (Exception)
            {
                return false;
            }
        }*/
    }
}
