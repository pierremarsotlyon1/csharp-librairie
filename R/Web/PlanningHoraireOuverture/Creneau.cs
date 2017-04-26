using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using R;
using System.ComponentModel;

namespace R.Web.PlanningHoraireOuverture
{
    public class Creneau
    {
        private TimeSpan _debut;

        [Required]
        public TimeSpan Debut {
            get
            {
                return _debut;
            }
            set
            {
                _debut = value;
                CheckIfIsFerme();
            }
        }

        private TimeSpan _fin;

        [Required]
        public TimeSpan Fin
        {
            get
            {
                return _fin;
            }
            set
            {
                _fin = value;
                CheckIfIsFerme();
            }
        }

        [Required]
        [DefaultValue(false)]
        public bool IsFerme { get; set; }

        public Creneau()
        {
            Init();
        }

        public Creneau(string heuredebut = null, string heurefin = null)
        {
            Init();

            if (heuredebut.IsNullOrEmpty() || heurefin.IsNullOrEmpty()) return;

            if (heuredebut.Contains(":").IsFalse() || heurefin.Contains(":").IsFalse()) return;

            var tab = heuredebut.Split(':');
            if (tab.Length != 3) return;

            Debut = new TimeSpan(tab.First().ToInt(), tab.Last().ToInt(), 0);

            tab = heurefin.Split(':');
            if (tab.Length != 3) return;
            Fin = new TimeSpan(tab.First().ToInt(), tab.Last().ToInt(), 0);
        }

        private void Init()
        {
            Debut = new TimeSpan();
            Fin = new TimeSpan();
        }

        public bool InitialisationCreneau()
        {
            try
            {
                Debut = new TimeSpan(0, 0, 0);
                Fin = new TimeSpan(0, 0, 0);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Permet de savoir si le creneau est fermé ou pas
        /// </summary>
        /// <returns>bool</returns>
        public void CheckIfIsFerme()
        {
            try
            {
                var timeSpanCompare = new TimeSpan(0, 0, 0);

                IsFerme = TimeSpan.Compare(Debut, timeSpanCompare) == 0 && TimeSpan.Compare(Fin, timeSpanCompare) == 0;
            }
            catch(Exception)
            {
              
            }
        }

        public bool IsDefault()
        {
            try
            {
                return Debut.Equals(new TimeSpan(0, 0, 0)) && Fin.Equals(new TimeSpan(0, 0, 0));
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool IsNotDefault()
        {
            try
            {
                return !IsDefault();
            }
            catch (Exception)
            {
                return false;
            }
        }

        public string GetHeureOuverture()
        {
            return Debut.ToFrench();
        }

        public string GetHeureFermeture()
        {
            return Fin.ToFrench();
        }

        public string GetHeureTotal()
        {
            return GetHeureOuverture() + "-" + GetHeureFermeture();
        }

        /// <summary>
        /// Permet d'ajouter un temps spécifique au début et fin du créneau
        /// </summary>
        /// <param name="time">Le temps à ajouter</param>
        /// <returns>bool</returns>
        public bool AddTimeBeginAndEnd(TimeSpan time)
        {
            try
            {
                if (time.IsNull() || Debut.IsNull() || Fin.IsNull() || IsDefault()) return false;

                Debut = Debut.Add(-time);
                if (Debut.IsNull()) return false;

                Fin = Fin.Add(time);
                if (Fin.IsNull()) return false;

                return true;
            }
            catch(Exception)
            {
                return false;
            }
        }

        public bool Contains(TimeSpan? time)
        {
            try
            {
                if (time.IsNull() || Debut.IsNull() || Fin.IsNull() || IsDefault()) return false;

                if (Debut <= time && Fin >= time)
                    return true;

                return false;
            }
            catch(Exception)
            {
                return false;
            }
        }

        public bool ContainsStrict(TimeSpan? time)
        {
            try
            {
                if (time.IsNull() || Debut.IsNull() || Fin.IsNull() || IsDefault()) return false;

                if (Debut < time && Fin > time)
                    return true;

                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}