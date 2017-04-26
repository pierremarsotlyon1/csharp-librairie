using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace R.PDF
{
    public class Pdf
    {
        private string CheminPdf;
        private Document dc;
        private FileStream fs;
        private PdfPTable tableau;
        private PdfPCell cellule;
        public Pdf(string chemin = null)
        {
            CheminPdf = HttpContext.Current.Server.MapPath(chemin);
            if (!RFichier.FileExist(CheminPdf))
                RFichier.CreerFichier(CheminPdf);
            dc = new Document();
            fs = new FileStream(CheminPdf, FileMode.Create);
        }

        public bool Open()
        {
            try
            {
                PdfWriter.GetInstance(dc, fs);
                dc.Open();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool Close()
        {
            try
            {
                dc.Close();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool AddParagraphe(string paragraphe = null, int indentationLeft = 0)
        {
            try
            {
                if (paragraphe.IsNullOrEmpty()) return false;

                var p = new Paragraph(paragraphe) {IndentationLeft = indentationLeft};
                dc.Add(p);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Permet de d'instancier un tableau
        /// </summary>
        /// <param name="nbColonnes">Nombre de colonnes du tableau</param>
        /// <returns>bool</returns>
        public bool BeginTable(int nbColonnes = 0)
        {
            try
            {
                if (nbColonnes.IsDefault()) return false;
                tableau = new PdfPTable(nbColonnes);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Permet d'ajouter le tableau dans le document
        /// </summary>
        /// <returns>bool</returns>
        public bool EndTable()
        {
            try
            {
                if (tableau.IsNull()) return false;
                dc.Add(tableau);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool AddCell(string p = null, int colspan = 1, bool alignCenter = false)
        {
            try
            {
                cellule = p.IsNullOrEmpty() ? new PdfPCell() : new PdfPCell(new Phrase(p));
                cellule.Colspan = colspan;
                cellule.HorizontalAlignment = alignCenter ? Element.ALIGN_CENTER : Element.ALIGN_LEFT;
                cellule.Padding = 5;
                tableau.AddCell(cellule);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Permet de faire un saut de ligne
        /// </summary>
        /// <param name="nbSaut"></param>
        /// <returns>bool</returns>
        public bool SautDeLigne(int nbSaut = 1)
        {
            try
            {
                if (nbSaut == 1)
                {
                    dc.Add(new Phrase("\n"));
                    return true;
                }
                int i;
                for(i = 0; i < nbSaut; i++)
                    dc.Add(new Phrase("\n"));
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
