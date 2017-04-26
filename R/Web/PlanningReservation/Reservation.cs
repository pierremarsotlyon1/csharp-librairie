﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace R.Web.PlanningReservation
{
    public class Reservation
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("IdJour")]
        public virtual Jour Jour { get; set; }

        [Required]
        public int IdJour { get; set; }

        [Required]
        public virtual CreneauReservation Creneau { get; set; }
    }
}
