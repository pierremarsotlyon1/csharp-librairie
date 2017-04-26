using System;
using System.Collections.Generic;
using System.Linq;

// ReSharper disable once CheckNamespace
namespace R
{
    public class RPanier<T>
    {
        private readonly Action<RPanier<T>> _calculerPrixPanier;
        public List<T> ListeProduct { get; set; }
        public double PrixTotal { get; set; }

        public RPanier(Action<RPanier<T>> calculerPrixPanier)
        {
            PrixTotal = 0;
            ListeProduct = new List<T>();
            _calculerPrixPanier = calculerPrixPanier;
        }
        
        /// <summary>
        ///     Permet d'ajouter un article dans le panier
        /// </summary>
        /// <param name="obj">L'objet à ajouter</param>
        /// <returns>bool</returns>
        public bool Add(T obj)
        {
            try
            {
                //On regarde si les variables sont correctes
                if (obj.IsNull()) return false;

                //On ajoute le produit
                ListeProduct.Add(obj);

                //On calcule le prix total du nouveau panier
                _calculerPrixPanier(this);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        ///     permet de supprimer un article du panier
        /// </summary>
        /// <param name="obj">L'objet à delete</param>
        /// <returns>bool</returns>
        public bool Delete(T obj)
        {
            try
            {
                if (obj.IsNull()) return false;
                //On supprime le produit du panier
                ListeProduct = ListeProduct.Where(product => !product.Equals(obj)).ToList();
                //On calcule le prix du nouveau panier
                _calculerPrixPanier(this);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        ///     Permet dem odifier un produit
        /// </summary>
        /// <param name="oldProduct">L'ancien produit</param>
        /// <param name="newProduct">Le nouveau produit</param>
        /// <returns>bool</returns>
        public bool Update(T oldProduct, T newProduct)
        {
            try
            {
                if (oldProduct.IsNull() || newProduct.IsNull()) return false;

                //On cherche l'ancien produit et on le modifie
                var check = false;
                var temp = new List<T>();
                foreach (var product in ListeProduct)
                {
                    if (!product.Equals(oldProduct))
                    {
                        temp.Add(product);
                        continue;
                    }
                    temp.Add(newProduct);
                    check = true;
                }

                ListeProduct = temp;
                _calculerPrixPanier(this);
                return check;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        ///     Vérifie si un article est déjà présent dans le panier
        /// </summary>
        /// <param name="obj">L'objet à vérifier</param>
        /// <returns>bool</returns>
        public bool IsPresent(T obj)
        {
            try
            {
                return !obj.IsNull() && Enumerable.Contains(ListeProduct, obj);
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}