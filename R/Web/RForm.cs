using ImageResizer;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;

namespace R.Web
{
    public class RForm
    {
        private static readonly List<string> ExtensionValide = new List<string> { "jpg", "jpeg", "png", "gif" };
        private readonly MethodForm _method;

        public RForm(MethodForm type = MethodForm.Post)
        {
            _method = type;
        }

        public enum MethodForm
        {
            Post = 1,
            Get = 2
        }

        /// <summary>
        /// Permet de récupérer une valeur
        /// </summary>
        /// <param name="key">La clef de la variable</param>
        /// <returns>string</returns>
        public string Get(string key = null)
        {
            try
            {
                if (key.IsNullOrEmpty()) return null;
                var allvariables = _method == MethodForm.Post ? AllVariablesPost() : AllVariablesGet();
                if (allvariables.IsNull()) return null;

                //On regarde si la clef demandé existe
                var check = allvariables.AllKeys.Any(keys => keys.Equals(key));

                if (!check)
                    return null;

                if (allvariables[key].IsNullOrEmpty())
                    return null;

                return allvariables[key];
            }
            catch (Exception)
            {
                return null;
            }
        }

        public string DownloadImage(string path = null, HttpPostedFileBase image = null, int width = 0, int height = 0)
        {
            try
            {
                if (image == null || image.ContentLength < 1) return null;

                //On récup l'extension de l'image
                var fileExt = System.IO.Path.GetExtension(image.FileName);
                if (fileExt.IsNullOrEmpty())
                {
                    return null;
                }

                var extension = fileExt.Substring(1);
                if (extension.IsNullOrEmpty())
                {
                    return null;
                }

                //On regarde si l'extension est valide
                if (ExtensionValide.Contains(extension).IsFalse())
                {
                    return null;
                }

                //On récup le chemin de sauvegarde
                var originalDirectory = new DirectoryInfo(string.Format("{0}" + path, HttpContext.Current.Server.MapPath(@"\"))).ToString();
                if (originalDirectory.IsNullOrEmpty())
                {
                    return null;
                }

                //On génére le nouveau nom de l'image
                var nameFile = Guid.NewGuid().ToString() + "." + extension;

                //On regarde si le chemin de sauvegarde existe
                bool isExists = System.IO.Directory.Exists(originalDirectory);
                if (!isExists)
                    System.IO.Directory.CreateDirectory(originalDirectory);

                //On génére le chemin complet de sauvegarde de l'image
                var pathComplet = string.Format("{0}\\{1}", originalDirectory, nameFile);
                if (pathComplet.IsNullOrEmpty())
                {
                    return null;
                }

                //On sauvegarde l'image
                image.SaveAs(pathComplet);

                //Création des paramétres pour redimensionner l'image
                var resizeSettings = new ImageResizer.ResizeSettings
                {
                    Scale = ImageResizer.ScaleMode.DownscaleOnly,
                    MaxWidth = width,
                    MaxHeight = height,
                    Mode = ImageResizer.FitMode.Crop,
                    Quality = 50,
                };

                //Création de la miniature
                var instruction = new Instructions(resizeSettings);
                var b = ImageBuilder.Current.Build(pathComplet, pathComplet, instruction);

                return "~/" + path + "/" + nameFile;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public string GetImage(string path = null, HttpPostedFileBase file = null, int width = 0, int height = 0)
        {
            try
            {
                if (path.IsNullOrEmpty() || file.IsNull()) return null;

                if (file.ContentLength <= 0) return null;

                var fileName = HttpContext.Current.Request.Browser.Browser == "IE" ? Path.GetFileName(file.FileName) : file.FileName;
                if (fileName.IsNullOrEmpty()) return null;

                //Get de l'extension
                var extension = fileName.GetExtension();
                if (extension.IsNullOrEmpty()) return null;

                //Génére le chemin complet du fichier
                var fullPathWithFileName = path + Guid.NewGuid() + "." + extension;

                //On vérifie que le ichier n'existe déjà pas
                while (true)
                {
                    if (RFichier.FileExist(HttpContext.Current.Server.MapPath(fullPathWithFileName)))
                    {
                        //Il existe donc on génére un nouveau nom pour le fichier
                        fullPathWithFileName = path + Guid.NewGuid() + "." + extension;
                        continue;
                    }
                    break;
                }
                file.SaveAs(HttpContext.Current.Server.MapPath(fullPathWithFileName));

                //On redimensionne l'image
                Image originalImage = Image.FromFile(HttpContext.Current.Server.MapPath(fullPathWithFileName), true);
                var bitmap = new Bitmap(originalImage);
                // originalImage.Dispose();
                var resize = ResizeImage(bitmap, width, height, 24, HttpContext.Current.Server.MapPath(fullPathWithFileName));

                return fullPathWithFileName;
            }
            catch (Exception e)
            {
                var g = e.Message;
                return null;
            }
        }
        public bool ResizeImage(Bitmap image, int width, int height, int quality, string filePath)
        {
            try
            {

                // Convert other formats (including CMYK) to RGB.
                Bitmap newImage = new Bitmap(width, height, PixelFormat.Format24bppRgb);

                // Draws the image in the specified size with quality mode set to HighQuality
                using (Graphics graphics = Graphics.FromImage(newImage))
                {
                    graphics.CompositingQuality = CompositingQuality.HighQuality;
                    graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    graphics.SmoothingMode = SmoothingMode.HighQuality;
                    graphics.DrawImage(image, 0, 0, width, height);
                }

                // Get an ImageCodecInfo object that represents the JPEG codec.
                ImageCodecInfo imageCodecInfo = this.GetEncoderInfo(ImageFormat.Jpeg);

                // Create an Encoder object for the Quality parameter.
                Encoder encoder = Encoder.Quality;

                // Create an EncoderParameters object. 
                EncoderParameters encoderParameters = new EncoderParameters(1);

                // Save the image as a JPEG file with quality level.
                EncoderParameter encoderParameter = new EncoderParameter(encoder, quality);
                encoderParameters.Param[0] = encoderParameter;

                //Si l'image existe, on la del
                if (File.Exists(filePath))
                {

                    image.Dispose();
                    File.Delete(filePath);
                }
                newImage.Save(filePath, imageCodecInfo, encoderParameters);

                return true;
            }
            catch (Exception e)
            {
                var m = e.Message;
                return false;
            }
        }

        /// <summary>
        /// Method to get encoder infor for given image format.
        /// </summary>
        /// <param name="format">Image format</param>
        /// <returns>image codec info.</returns>
        private ImageCodecInfo GetEncoderInfo(ImageFormat format)
        {
            return ImageCodecInfo.GetImageDecoders().SingleOrDefault(c => c.FormatID == format.Guid);
        }

        /// <summary>
        /// Permet de récupérer toutes les clefs d'un formulaire en POST
        /// </summary>
        /// <returns>NameValueCollection</returns>
        private static NameValueCollection AllVariablesPost()
        {
            try
            {
                return HttpContext.Current.Request.Form;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Permet de récupérer toutes les clefs name d'un formulaire POST
        /// </summary>
        /// <returns></returns>
        public string[] AllKeysPost()
        {
            try
            {
                return HttpContext.Current.Request.Form.AllKeys;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Permet de récupérer toutes les clefs d'un formulaire en GET
        /// </summary>
        /// <returns>NameValueCollection</returns>
        private static NameValueCollection AllVariablesGet()
        {
            try
            {
                return HttpContext.Current.Request.QueryString;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public bool IsPost()
        {
            try
            {
                return HttpContext.Current.Request.HttpMethod == "POST";
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}