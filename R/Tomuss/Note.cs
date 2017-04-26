using System;

namespace R
{
    public class Note
    {
        [Rsqlgnore(true)]
        public int Id { get; set; }

        [RsqlInt(true)]
        public int IdClient { get; set; }

        [RsqlInt(true)]
        public int IdUe { get; set; }

        [RsqlText(true)]
        public string NoteClient { get; set; }

        [RsqlDateTime(true)]
        public DateTime Date { get; set; }
    }
}
