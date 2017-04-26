namespace R
{
    public class XpathArticle
    {
        public string UrlBase { get; set; }
        public AttributesXpath[] Href { get; set; }
        public AttributesXpath[] Titre { get; set; }
        public AttributesXpath[] Image { get; set; }
        public ArrayDoubleDimension[] Params { get; set; }
        public AttributesXpath[] ContenuArticle { get; set; }
    }

    public class AttributesXpath
    {
        /// <summary>
        /// Le chemin xpath de l'élément
        /// </summary>
        public string Chemin { get; set; }

        /// <summary>
        /// Le type qui contient l'élément (href, src, ...)
        /// </summary>
        public string Attribute { get; set; }
    }

    /// <summary>
    /// Classe qui permet de faire passer des paramètres que l'on peut récupérer lor du traitement d'un article
    /// </summary>
    public class ArrayDoubleDimension
    {
        public string Key { get; set; }
        public object Value { get; set; }
    }
}
