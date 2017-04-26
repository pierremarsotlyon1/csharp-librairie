namespace R
{
    public class ClientUe
    {
        [Rsqlgnore(true)]
        public int Id { get; set; }

        [RsqlInt(true)]
        public int IdUe { get; set; }

        [RsqlInt(true)]
        public int IdClient { get; set; }
    }
}
