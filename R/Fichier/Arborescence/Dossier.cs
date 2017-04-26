using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace R.Fichier.Arborescence
{
    public class Dossier : Base
    {
        public List<Base> listeFichier { get; set; }

        public Dossier()
        {
            listeFichier = new List<Base>();
        }
    }
}
