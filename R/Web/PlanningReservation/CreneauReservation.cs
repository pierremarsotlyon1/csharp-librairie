using R.Web.PlanningHoraireOuverture;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace R.Web.PlanningReservation
{
    public class CreneauReservation : Creneau
    {
        [Key, ForeignKey("Reservation")]
        public int IdCreneau { get; set; }

        public virtual Reservation Reservation { get; set; }
    }
}
