using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;

namespace R.Fichier.Arborescence
{
    public class Base
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public bool IsFile { get; set; }
    }
}
