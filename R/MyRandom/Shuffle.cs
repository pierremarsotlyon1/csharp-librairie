using System;
using System.Collections.Generic;
using System.Linq;

namespace R
{
    public class Shuffle
    {
        private const string Alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
        private const string Numeric = "0123456789";
        private const string Operator = "*/+-=";
        private const string Special = "@#$%&";
        private const string Pontuacao = "!?;:._\\|";
        private const string Conjuntos = "{[()]}";
        private readonly string _results;

        public Shuffle()
        {
            _results += Alphabet;
            _results += Numeric;
            _results += Operator;
            _results += Special;
            _results += Conjuntos;
            _results += Pontuacao;
        }

        public string GetToken(int limit = 0)
        {
            try
            {
                //Vérification du la limite donnée
                if (limit.IsDefault()) return null;
                if (limit.IsNotInt() && limit.IsNotFloat()) return null;
                if (limit <= 0 || limit > 87) return null;
                //Return du string mélangé
                return SetTocken(limit);
            }
            catch (Exception)
            {
                return null;
            }
        }

        private string SetTocken(int limit)
        {
            try
            {
                //Création du random
                var num = new System.Random();

                //Création du nouveau string avec les données mélangées
                return new string(_results.ToCharArray().
                    OrderBy(s => (num.Next(2) % 2) == 0).ToArray()).RSubstring(0, limit);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static List<int> GenerateTableauAleatoire(int nbVoulu = 0, int nbMax = 0, List<int> nbNonVoulu = null)
        {
            try
            {
                if (nbMax.IsDefault() || nbVoulu.IsDefault()) return new List<int>();

                var liste = new List<int>();
                while (liste.Count < (nbVoulu + 1))
                {
                    var random = new System.Random();
                    int temp = random.Next(0, nbMax);
                    if (nbNonVoulu.IsNotNull() && nbNonVoulu.IsNotEmpty())
                    {
                        bool b = false;
                        // ReSharper disable once AssignNullToNotNullAttribute
                        // ReSharper disable once UnusedVariable
                        foreach (int i in nbNonVoulu.Where(i => temp.Equals(i)))
                        {
                            b = true;
                        }
                        if (b) continue;
                    }
                    if (liste.CheckDoublon(temp)) continue;
                    liste.Add(temp);
                }

                return liste;
            }
            catch (Exception)
            {
                return new List<int>();
            }
        }

        public static int RandomNumber(int min, int max)
        {
            try
            {
                return new Random().Next(min, max); 
            }
            catch (Exception)
            {
                return -1;
            }
        }
    }
}