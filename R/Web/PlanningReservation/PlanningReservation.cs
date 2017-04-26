using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace R.Web.PlanningReservation
{
    public class PlanningReservation
    {
        [Key]
        public int Id { get; set; }

        public virtual List<Jour> Jours { get;  set;}
    }
}
