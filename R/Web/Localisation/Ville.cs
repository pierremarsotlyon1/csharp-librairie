using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace R.Web.Localisation
{
    public class Ville
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Nom { get; set; }
    }
}
