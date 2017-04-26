using System;

namespace R
{
    public class Presence
    {
        [Rsqlgnore(true)]
        public int Id { get; set; }

        [RsqlInt(true)]
        public int IdClient { get; set; }

        [RsqlInt(true)]
        public int IdUe { get; set; }

        [RsqlDateTime(true)]
        public DateTime Date { get; set; }
    }
}
