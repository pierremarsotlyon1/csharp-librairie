using System.Collections.Generic;

namespace R
{
    public class Ue
    {
        [Rsqlgnore(true)]
        public int Id { get; set; }

        [RsqlText(true)]
        public string Nom { get; set; }

        [Rsqlgnore(true)]
        public List<Note> ListeNote { get; set; }

        public Ue()
        {
            ListeNote = new List<Note>();
        }
    }
}
