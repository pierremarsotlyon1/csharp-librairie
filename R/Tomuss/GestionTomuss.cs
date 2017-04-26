using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using HtmlAgilityPack;
using R.Web;

namespace R
{
    public class GestionTomuss
    {
        private const string UserGmail = "encom.agency@gmail.com";
        private const string PasswordGmail = "0890877542901";
        private const string SmtpGmail = "smtp.gmail.com";

        private static RmySqlObject _gestionBdd;
        private static List<ClientTomuss> _listeClient;
        private readonly REmailSender _gestionEmail;
        private readonly Object _lockBdd;
        private bool _continueThread = true;

        public GestionTomuss(RmySqlObject gest)
        {
            if (!gest.CheckConnexion) return;
            _gestionEmail = new REmailSender(UserGmail, SmtpGmail, UserGmail, PasswordGmail);
            _lockBdd = new object();
            _gestionBdd = gest;

            _listeClient = new List<ClientTomuss>();
            var timer = new Timer(20 * 1000);
            timer.Elapsed += OnTimedEvent;

            //Création / update des table
            _gestionBdd.CreateOrUpdateTable(new ClientTomuss());
            _gestionBdd.CreateOrUpdateTable(new ClientUe());
            _gestionBdd.CreateOrUpdateTable(new Ue());
            _gestionBdd.CreateOrUpdateTable(new Note());
            _gestionBdd.CreateOrUpdateTable(new FluxRss());
            _gestionBdd.CreateOrUpdateTable(new Presence());
            _gestionBdd.CreateOrUpdateTable(new Absence());

            //Get des clients
            _listeClient = _gestionBdd.Select(new ClientTomuss());
            if (_listeClient.IsEmpty()) return;

            //Get des ue et notes pour chaque ClientTomuss
            foreach (ClientTomuss client in _listeClient)
            {
                if (client.IsNull()) continue;
                List<Ue> listeUe =
                    _gestionBdd.ManualSelect(
                        "SELECT * from ue u JOIN clientue cu ON cu.IdUe = u.id_ue WHERE cu.IdClient = " + client.Id,
                        new Ue());
                if (listeUe.IsEmpty()) continue;
                client.ListeUe = listeUe;

                //Get des notes pour chaque ue
                foreach (Ue ue in client.ListeUe)
                {
                    if (ue.IsNull()) continue;
                    List<Note> listeNote = _gestionBdd.Select(new Note(), new[]
                    {
                        new RField {Name = "IdClient", Operator = "=", Value = client.Id},
                        new RField {ConditionOperator = "AND", Name = "IdUe", Operator = "=", Value = ue.Id}
                    });
                    if (listeNote.IsEmpty()) continue;
                    ue.ListeNote = listeNote;
                }

                //Get des présences
                List<Presence> listePresence = _gestionBdd.Select(new Presence(), new[]
                {
                    new RField {Name = "IdClient", Value = client.Id, Operator = "="}
                });
                if (listePresence.IsNotEmpty())
                    client.ListePresence = listePresence;

                //Get des absences
                List<Absence> listeAbsence = _gestionBdd.Select(new Absence(), new[]
                {
                    new RField {Name = "IdClient", Value = client.Id, Operator = "="}
                });
                if (listeAbsence.IsNotEmpty())
                    client.ListeAbsence = listeAbsence;

                //Get du lien du flux
                List<FluxRss> flux = _gestionBdd.Select(new FluxRss(), new[]
                {
                    new RField {Name = "IdClient", Operator = "=", Value = client.Id}
                });
                if (!flux.IsNotEmpty()) continue;
                client.FluxRss = flux.GetFirstElement();
                ChangedFluxRss(client, _gestionBdd);
            }
        }

        private void OnTimedEvent(object sender, ElapsedEventArgs e)
        {
            try
            {
                lock (_lockBdd)
                {
                    //On regarde si il y a des nouveaux clients
                    List<ClientTomuss> liste = _gestionBdd.Select(new ClientTomuss(), new[]
                    {
                        new RField {Name = "id_client", Operator = ">", Value = _listeClient.GetLastElement().Id}
                    });
                    if (liste.IsEmpty()) return;
                    FinishThreads();
                    2.SleepToMin();
                    Syst.RestartApplication();
                }
            }
            catch (Exception)
            {
                return;
            }
        }

        public void ChangedFluxRss(ClientTomuss clientTomuss = null, RmySqlObject gestionBdd = null)
        {
            try
            {
                if (clientTomuss.IsNull() || gestionBdd.IsNull()) return;
                if (clientTomuss.FluxRss.Lien.IsNullOrEmpty()) return;

                //On récup la taille du fichier
                int tailleOctetFichier = GetTailleContenuRss(clientTomuss);

                bool firstPassage = true;
                RThread.DoAsync(() =>
                {
                    while (_continueThread)
                    {
                        if (!firstPassage)
                        {
                            2.SleepToMin();
                        }
                        firstPassage = false;

                        //On récup le html du flux RSS
                        HtmlDocument doc = clientTomuss.FluxRss.Lien.RloadUrl();
                        if (doc.IsNull()) continue;
                        string html = doc.DocumentNode.InnerHtml;
                        if (html.IsNullOrEmpty()) continue;

                        //Si taille identique, on arrête
                        if (html.Length == tailleOctetFichier) continue;

                        //On send un mail pour dire qu'il y a une notif
                        _gestionEmail.EnvoyerEmail(UserGmail, "Notification Tomuss",
                            CompareFlux(html, clientTomuss, gestionBdd));

                        //Traitement fichier
                        if (!ClearContenuRss(clientTomuss, gestionBdd)) continue;
                        if (!WriteContenuRss(clientTomuss, gestionBdd, html)) continue;

                        //On récup la nouvelle taille
                        tailleOctetFichier = GetTailleContenuRss(clientTomuss);
                    }
                });
            }
            catch (Exception)
            {
            }
        }

        private string CompareFlux(string flux = null, ClientTomuss clientTomuss = null, RmySqlObject gestionBdd = null,
            bool newCheck = false)
        {
            try
            {
                if (clientTomuss.IsNull() || gestionBdd.IsNull()) return null;
                string strFichier = clientTomuss.FluxRss.Contenu;
                if (strFichier.IsNullOrEmpty())
                    strFichier = "";
                if (flux.IsNullOrEmpty()) return null;

                if (strFichier.Equals(flux) && newCheck == false) return null;
                var doc = new HtmlDocument();
                var doc2 = new HtmlDocument();
                doc.LoadHtml(strFichier);
                doc2.LoadHtml(flux);

                //On recup les items du nouveau document
                HtmlNodeCollection listeItemsNewDocument = doc2.DocumentNode.SelectNodes("//item");
                //On recup les items de l'ancien document
                HtmlNodeCollection listeItemsOldDocument = doc.DocumentNode.SelectNodes("//item");

                if (listeItemsNewDocument == null) return null;
                if (listeItemsOldDocument == null)
                    listeItemsOldDocument = new HtmlNodeCollection(null);
                string reponse = null;
                foreach (HtmlNode newitem in listeItemsNewDocument)
                {
                    if (newitem.IsNull()) continue;
                    bool check =
                        listeItemsOldDocument.Where(olditem => !olditem.IsNull())
                            .Any(olditem => newitem.InnerText.Equals(olditem.InnerText));

                    if (check) continue;

                    //On a pas trouvé l'item dans l'ancien rss, on récup les infos
                    HtmlNode node = newitem.SelectSingleNode("./title");
                    if (node.IsNull()) continue;
                    reponse += "Titre : " + node.InnerText + "\n";
                    //On regarde si c'est une présence
                    if (node.InnerText.ToUpper().Contains("PRST"))
                    {
                        if (node.InnerText.ToUpper().Contains(":"))
                        {
                            List<string> tab = node.InnerText.ToArray(':').ToList();
                            if (tab.IsNotEmpty())
                            {
                                //On récup l'ue
                                string nameUe = tab.GetFirstElement().Trim();
                                if (nameUe.IsNotNullOrEmpty())
                                {
                                    Ue ue = clientTomuss.ListeUe.FirstOrDefault(ue2 => ue2.Nom.Equals(nameUe));

                                    Presence presence;
                                    if (ue.IsNotNull())
                                    {
                                        presence = new Presence
                                        {
                                            Date = RDateTime.GetDateTime(),
                                            IdClient = clientTomuss.Id,
                                            IdUe = ue.Id
                                        };
                                        //On save la présence en bdd
                                        SavePresence(clientTomuss, gestionBdd, presence);
                                        //On ajoute le présence au ClientTomuss
                                        clientTomuss.ListePresence.Add(presence);
                                    }
                                    else
                                    {
                                        //On a pas trouvé l'ue, on l'ajout à la liste des ue du ClientTomuss et en bdd
                                        ue = AddUe(clientTomuss, new Ue
                                        {
                                            Nom = nameUe
                                        }, gestionBdd);
                                        presence = new Presence
                                        {
                                            Date = RDateTime.GetDateTime(),
                                            IdClient = clientTomuss.Id,
                                            IdUe = ue.Id
                                        };
                                        //On save la présence en bdd
                                        SavePresence(clientTomuss, gestionBdd, presence);
                                        //On ajoute le présence au ClientTomuss
                                        clientTomuss.ListePresence.Add(presence);
                                    }
                                }
                            }
                        }
                    }
                    else if (node.InnerText.ToUpper().Contains("ABINJ"))
                    {
                        if (node.InnerText.ToUpper().Contains(":"))
                        {
                            List<string> tab = node.InnerText.ToArray(':').ToList();
                            if (tab.IsNotEmpty())
                            {
                                //On récup l'ue
                                string nameUe = tab.GetFirstElement().Trim();
                                if (nameUe.IsNotNullOrEmpty())
                                {
                                    Ue ue = clientTomuss.ListeUe.FirstOrDefault(ue2 => ue2.Nom.Equals(nameUe));

                                    Absence absence;
                                    if (ue.IsNotNull())
                                    {
                                        absence = new Absence
                                        {
                                            Date = RDateTime.GetDateTime(),
                                            IdClient = clientTomuss.Id,
                                            IdUe = ue.Id
                                        };
                                        //On save la présence en bdd
                                        SaveAbsence(clientTomuss, gestionBdd, absence);
                                        //On ajoute le présence au ClientTomuss
                                        clientTomuss.ListePresence.Add(absence);
                                    }
                                    else
                                    {
                                        //On a pas trouvé l'ue, on l'ajout à la liste des ue du ClientTomuss et en bdd
                                        ue = AddUe(clientTomuss, new Ue
                                        {
                                            Nom = nameUe
                                        }, gestionBdd);
                                        absence = new Absence
                                        {
                                            Date = RDateTime.GetDateTime(),
                                            IdClient = clientTomuss.Id,
                                            IdUe = ue.Id
                                        };
                                        //On save l'absence en bdd
                                        SaveAbsence(clientTomuss, gestionBdd, absence);
                                        //On ajoute l'absence au ClientTomuss
                                        clientTomuss.ListeAbsence.Add(absence);
                                    }
                                }
                            }
                        }
                    }
                    //Si c'est ue note
                    else if (node.InnerText.ToUpper().Contains(new[]
                    {
                        "/1",
                        "/2",
                        "/3",
                        "/4",
                        "/5",
                        "/6",
                        "/7",
                        "/8",
                        "/9",
                        "/10",
                        "/11",
                        "/12",
                        "/13",
                        "/14",
                        "/15",
                        "/16",
                        "/17",
                        "/18",
                        "/19",
                        "/20"
                    }))
                    {
                        if (node.InnerText.ToUpper().Contains(":"))
                        {
                            List<string> tab = node.InnerText.ToArray(':').ToList();
                            if (tab.IsNotEmpty())
                            {
                                //On récup l'ue
                                string nameUe = tab.GetFirstElement().Trim();
                                string note = tab.GetLastElement().Trim();
                                if (nameUe.IsNotNullOrEmpty() && note.IsNotNullOrEmpty())
                                {
                                    Ue ue = clientTomuss.ListeUe.FirstOrDefault(ue2 => ue2.Nom.Equals(nameUe));

                                    Note objNote;
                                    if (ue.IsNotNull())
                                    {
                                        objNote = new Note
                                        {
                                            Date = RDateTime.GetDateTime(),
                                            IdClient = clientTomuss.Id,
                                            IdUe = ue.Id,
                                            NoteClient = note
                                        };
                                        //On save la note en bdd
                                        SaveNote(clientTomuss, gestionBdd, objNote);
                                        //On ajoute le note au ClientTomuss
                                        ue.ListeNote.Add(objNote);
                                    }
                                    else
                                    {
                                        //On a pas trouvé l'ue, on l'ajout à la liste des ue du ClientTomuss et en bdd
                                        ue = AddUe(clientTomuss, new Ue
                                        {
                                            Nom = nameUe
                                        }, gestionBdd);
                                        objNote = new Note
                                        {
                                            Date = RDateTime.GetDateTime(),
                                            IdClient = clientTomuss.Id,
                                            IdUe = ue.Id,
                                            NoteClient = note
                                        };
                                        //On save la note en bdd
                                        SaveNote(clientTomuss, gestionBdd, objNote);
                                        //On ajoute la note au ClientTomuss
                                        ue.ListeNote.Add(objNote);
                                        clientTomuss.ListeUe.Add(ue);
                                    }
                                }
                            }
                        }
                    }

                    node = newitem.SelectSingleNode("./author");
                    if (node.IsNull()) continue;
                    reponse += "Auteur : " + node.InnerText + "\n";
                }

                return reponse;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public bool BeginThreads()
        {
            try
            {
                _continueThread = true;
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool FinishThreads()
        {
            try
            {
                _continueThread = false;
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private bool ClearContenuRss(ClientTomuss clientTomuss = null, RmySqlObject gestionBdd = null)
        {
            try
            {
                if (clientTomuss.IsNull() || gestionBdd.IsNull()) return false;
                lock (_lockBdd)
                {
                    clientTomuss.FluxRss.Contenu = null;
                    return gestionBdd.UpdateById(clientTomuss.FluxRss, clientTomuss.FluxRss.Id);
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        private bool WriteContenuRss(ClientTomuss clientTomuss = null, RmySqlObject gestionBdd = null, string contenu = null)
        {
            try
            {
                if (clientTomuss.IsNull() || gestionBdd.IsNull() || contenu.IsNullOrEmpty()) return false;
                lock (_lockBdd)
                {
                    clientTomuss.FluxRss.Contenu = contenu;
                    return gestionBdd.UpdateById(clientTomuss.FluxRss, clientTomuss.FluxRss.Id);
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        private static int GetTailleContenuRss(ClientTomuss clientTomuss = null)
        {
            try
            {
                return clientTomuss.IsNull()
                    ? 0
                    : (clientTomuss.FluxRss.Contenu.IsNullOrEmpty() ? 0 : clientTomuss.FluxRss.Contenu.Length);
            }
            catch (Exception)
            {
                return 0;
            }
        }

        private bool SavePresence(ClientTomuss clientTomuss = null, RmySqlObject gestionBdd = null, Presence presence = null)
        {
            try
            {
                if (clientTomuss.IsNull() || gestionBdd.IsNull() || presence.IsNull()) return false;
                lock (_lockBdd)
                {
                    return (gestionBdd.Add(presence) > 0);
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        private bool SaveAbsence(ClientTomuss clientTomuss = null, RmySqlObject gestionBdd = null, Absence absence = null)
        {
            try
            {
                if (clientTomuss.IsNull() || gestionBdd.IsNull() || absence.IsNull()) return false;
                lock (_lockBdd)
                {
                    return (gestionBdd.Add(absence) > 0);
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        private bool SaveNote(ClientTomuss clientTomuss = null, RmySqlObject gestionBdd = null, Note note = null)
        {
            try
            {
                if (clientTomuss.IsNull() || gestionBdd.IsNull() || note.IsNull()) return false;
                lock (_lockBdd)
                {
                    return (gestionBdd.Add(note) > 0);
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        private Ue AddUe(ClientTomuss clientTomuss = null, Ue ue = null, RmySqlObject gestionBdd = null)
        {
            try
            {
                if (clientTomuss.IsNull() || ue.IsNull() || gestionBdd.IsNull()) return null;
                lock (_lockBdd)
                {
                    //On regarde si on a pas déjà l'ue en bdd
                    List<Ue> x = gestionBdd.Select(new Ue(), new[]
                    {
                        new RField
                        {
                            Name = "Nom",
                            Operator = "=",
                            Value = ue.Nom
                        }
                    });
                    if (x.IsNotEmpty())
                    {
                        //On a trouvé l'ue en bdd, on l'ajout juste à l'utilisateur
                        int idclientue = gestionBdd.Add(new ClientUe
                        {
                            IdClient = clientTomuss.Id,
                            IdUe = x.GetFirstElement().Id
                        });
                        if (idclientue <= 0) return null;
                        ue.Id = x.GetFirstElement().Id;
                        clientTomuss.ListeUe.Add(ue);
                    }
                    else
                    {
                        //On a pas trouvé l'ue, il faut l'ajouter à la table et à l'utilisateur
                        int idue = gestionBdd.Add(new Ue
                        {
                            Nom = ue.Nom
                        });
                        if (idue <= 0) return null;
                        gestionBdd.Add(new ClientUe
                        {
                            IdClient = clientTomuss.Id,
                            IdUe = idue
                        });
                        ue.Id = idue;
                        clientTomuss.ListeUe.Add(ue);
                    }
                }
                return ue;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}