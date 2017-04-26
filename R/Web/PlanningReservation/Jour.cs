using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace R.Web.PlanningReservation
{
    public class Jour
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("IdPlanningReservation")]
        public virtual PlanningReservation PlanningReservation { get; set; }

        [Required]
        public int IdPlanningReservation { get; set; }

        [Required]
        public DateTime Date { get; set; }

        public int TempsMax { get; set; }

        public virtual List<Reservation> Reservations { get; set; }

        public virtual List<CreneauReservationLibre> CreneauReservationLibres { get; set; }
    }
}
