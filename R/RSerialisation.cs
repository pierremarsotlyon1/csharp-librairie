using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml;
using System.Xml.Serialization;

namespace R
{
    public static class RSerialisation
    {
        /// <summary>
        ///     Permet de sérialiser un objet en fichier XML
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objectToSerialize"></param>
        /// <param name="chemin">Chemin du fichier où l'on doit enregistrer l'objet</param>
        public static bool XmlSerialize<T>(this T objectToSerialize, string chemin)
        {
            StringWriter stringWriter = null;
            try
            {
                if (objectToSerialize.IsNull() || chemin.IsNullOrEmpty()) return false;

                var xmlSerializer = new XmlSerializer(typeof (T));

                stringWriter = new StringWriter();
                var xmlWriter = new XmlTextWriter(stringWriter) {Formatting = Formatting.Indented};

                xmlSerializer.Serialize(xmlWriter, objectToSerialize);

                //On écrit le fichier XML
                new RFichier(chemin).EcritureFichier(new[] {stringWriter.ToString()});

                return true;
            }
            catch (Exception)
            {
                return false;
            }

            finally
            {
                if (stringWriter != null)
                {
                    stringWriter.Dispose();
                    stringWriter.Close();
                }
            }
        }

        /// <summary>
        ///     Permet de sérialiser un objet en fichier XML
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objectToSerialize"></param>
        public static string XmlSerialize<T>(this T objectToSerialize)
        {
            StringWriter stringWriter = null;
            try
            {
                if (objectToSerialize.IsNull()) return null;

                var xmlSerializer = new XmlSerializer(typeof(T));

                stringWriter = new StringWriter();
                var xmlWriter = new XmlTextWriter(stringWriter) { Formatting = Formatting.Indented };

                xmlSerializer.Serialize(xmlWriter, objectToSerialize);

                return stringWriter.ToString();
            }
            catch (Exception)
            {
                return null;
            }

            finally
            {
                if (stringWriter != null)
                {
                    stringWriter.Dispose();
                    stringWriter.Close();
                }
            }
        }

        /// <summary>
        ///     Permet de déserialiser un fichier XML
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="chemin">Chemin du fichier à déserialiser</param>
        /// <returns>Object</returns>
        public static T FileXmlUnserialize<T>(this string chemin)
        {
            StreamReader lecteur = null;
            try
            {
                var serializer = new XmlSerializer(typeof (T));

                lecteur = new StreamReader(chemin);

                var p = (T) serializer.Deserialize(lecteur);

                return p;
            }
            catch (Exception)
            {
                return default(T);
            }

            finally
            {
                if (lecteur != null)
                {
                    lecteur.Dispose();
                    lecteur.Close();
                }
            }
        }

        public static T XmlUnserialize<T>(this string xmlString)
        {
            try
            {
                if (xmlString.IsNullOrEmpty()) return default (T);
                XmlSerializer ser = new XmlSerializer(typeof(T));
                StringReader stringReader = new StringReader(xmlString);
                if (stringReader.IsNull()) return default (T);
                XmlTextReader xmlReader = new XmlTextReader(stringReader);
                if (xmlReader.IsNull()) return default(T);
                object obj = ser.Deserialize(xmlReader);
                xmlReader.Close();
                stringReader.Close();
                return (T) obj;
            }
            catch (Exception e)
            {
                string h = e.Message;
                return default(T);
            }
        }

        /// <summary>
        ///     Permet de sérialiser un objet en tableau de byte
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns>byte[]</returns>
        public static byte[] BinarySerialize<T>(this T obj)
        {
            var m = new MemoryStream();
            var formatter = new BinaryFormatter();

            try
            {
                formatter.Serialize(m, obj);
                return m.ToArray(); //File.WriteAllBytes(filename, m.ToArray())
            }
            catch (Exception)
            {
                return new byte[0];
            }
            finally
            {
                m.Dispose();
                m.Close();
            }
        }

        /// <summary>
        ///     Permet de déserialiser un tableau de byte
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tab"></param>
        /// <returns>Object</returns>
        public static T BinaryUnserialize<T>(this byte[] tab)
        {
            MemoryStream memStream = null;
            var binForm = new BinaryFormatter();

            try
            {
                memStream = new MemoryStream(tab);
                memStream.Seek(0, 0);
                Object obj = binForm.Deserialize(memStream);

                return (T)obj;
            }
            catch (Exception)
            {
                return default(T);
            }
            finally
            {
                if (memStream.IsNotNull())
                {
                    memStream.Close();
                    memStream.Dispose();
                }
            }
        }

        public static T BinaryUnserialize<T>(this string str)
        {
            var memStream = new MemoryStream();
            var binForm = new BinaryFormatter();

            try
            {
                var tab = str.ToTabByte();
                memStream.Write(tab, 0, tab.Length);
                memStream.Seek(0, SeekOrigin.Begin);
                Object obj = binForm.Deserialize(memStream);

                return (T)obj;
            }
            catch (Exception)
            {
                return default(T);
            }
            finally
            {
                memStream.Close();
                memStream.Dispose();
            }
        }

        /// <summary>
        ///     Permet de sérialiser un objet dans un fichier binaire
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="chemin">Chemin du ficheir où l'on doit enregistrer l'objet</param>
        /// <returns>bool</returns>
        public static bool BinarySerialize<T>(this T obj, string chemin)
        {
            var formatter = new BinaryFormatter();
            FileStream flux = null;

            try
            {
                flux = new FileStream(chemin, FileMode.Create, FileAccess.Write);
                formatter.Serialize(flux, obj);
                flux.Flush();
                return true;
            }
            catch(Exception)
            {
                return false;
            }
            finally
            {
                //Et on ferme le flux.
                if (flux != null)
                    flux.Close();
            }
        }

        /// <summary>
        ///     Permet de déserialiser un fichier binaire
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="chemin">Chemin du fichier que l'on doit déserialiser</param>
        /// <returns>Object</returns>
        public static T BinaryUnserializeFile<T>(this string chemin)
        {
            var formatter = new BinaryFormatter();
            FileStream flux = null;
            try
            {
                flux = new FileStream(chemin, FileMode.Open, FileAccess.Read);
                return (T)formatter.Deserialize(flux);
            }
            catch(Exception e)
            {
                var a = e.Message;
                return default(T);
            }
            finally
            {
                if (flux != null)
                    flux.Close();
            }
        }
    }
}