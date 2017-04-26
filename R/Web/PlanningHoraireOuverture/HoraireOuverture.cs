using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace R.Web.PlanningHoraireOuverture
{
    public abstract class HoraireOuverture<T> where T : DbContext
    {
        [ForeignKey("IdLundi")]
        public virtual Disponibilite Lundi { get; set; }

        [Required]
        public int IdLundi { get; set; }

        [ForeignKey("IdMardi")]
        public virtual Disponibilite Mardi { get; set; }

        [Required]
        public int IdMardi { get; set; }

        [ForeignKey("IdMercredi")]
        public virtual Disponibilite Mercredi { get; set; }

        [Required]
        public int IdMercredi { get; set; }

        [ForeignKey("IdJeudi")]
        public virtual Disponibilite Jeudi { get; set; }

        [Required]
        public int IdJeudi { get; set; }

        [ForeignKey("IdVendredi")]
        public virtual Disponibilite Vendredi { get; set; }

        [Required]
        public int IdVendredi { get; set; }

        [ForeignKey("IdSamedi")]
        public virtual Disponibilite Samedi { get; set; }

        [Required]
        public int IdSamedi { get; set; }

        [ForeignKey("IdDimanche")]
        public virtual Disponibilite Dimanche { get; set; }

        [Required]
        public int IdDimanche { get; set; }

        /// <summary>
        /// Retourne une liste des jours de la semaine
        /// </summary>
        /// <returns></returns>
        public List<Disponibilite> GetList()
        {
            try
            {
                return new List<Disponibilite>
                {
                    Lundi,
                    Mardi, 
                    Mercredi,
                    Jeudi,
                    Vendredi,
                    Samedi,
                    Dimanche
                };
            }
            catch (Exception)
            {
                return new List<Disponibilite>();
            }
        }

        /// <summary>
        /// Retourne un dictionnaire des jours de la semaine
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, Disponibilite> GetDictionary()
        {
            try
            {
                return new Dictionary<string, Disponibilite>
                {
                    {"Lundi", Lundi},
                    {"Mardi", Mardi},
                    {"Mercredi", Mercredi},
                    {"Jeudi", Jeudi},
                    {"Vendredi", Vendredi},
                    {"Samedi", Samedi},
                    {"Dimanche", Dimanche}
                };
            }
            catch (Exception)
            {
                return new Dictionary<string, Disponibilite>();
            }
        }

        
        public bool InitialisationHoraireOuverture()
        {
            try
            {
                Lundi = new Disponibilite();
                if (Lundi.InitialisationDisponibilite().IsFalse()) return false;

                Mardi = new Disponibilite();
                if (Mardi.InitialisationDisponibilite().IsFalse()) return false;

                Mercredi = new Disponibilite();
                if (Mercredi.InitialisationDisponibilite().IsFalse()) return false;

                Jeudi = new Disponibilite();
                if (Jeudi.InitialisationDisponibilite().IsFalse()) return false;

                Vendredi = new Disponibilite();
                if (Vendredi.InitialisationDisponibilite().IsFalse()) return false;

                Samedi = new Disponibilite();
                if (Samedi.InitialisationDisponibilite().IsFalse()) return false;

                Dimanche = new Disponibilite();
                if (Dimanche.InitialisationDisponibilite().IsFalse()) return false;

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public Disponibilite GetDisponibiliteByDayOfWeek(DayOfWeek dayOfWeek)
        {
            try
            {
                switch (dayOfWeek)
                {
                    case DayOfWeek.Monday:
                        return Lundi;
                    case DayOfWeek.Tuesday:
                        return Mardi;
                    case DayOfWeek.Wednesday:
                        return Mercredi;
                    case DayOfWeek.Thursday:
                        return Jeudi;
                    case DayOfWeek.Friday:
                        return Vendredi;
                    case DayOfWeek.Saturday:
                        return Samedi;
                    case DayOfWeek.Sunday:
                        return Dimanche;
                }

                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public bool Load(T bdd)
        {
            try
            {
                if (bdd.IsNull()) return false;

                var listeTask = new[]
                {
                    bdd.Entry(this).Reference(t => t.Lundi).LoadAsync(),
                    bdd.Entry(this).Reference(t => t.Mardi).LoadAsync(),
                    bdd.Entry(this).Reference(t => t.Mercredi).LoadAsync(),
                    bdd.Entry(this).Reference(t => t.Jeudi).LoadAsync(),
                    bdd.Entry(this).Reference(t => t.Vendredi).LoadAsync(),
                    bdd.Entry(this).Reference(t => t.Samedi).LoadAsync(),
                    bdd.Entry(this).Reference(t => t.Dimanche).LoadAsync()
                };
                Task.WaitAll(listeTask);

                var listeTaskCreneau = new[]
                {
                    bdd.Entry(Lundi).Reference(t => t.CreneauMatin).LoadAsync(),
                    bdd.Entry(Lundi).Reference(t => t.CreneauAprem).LoadAsync(),
                    bdd.Entry(Mardi).Reference(t => t.CreneauMatin).LoadAsync(),
                    bdd.Entry(Mardi).Reference(t => t.CreneauAprem).LoadAsync(),
                    bdd.Entry(Mercredi).Reference(t => t.CreneauMatin).LoadAsync(),
                    bdd.Entry(Mercredi).Reference(t => t.CreneauAprem).LoadAsync(),
                    bdd.Entry(Jeudi).Reference(t => t.CreneauMatin).LoadAsync(),
                    bdd.Entry(Jeudi).Reference(t => t.CreneauAprem).LoadAsync(),
                    bdd.Entry(Vendredi).Reference(t => t.CreneauMatin).LoadAsync(),
                    bdd.Entry(Vendredi).Reference(t => t.CreneauAprem).LoadAsync(),
                    bdd.Entry(Samedi).Reference(t => t.CreneauMatin).LoadAsync(),
                    bdd.Entry(Samedi).Reference(t => t.CreneauAprem).LoadAsync(),
                    bdd.Entry(Dimanche).Reference(t => t.CreneauMatin).LoadAsync(),
                    bdd.Entry(Dimanche).Reference(t => t.CreneauAprem).LoadAsync()
                };
                
                Task.WaitAll(listeTaskCreneau);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Permet de supprimer les entités en bdd
        /// </summary>
        /// <param name="bdd">L'objet de BDD</param>
        /// <returns>bool</returns>
        public bool Free(T bdd = null)
        {
            try
            {
                if (bdd.IsNull()) return false;

                //On récup les jours de la semaine sous la forme d'une liste
                var semaine = GetList();
                if (semaine.IsEmpty()) return false;

                //On parcourt la liste des jours de la semaine
                foreach (var jour in semaine)
                {
                    //Si jour == null, on passe au prochaine
                    if (jour.IsNull()) continue;

                    //Si le créneau à une référence, on le marque comme supprimé
                    if (jour.CreneauMatin.IsNotNull())
                        bdd.Entry(jour.CreneauMatin).State = EntityState.Deleted;

                    //Si le créneau à une référence, on le marque comme supprimé
                    if (jour.CreneauAprem.IsNotNull())
                        bdd.Entry(jour.CreneauAprem).State = EntityState.Deleted;

                    bdd.Entry(jour).State = EntityState.Deleted;
                }

                //On supprime l'objet courant (HoraireOuvertureExtends)
                bdd.Entry(this).State = EntityState.Deleted;

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Permet de créer un string d'affichage sous forme de <table>, il faut que la semaine soit chargée
        /// </summary>
        /// <returns>string</returns>
        public override string ToString()
        {
            try
            {
                //On get la semaine
                var semaine = GetDictionary();
                if (semaine.IsEmpty()) return null;

                //On regarde si tous les jours sont chargés
                var check = semaine.All(j => !j.Value.IsNull() && !j.Value.CreneauMatin.IsNull() && !j.Value.CreneauAprem.IsNull());
                if (check.IsFalse()) return null;

                var response = "<table class='opening-hours'><tbody>";

                foreach (var jour in semaine)
                {
                    response += "<tr>";

                    response += "<th>"+jour.Key+"</th>";

                    response += "<td class='disponibilite_day'>";
                    response += jour.Value.ToString();
                    response += "</td>";

                    response += "</tr>";
                }

                response += "</tbody></table>";
                return response;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
