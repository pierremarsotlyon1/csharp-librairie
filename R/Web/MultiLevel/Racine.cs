using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace R.Web.MultiLevel
{
    public class Racine
    {
        public List<Node> Nodes { get; set; }

        public Racine()
        {
            Nodes = new List<Node>();
        }
    }
}
