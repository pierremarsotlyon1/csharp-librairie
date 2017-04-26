using Gma.QrCodeNet.Encoding;
using Gma.QrCodeNet.Encoding.Windows.Render;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace R.Web.GestionQrCode
{
    public class GestionQrCode
    {
        public GestionQrCode()
        {

        }

        public string GenerateImageQrCode(string value = null, string path = null, int width = 50, int height = 50)
        {
            try
            {
                if (value.IsNullOrEmpty() || path.IsNullOrEmpty()) return null;
                
                //On génére le nom de l'image
                var nameImage = Guid.NewGuid() + ".png";
                
                //On récup le chemin de sauvegarde
                var originalDirectory = new DirectoryInfo(string.Format("{0}" + path, HttpContext.Current.Server.MapPath(@"\"))).ToString();
                if (originalDirectory.IsNullOrEmpty())
                {
                    return null;
                }

                //On regarde si le chemin de sauvegarde existe
                bool isExists = System.IO.Directory.Exists(originalDirectory);
                if (!isExists)
                    System.IO.Directory.CreateDirectory(originalDirectory);

                //On génére le chemin complet de sauvegarde de l'image
                var pathComplet = string.Format("{0}\\{1}", originalDirectory, nameImage);
                if (pathComplet.IsNullOrEmpty())
                {
                    return null;
                }
                
                QrEncoder qrEncoder = new QrEncoder(ErrorCorrectionLevel.H);
                QrCode qrCode = new QrCode();
                qrEncoder.TryEncode(value, out qrCode);

                GraphicsRenderer renderer = new GraphicsRenderer(
                    new FixedCodeSize(400, QuietZoneModules.Zero),
                    Brushes.Black,
                    Brushes.White);
                MemoryStream ms = new MemoryStream();
                renderer.WriteToStream(qrCode.Matrix, ImageFormat.Png, ms);
                var imageTemp = new Bitmap(ms);
                var image = new Bitmap(imageTemp, new Size(new Point(width, height)));
                image.Save(pathComplet, ImageFormat.Png);

                return "~/" + path + "/" + nameImage;
            }
            catch(Exception)
            {
                return null;
            }
        }
    }
}
