using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace R.Web.MultiLevel
{
    public class Element : Node
    {
        public Element()
        {

        }

        public Element(int parent) : base(parent)
        {

        }
    }
}
