using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Threading.Tasks;
using R;
using System.Collections.Generic;

namespace R.Web.PlanningHoraireOuverture
{
    public class Disponibilite
    {
        [Key]
        public int DisponibiliteId { get; set; }

        [ForeignKey("IdCreneauMatin")]
        public virtual CreneauHoraireOuverture CreneauMatin { get; set; }

        [Required]
        public int IdCreneauMatin { get; set; }

        [ForeignKey("IdCreneauAprem")]
        public virtual CreneauHoraireOuverture CreneauAprem { get; set; }

        [Required]
        public int IdCreneauAprem { get; set; }

        public Disponibilite()
        {
            //CreneauMatin = new Creneau();
            //CreneauAprem = new Creneau();
        }

        /// <summary>
        /// Permet de savoir si un creneau est dans les heures de dispo matin ou aprem
        /// </summary>
        /// <param name="compare">Le creneau à comparer</param>
        /// <returns>bool</returns>
        public bool Contains(Creneau compare = null)
        {
            try
            {
                if (compare.IsNull() || CreneauAprem.IsNull() || CreneauMatin.IsNull()) return false;

                if (CreneauMatin.IsNotDefault())
                {
                    if (CreneauMatin.Debut <= compare.Debut && CreneauMatin.Fin >= compare.Fin)
                        return true;
                }

                if (CreneauAprem.IsNotDefault())
                {
                    if (CreneauAprem.Debut <= compare.Debut && CreneauAprem.Fin >= compare.Fin)
                        return true;
                }

                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool InitialisationDisponibilite()
        {
            try
            {
                CreneauMatin = new CreneauHoraireOuverture();
                CreneauAprem = new CreneauHoraireOuverture();

                return CreneauMatin.InitialisationCreneau() && CreneauAprem.InitialisationCreneau();
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Permet de générer le string d'affichage d'une disponibilite, il faut que les creneaux soient chargés
        /// </summary>
        /// <returns>string</returns>
        public override string ToString()
        {
            try
            {
                if (CreneauMatin.IsNull() || CreneauAprem.IsNull()) return null;

                //On regarde si le jour est fermé
                if(IsFerme())
                {
                    return "<span class='text-danger'>Fermé</span>";
                }

                //Si le matin est fermé
                if(CreneauMatin.IsFerme)
                {
                    //On affiche les horaires de l'aprem
                    return CreneauAprem.GetHeureTotal();
                }

                //Si l'aprem est fermé
                if(CreneauAprem.IsFerme)
                {
                    //On affiche les horaires du matin
                    return CreneauMatin.GetHeureTotal();
                }

                //Si l'heure de fermeture matin est égale à l'heure d'ouverture aprem, on affiche les deux extremités de l'ensemble
                if(TimeSpan.Compare(CreneauMatin.Fin, CreneauAprem.Debut) == 0)
                {
                    return CreneauMatin.GetHeureOuverture() + " - " + CreneauAprem.GetHeureFermeture();
                }
                //Sinon on affiche tout
                else
                {
                    return CreneauMatin.GetHeureTotal() + " - " + CreneauAprem.GetHeureTotal();
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Permet de savoir si la journée est fermée
        /// </summary>
        /// <returns>bool</returns>
        public bool IsFerme()
        {
            try
            {
                if (CreneauMatin.IsNull() || CreneauAprem.IsNull()) return false;

                return CreneauMatin.IsFerme && CreneauAprem.IsFerme;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Permet de récupérer un ensemble de créneau des horaires disponibles
        /// </summary>
        /// <returns></returns>
        public List<Creneau> GetDisponibilite()
        {
            try
            {
                var response = new List<Creneau>();

                if (CreneauMatin.IsNull() || CreneauAprem.IsNull() || (CreneauMatin.IsFerme && CreneauAprem.IsFerme)) return response;

                if(CreneauMatin.IsFerme.IsFalse() && CreneauAprem.IsFerme)
                {
                    response.Add(CreneauMatin);
                }
                else if (CreneauAprem.IsFerme.IsFalse() && CreneauMatin.IsFerme)
                {
                    response.Add(CreneauAprem);
                }
                else if(CreneauMatin.Fin.Equals(CreneauAprem.Debut))
                {
                    response.Add(new Creneau
                    {
                        Debut = CreneauMatin.Debut,
                        Fin = CreneauAprem.Fin,
                    });
                }
                else
                {
                    response.Add(CreneauMatin);
                    response.Add(CreneauAprem);
                }

                return response;
            }
            catch(Exception)
            {
                return new List<Creneau>();
            }
        }
    }
}