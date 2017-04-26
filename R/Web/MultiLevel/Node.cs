using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace R.Web.MultiLevel
{
    public class Node
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Vous devez saisir un nom")]
        [Display(Name = "Nom")]
        public string Name { get; set; }
        
        public virtual Node Parent { get; set; }

        [ForeignKey("Parent")]
        public int? IdParent { get; set; }

        public Node()
        {

        }

        public Node(int parent)
        {
            IdParent = parent;
        }
    }
}
