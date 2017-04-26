using System.Collections.Generic;

namespace R
{
    public class ClientTomuss
    {
        [Rsqlgnore(true)]
        public int Id { get; set; }

        [RsqlText(true)]
        public string Nom { get; set; }

        [RsqlText(true)]
        public string Prenom { get; set; }

        [RsqlText(true)]
        public string Email { get; set; }

        [RsqlText(true)]
        public string Password { get; set; }

        [Rsqlgnore(true)]
        public List<Ue> ListeUe { get; set; }

        [Rsqlgnore(true)]
        public List<Presence> ListePresence { get; set; }

        [Rsqlgnore(true)]
        public List<Absence> ListeAbsence { get; set; }

        [Rsqlgnore(true)]
        public FluxRss FluxRss { get; set; }

        public ClientTomuss()
        {
            ListeUe = new List<Ue>();
            ListePresence = new List<Presence>();
            ListeAbsence = new List<Absence>();
        }
    }
}
