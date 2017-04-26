using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace R.CyberSocket
{
    public class GestionServeur
    {
        private readonly TcpListener _tcpListener;
        private readonly List<TcpClient> _listeClient; 
        public event Action<CyberData, TcpClient> NewDemande;
        public bool RunServeur;

        public GestionServeur()
        {
            try
            {
                _tcpListener = new TcpListener(IPAddress.Any, 8080);
                _listeClient = new List<TcpClient>();
                var listenThread = new Thread(ListenForClients);
                listenThread.Start();
                RunServeur = true;
            }
            catch (Exception)
            {
                RunServeur = false;
            }
        }

        private void ListenForClients()
        {
            try
            {
                _tcpListener.Start();

                while (true)
                {
                    //On attend qu'un client se connecte
                    TcpClient client = _tcpListener.AcceptTcpClient();

                    //On crée un Thread d'écoute pour le client
                    new Thread(HandleClientComm).Start(client);
                    _listeClient.Add(client);
                    "ClientTomuss connecté".WriteLn();
                }
            }
            catch (Exception)
            {
                // ReSharper disable once RedundantJumpStatement
                return;
            }
        }

        private void HandleClientComm(object client)
        {
            try
            {
                TcpClient tcpClient = (TcpClient)client;
                NetworkStream clientStream = tcpClient.GetStream();

                byte[] message = new byte[4096];

                while (true)
                {
                    // ReSharper disable once RedundantAssignment
                    int bytesRead = 0;

                    try
                    {
                        //On attend que le client envoie un message
                        bytesRead = clientStream.Read(message, 0, 4096);
                    }
                    catch
                    {
                        //Erreur lors de la lecture
                        break;
                    }

                    if (bytesRead == 0)
                    {
                        //Le client est déconnecté du serveur
                        break;
                    }

                    //Check info reçue
                    if (message.IsNull() || message.Length == 0)
                    {
                        WriteClient(new CyberData { Clef = "Erreur", Objet = "Erreur lors de la récupération du message." },
                            tcpClient);
                        continue;
                    }

                    //Message bien reçu, on le traîte
                    var obj = message.BinaryUnserialize<CyberData>();
                    if (obj.IsNull())
                    {
                        WriteClient(new CyberData { Clef = "Erreur", Objet = "Erreur lors de la récupération du message." },
                            tcpClient);
                        continue;
                    }

                    //On déclenche l'event
                    OnNewDemande(obj, tcpClient);
                }

                tcpClient.Close();

                //On retire le client de la liste
                _listeClient.Remove(tcpClient);
                "ClientTomuss deconnecté".WriteLn();
            }
            catch (Exception)
            {
                // ReSharper disable once RedundantJumpStatement
                return;
            }
        }

        public bool WriteClient(CyberData cyber, TcpClient client)
        {
            try
            {
                //Check
                if (cyber.IsNull() || client.IsNull()) return false;

                //On envoie la réponse
                NetworkStream clientStream = client.GetStream();
                byte[] buffer = cyber.BinarySerialize();
                clientStream.Write(buffer, 0, buffer.Length);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        protected virtual void OnNewDemande(CyberData arg1, TcpClient arg2)
        {
            var handler = NewDemande;
            if (handler != null) handler(arg1, arg2);
        }

        /// <summary>
        /// Permet de récupérer le nombre de client connecté au serveur
        /// </summary>
        /// <returns>int</returns>
        public int GetNumberClient()
        {
            try
            {
                return _listeClient.IsEmpty() ? 0 : _listeClient.Count;
            }
            catch (Exception)
            {
                return 0;
            }
        }
    }
}
