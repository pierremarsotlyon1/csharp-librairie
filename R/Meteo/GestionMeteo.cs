using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace R
{
    public class GestionMeteo
    {
        public string GetConditionOneCity(string city)
        {
            var conditions = GetCurrentConditions(city);

            return conditions != null
                ? "Voici les conditions météorologique pour la ville de " + conditions.City + ". La température est de " + conditions.Temperature + " degrés Celsius. L'humidité est de " + conditions.Humidity + " pourcents. La vitesse du vent est de" + conditions.Wind + " Kilomètre par heure"
                : "Une erreur c'est produite lors de la récupération des informations météorologiques de la ville voulue";
        }

        private ConditionMeteo GetCurrentConditions(string location)
        {
            var conditions = new ConditionMeteo();

            var xmlConditions = new XmlDocument();
            xmlConditions.Load("http://weather.yahooapis.com/forecastrss?w=12667038");

            if (xmlConditions.SelectSingleNode("xml_api_reply/weather/problem_cause") != null)
            {
                conditions = null;
            }
            else
            {
                GetConditionCity(xmlConditions, ref conditions);
            }

            return conditions;
        }

        private static void GetConditionCity(XmlDocument xmlConditions, ref ConditionMeteo condition)
        {
            try
            {
                var selectSingleNode = xmlConditions.SelectSingleNode("rss/channel");
                if (selectSingleNode != null)
                    if (selectSingleNode.Attributes != null)
                    {
                        var xmlAttributeCollection = selectSingleNode.ChildNodes[6].Attributes;
                        if (xmlAttributeCollection != null)
                            condition.City = xmlAttributeCollection["city"].Value;
                    }

                var xmlNode = xmlConditions.SelectSingleNode("rss/channel/item");
                if (xmlNode != null)
                    if (xmlNode.Attributes != null)
                    {
                        var xmlAttributeCollection = xmlNode.ChildNodes[5].Attributes;
                        if (xmlAttributeCollection != null)
                            condition.Temperature = xmlAttributeCollection["temp"].Value.ToDegres();
                    }
                var selectSingleNode1 =
                    xmlConditions.SelectSingleNode("rss/channel");
                if (selectSingleNode1 != null)
                    if (selectSingleNode1.Attributes != null)
                    {
                        var xmlAttributeCollection = selectSingleNode1.ChildNodes[9].Attributes;
                        if (xmlAttributeCollection != null)
                            condition.Humidity = xmlAttributeCollection["humidity"].Value;
                    }
                var singleNode1 =
                    xmlConditions.SelectSingleNode("rss/channel");
                if (singleNode1 == null) return;
                if (singleNode1.Attributes != null)
                {
                    var xmlAttributeCollection = singleNode1.ChildNodes[8].Attributes;
                    if (xmlAttributeCollection != null)
                        condition.Wind = xmlAttributeCollection["speed"].Value;
                }
            }
            // ReSharper disable once EmptyGeneralCatchClause
            catch (Exception)
            {
            }
        }
    }
}
