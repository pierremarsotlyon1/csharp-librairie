namespace R
{
    public class FluxRss
    {
        [Rsqlgnore(true)]
        public int Id { get; set; }

        [RsqlInt(true)]
        public int IdClient { get; set; }

        [RsqlText(true)]
        public string Lien { get; set; }

        [RsqlText(true)]
        public string Contenu { get; set; }
    }
}
