using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace R
{
    public class RHamming
    {
        private readonly RFichier _fichier;
        private int _b1;
        private int _b2;
        private int _b3;
        private int _b4;
        private int _p1;
        private int _p2;
        private int _p3;

        public RHamming()
        {
            _fichier = new RFichier();
        }

        public string FichierOrigin { get; set; }
        public string FichierDest { get; set; }
        public bool VerifFichier { get; set; }

        /// <summary>
        ///     Permet de récupèrer les bytes d'un fichier
        /// </summary>
        /// <param name="fichier">Le fichier</param>
        /// <returns>byte[]</returns>
        public byte[] GetAllByte(string fichier)
        {
            try
            {
                return File.Exists(fichier) ? File.ReadAllBytes(fichier) : new byte[0];
            }
            catch (Exception)
            {
                throw new Exception("Erreur dans la lecture du fichier");
            }
        }

        /// <summary>
        ///     Permet de crypter un tableau de byte via hamming
        /// </summary>
        /// <returns>List{byte}</returns>
        public List<byte> Cryptage(byte[] tab)
        {
            try
            {
                if (!tab.Any()) throw new Exception("La liste de byte est vide");

                var result = new List<byte>();

                foreach (byte octet in tab)
                {
                    //On récup le quartet haut
                    var haut = (byte)(octet >> 4);
                    //On récup le quartet bas
                    var bas = (byte)(octet & 0x0F);

                    //On parse les 2 octets
                    CoderByte(ref haut);
                    CoderByte(ref bas);
                    result.Add(haut);
                    result.Add(bas);
                }

                return result;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        /// <summary>
        /// Permet de coder un byte
        /// </summary>
        /// <param name="octet">Byte à coder</param>
        private void CoderByte(ref byte octet)
        {
            try
            {
                //On récup les _b
                _b1 = (octet & 0x08) != 0 ? 1 : 0;
                _b2 = (octet & 0x04) != 0 ? 1 : 0;
                _b3 = (octet & 0x02) != 0 ? 1 : 0;
                _b4 = (octet & 0x01) != 0 ? 1 : 0;

                //Calcul de _p1 ( b1 + b2 + b4 )
                _p1 = (_b1 + _b2 + _b4) % 2 != 0 ? 1 : 0;

                //Calcul de _p2 ( b1 + b3 + b4 )
                _p2 = (_b1 + _b3 + _b4) % 2 != 0 ? 1 : 0;

                //Calcul de _p3 ( b2 + b3 + b4 )
                _p3 = (_b2 + _b3 + _b4) % 2 != 0 ? 1 : 0;

                //On calcul le nouveau bit ( p1, p2, b1, p3, b2, b3, b4)
                byte temp = 0;
                if (_p1 == 1)
                    temp = (byte)(temp | 0x80);
                if (_p2 == 1)
                    temp = (byte)(temp | 0x40);
                if (_b1 == 1)
                    temp = (byte)(temp | 0x20);
                if (_p3 == 1)
                    temp = (byte)(temp | 0x10);
                if (_b2 == 1)
                    temp = (byte)(temp | 0x08);
                if (_b3 == 1)
                    temp = (byte)(temp | 0x04);
                if (_b4 == 1)
                    temp = (byte)(temp | 0x02);

                //Calcul de la parite
                if (((_p1 + _p2 + _p3 + _b1 + _b2 + _b3 + _b4) % 2 != 0))
                    temp = (byte)(temp | 0x01);
                octet = temp;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        /// <summary>
        /// Permet de décoder un tableau de byte
        /// </summary>
        /// <param name="listeAncienByte">Le tableau de byte à decoder</param>
        /// <returns>List{byte}</returns>
        public List<byte> Decoder(byte[] listeAncienByte)
        {
            try
            {
                if (!listeAncienByte.Any()) throw new Exception("Le liste de byte est vide");

                var liste = new List<byte>();

                for (int i = 0; i < listeAncienByte.Count(); i += 2)
                {
                    CheckErreur(ref listeAncienByte[i]);
                    CheckErreur(ref listeAncienByte[i + 1]);
                    byte haut = DecrypterByte(ref listeAncienByte[i]);
                    byte bas = DecrypterByte(ref listeAncienByte[i + 1]);

                    int temp = (haut << 4) | bas;
                    liste.Add((byte)temp);
                }
                return liste;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        /// <summary>
        /// Permet de vérifier les erreurs lorsque l'on décode
        /// </summary>
        /// <param name="octet">Le byte à vérifier</param>
        private static void CheckErreur(ref byte octet)
        {
            try
            {
                int temp = 0;
                byte count = 0;
                //Groupement 1-3-5-7
                if ((octet & 0x80) != 0)
                    temp++;
                if ((octet & 0x20) != 0)
                    temp++;
                if ((octet & 0x08) != 0)
                    temp++;
                if ((octet & 0x02) != 0)
                    temp++;

                if ((temp % 2) != 0)
                    count = (byte)(count | 0x01);
                temp = 0;

                //Groupement 2-3-6-7
                if ((octet & 0x40) != 0)
                    temp++;
                if ((octet & 0x20) != 0)
                    temp++;
                if ((octet & 0x04) != 0)
                    temp++;
                if ((octet & 0x02) != 0)
                    temp++;
                if ((temp % 2) != 0)
                    count = (byte)(count | 0x02);
                temp = 0;

                //Groupement 4-5-6-7
                if ((octet & 0x10) != 0)
                    temp++;
                if ((octet & 0x08) != 0)
                    temp++;
                if ((octet & 0x04) != 0)
                    temp++;
                if ((octet & 0x02) != 0)
                    temp++;
                if ((temp % 2) != 0)
                    count = (byte)(count | 0x04);

                switch (count)
                {
                    case 4:
                        octet = (byte)((octet & 0x10) != 0 ? octet & 0x10 : octet | 0x10);
                        break;
                    case 2:
                        octet = (byte)((octet & 0x40) != 0 ? octet & 0x40 : octet | 0x40);
                        break;
                    case 1:
                        octet = (byte)((octet & 0x80) != 0 ? octet & 0x80 : octet | 0x80);
                        break;
                    case 3:
                        octet = (byte)((octet & 0x20) != 0 ? octet & 0x20 : octet | 0x20);
                        break;
                    case 5:
                        octet = (byte)((octet & 0x08) != 0 ? octet & 0x08 : octet | 0x08);
                        break;
                    case 6:
                        octet = (byte)((octet & 0x04) != 0 ? octet & 0x04 : octet | 0x04);
                        break;
                    case 7:
                        octet = (byte)((octet & 0x02) != 0 ? octet & 0x02 : octet | 0x02);
                        break;
                    case 8:
                        octet = (byte)((octet & 0x01) != 0 ? octet & 0x01 : octet | 0x01);
                        break;
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        /// <summary>
        /// Permet de décrypter un byte
        /// </summary>
        /// <param name="octet">Le byte à décrypter</param>
        /// <returns>byte</returns>
        private byte DecrypterByte(ref byte octet)
        {
            _b1 = (octet & 0x20) != 0 ? 1 : 0;
            _b2 = (octet & 0x08) != 0 ? 1 : 0;
            _b3 = (octet & 0x04) != 0 ? 1 : 0;
            _b4 = (octet & 0x02) != 0 ? 1 : 0;
            byte temp = 0;
            if (_b1 == 1)
                temp = (byte)(temp | 0x08);
            if (_b2 == 1)
                temp = (byte)(temp | 0x04);
            if (_b3 == 1)
                temp = (byte)(temp | 0x02);
            if (_b4 == 1)
                temp = (byte)(temp | 0x01);

            return temp;
        }

        public bool EcritureFichier(List<byte> liste, string path)
        {
            try
            {
                return _fichier.EcritureFichier(liste.ToArray(), path);
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}