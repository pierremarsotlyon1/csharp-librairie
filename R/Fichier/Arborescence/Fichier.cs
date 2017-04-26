using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace R.Fichier.Arborescence
{
    public class Fichier : Base
    {
        public string Extension { get; set; }
        public byte[] ByteFile { get; set; }

        public Fichier()
        {
            ByteFile = new byte[0];
        }
    }
}
