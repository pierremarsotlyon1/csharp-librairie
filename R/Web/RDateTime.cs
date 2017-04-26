using System;

namespace R
{
    public static class RDateTime
    {
        public static string ToFrench(this DateTime datetime)
        {
            try
            {
                return datetime.ToString("dd/MM/yyyy");
            }
            catch (Exception)
            {
                return null;
            }
        }
        
        /// <summary>
        /// Obtient le DateTime courant
        /// </summary>
        /// <returns>DateTime</returns>
        public static DateTime GetDateTime()
        {
            return DateTime.Now;
        }

        /// <summary>
        /// Permet de récupérer la date du jour
        /// </summary>
        /// <returns>string</returns>
        public static string GetDateDay()
        {
            return DateTime.Now.ToShortDateString();
        }

        /// <summary>
        /// Obtient le jour de la semaine courante
        /// </summary>
        /// <returns>DayOfWeek</returns>
        public static int GetJourSemaine()
        {
            return (int) DateTime.Now.Date.DayOfWeek;
        }

        /// <summary>
        /// Obtient le jour du mois
        /// </summary>
        /// <returns>int</returns>
        public static int GetJourMois()
        {
            return DateTime.Now.Date.Day;
        }

        /// <summary>
        /// Obtient le jour de l'année
        /// </summary>
        /// <returns>int</returns>
        public static int GetJourAnnee()
        {
            return DateTime.Now.Date.DayOfYear;
        }

        /// <summary>
        /// Obtient le mois courant
        /// </summary>
        /// <returns>int</returns>
        public static int GetMois()
        {
            return DateTime.Now.Date.Month;
        }

        /// <summary>
        /// Obtient l'année courante
        /// </summary>
        /// <returns>int</returns>
        public static int GetAnnee()
        {
            return DateTime.Now.Date.Year;
        }

        /// <summary>
        /// Obtient l'heure courante
        /// </summary>
        /// <returns>int</returns>
        public static int GetHeure()
        {
            return DateTime.Now.Hour;
        }

        /// <summary>
        /// Obtient la minute courante
        /// </summary>
        /// <returns>int</returns>
        public static int GetMinute()
        {
            return DateTime.Now.Minute;
        }

        /// <summary>
        /// Obtient le seconde courante
        /// </summary>
        /// <returns>int</returns>
        public static int GetSeconde()
        {
            return DateTime.Now.Second;
        }

        /// <summary>
        /// Obtient les millisecondes courantes
        /// </summary>
        /// <returns>int</returns>
        public static int GetMillisecond()
        {
            return DateTime.Now.Millisecond;
        }

        /// <summary>
        /// Obtient l'heure compléte du jour courant
        /// </summary>
        /// <returns>int</returns>
        public static string GetTimeJour()
        {
            return GetDateTime().ToString("HH:mm:ss");
        }

        /// <summary>
        /// Permet de récupérer l'heure complete avec formatage
        /// </summary>
        /// <returns>string</returns>
        public static string GetTime()
        {
            return "Il est " + GetHeure() + " heures " + GetMinute() + " minutes et " + GetSeconde() + " secondes";
        }

        /// <summary>
        /// Permet d'ajouter un nombre de jour à un DateTime
        /// </summary>
        /// <param name="o"></param>
        /// <param name="day">Nombre de jour</param>
        /// <returns>DateTime</returns>
        public static DateTime AddDay(this DateTime o, int day = 0)
        {
            try
            {
                var duration = new TimeSpan(day, 0, 0, 0);
                return o.Add(duration);
            }
            catch (Exception)
            {
                return o;
            }
        }

        /// <summary>
        /// Converti un int jour en string
        /// </summary>
        /// <param name="obj">int</param>
        /// <returns>string</returns>
        public static string JourSemaineToString(this int obj)
        {
            switch (obj)
            {
                case 1 :
                    return "Lundi";
                case 2:
                    return "Mardi";
                case 3:
                    return "Mercredi";
                case 4:
                    return "Jeudi";
                case 5:
                    return "Vendredi";
                case 6:
                    return "Samedi";
                case 7:
                    return "Dimanche";
                default:
                    return null;
            }
        }

        /// <summary>
        /// Converti un string jour en int
        /// </summary>
        /// <param name="obj">string</param>
        /// <returns>int</returns>
        public static int JourSemaineToInt(this string obj)
        {
            switch (obj)
            {
                case "Lundi":
                    return 1;
                case "Mardi":
                    return 2;
                case "Mercredi":
                    return 3;
                case "Jeudi":
                    return 4;
                case "Vendredi":
                    return 5;
                case "Samedi":
                    return 6;
                case "Dimanche":
                    return 7;
                default:
                    return 0;
            }
        }

        /// <summary>
        /// Converti un string jour en int
        /// </summary>
        /// <param name="obj">string</param>
        /// <returns>int</returns>
        public static int JourSemaineEnglishToInt(this string obj)
        {
            try
            {
                if (obj.IsNullOrEmpty()) return -1;
                obj = obj.ToLower();
                switch (obj)
                {
                    case "monday":
                        return 1;
                    case "tuesday":
                        return 2;
                    case "wednesday":
                        return 3;
                    case "thursday":
                        return 4;
                    case "friday":
                        return 5;
                    case "saturday":
                        return 6;
                    case "sunday":
                        return 7;
                    default:
                        return -1;
                }
            }
            catch (Exception)
            {
                return -1;
            }
        }

        /// <summary>
        /// Converti un int mois en string
        /// </summary>
        /// <param name="mois">int</param>
        /// <returns>string</returns>
        public static string MoisToString(this int mois)
        {
            switch (mois)
            {
                case 1:
                    return "Janvier";
                case 2:
                    return "Février";
                case 3:
                    return "Mars";
                case 4:
                    return "Avril";
                case 5:
                    return "Mai";
                case 6:
                    return "Juin";
                case 7:
                    return "Juillet";
                case 8:
                    return "Août";
                case 9:
                    return "Septembre";
                case 10:
                    return "Octobre";
                case 11:
                    return "Novembre";
                case 12:
                    return "Décembre";
                default:
                    return null;
            }
        }

        /// <summary>
        /// Converti un string mois en int
        /// </summary>
        /// <param name="mois">string</param>
        /// <returns>int</returns>
        public static int MoisToInt(this string mois)
        {
            switch (mois)
            {
                case "Janvier":
                    return 1;
                case "Février":
                    return 2;
                case "Mars":
                    return 3;
                case "Avril":
                    return 4;
                case "Mai":
                    return 5;
                case "Juin":
                    return 6;
                case "Juillet":
                    return 7;
                case "Août":
                    return 8;
                case "Septembre":
                    return 9;
                case "Octobre":
                    return 10;
                case "Novembre":
                    return 11;
                case "Décembre":
                    return 12;
                default:
                    return 0;
            }
        }

        /// <summary>
        /// Convertir un DateTime en string
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns>string</returns>
        public static string DateTimeToString(this DateTime dateTime)
        {
            return dateTime.ToString("dd-MM-yyyy HH:mm:ss");
        }

        /// <summary>
        /// Convertit un string en DateTime
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns>DateTime</returns>
        public static DateTime StringToDateTime(this string dateTime)
        {
            try
            {
                return Convert.ToDateTime(dateTime);
            }
            catch (Exception)
            {
                return new DateTime();
            }
        }

        /// <summary>
        /// Compare deux DateTime
        /// </summary>
        /// <param name="o"></param>
        /// <param name="comparer">DateTime à comparer</param>
        /// <returns>int</returns>
        public static int Compare(this DateTime o, DateTime comparer)
        {
            try
            {
                int result = DateTime.Compare(o, comparer);
                return result;
            }
            catch (Exception)
            {
                return -10;
            }
        }

        public static int GetAge(this DateTime depart)
        {
            try
            {
                return (int)(DateTime.Now - depart).TotalDays / 365;
            }
            catch (Exception)
            {
                return -1;
            }
        }

        public static int GetAge(this DateTime depart, DateTime arrive)
        {
            try
            {
                if(arrive.IsNull()) return 1;
                
                return (int)(DateTime.Now - arrive).TotalDays / 365;
            }
            catch (Exception)
            {
                return -1;
            }
        }

        public static DateTime ParseDateTime(this string dt)
        {
            try
            {
                return DateTime.Parse(dt);
            }
            catch (Exception)
            {
                return new DateTime();
            }
        }

        public static bool IsNull(this DateTime dt)
        {
            try
            {
                return dt == default(DateTime);
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
