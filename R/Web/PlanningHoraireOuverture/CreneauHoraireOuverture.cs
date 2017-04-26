using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace R.Web.PlanningHoraireOuverture
{
    public class CreneauHoraireOuverture : Creneau
    {
        [Key]
        public int Id { get; set; }
    }
}
