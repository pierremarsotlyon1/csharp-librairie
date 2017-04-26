using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using R.Web;

namespace R
{
    public class RUser
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Email { get; set; }
        
        private string _password;

        [Required]
        public string Nom { get; set; }

        [Required]
        public string Prenom { get; set; }

        [Required]
        public string Genre { get; set; }

        [Required]
        public string Password
        {
            get { return _password; }
            set { _password = value.ToMd5(); }
        }
    }
}
