using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace R.Web.MultiLevel
{
    public class Category : Node
    {
        public Category()
        {
            Nodes = new List<Node>();
        }

        public Category(int parent) : base(parent)
        {
            Nodes = new List<Node>();
        }

        public List<Node> Nodes { get; set; }
    }
}
