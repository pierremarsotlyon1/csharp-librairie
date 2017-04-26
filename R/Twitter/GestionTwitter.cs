using System;
using System.Collections.Generic;
using Tweetinvi;
using Tweetinvi.Core;
using Tweetinvi.Core.Credentials;
using Tweetinvi.Core.Enum;
using Tweetinvi.Core.Extensions;
using Tweetinvi.Core.Interfaces;
using Tweetinvi.Core.Interfaces.Controllers;
using Tweetinvi.Core.Interfaces.DTO;
using Tweetinvi.Core.Interfaces.Models;
using Tweetinvi.Core.Interfaces.Streaminvi;
using Tweetinvi.Core.Parameters;
using Tweetinvi.Json;

// ReSharper disable once CheckNamespace

namespace R
{
    public class GestionTwitter
    {
        protected readonly bool Connexion;

        public static event Action<IUser, string> EventSendMessage;

        protected virtual void OnEventSendMessage(IUser arg1, string arg2)
        {
            Action<IUser, string> handler = EventSendMessage;
            if (handler != null) handler(arg1, arg2);
        }

        public GestionTwitter(string accessToken = null, string accessTokenSecret = null, string consumerKey = null,
            string consumerKeySecret = null)
        {
            if (accessToken.IsNullOrEmpty() || accessTokenSecret.IsNullOrEmpty() || consumerKey.IsNullOrEmpty() ||
                consumerKeySecret.IsNullOrEmpty())
            {
                Connexion = false;
                return;
            }


            // Create the user credentials
            //TwitterCredentials userCredentials = new TwitterCredentials(consumerKey, consumerKeySecret, accessToken, accessTokenSecret);
           
            Auth.SetUserCredentials(consumerKey, consumerKeySecret, accessToken, accessTokenSecret);
            Connexion = true;
        }

        /// <summary>
        ///     Permet de récupérer le client loggé
        /// </summary>
        /// <returns>ILoggedUser</returns>
        public ILoggedUser GetUserLoged()
        {
            try
            {
                return !Connexion ? null : User.GetLoggedUser();
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        ///     Permet de récupérer un user via son id
        /// </summary>
        /// <param name="idUser">Id du client</param>
        /// <returns>IUser</returns>
        public IUser GetUserFromId(long idUser = 0)
        {
            try
            {
                return idUser == 0 ? null : User.GetUserFromId(idUser);
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        ///     Permet de récupérer un user via son nom
        /// </summary>
        /// <param name="name">Nom de l'utilisateur du compte</param>
        /// <returns>IUser</returns>
        public IUser GetUserFromName(string name = null)
        {
            try
            {
                return !Connexion ? null : (name.IsNullOrEmpty() ? null : User.GetUserFromScreenName(name));
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        ///     Permet de récupérer les abonnements du client
        /// </summary>
        /// <param name="user">Le client cible</param>
        /// <param name="maxResearch">Nombre maximum d'user à retourner</param>
        /// <returns>IEnumerable{IUser}</returns>
        public IEnumerable<IUser> GetAbonnementsUser(IUser user = null, int maxResearch = 250)
        {
            try
            {
                // ReSharper disable once PossibleNullReferenceException
                return !Connexion ? null : (user.IsNull() ? null : user.GetFollowers(maxResearch));
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        ///     Permet de récupérer les id des abonnements du client
        /// </summary>
        /// <param name="user">Le client cible</param>
        /// <param name="maxResearch">Nombre maximum d'id abo user à retourner</param>
        /// <returns>IEnumerable{long}</returns>
        public IEnumerable<long> GetIdAbonnementsUser(IUser user = null, int maxResearch = 5000)
        {
            try
            {
                // ReSharper disable once PossibleNullReferenceException
                return !Connexion ? null : (user.IsNull() ? null : user.GetFollowerIds(maxResearch));
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        ///     Permet de récupérer les ami(e)s du client
        /// </summary>
        /// <param name="user">Le client cible</param>
        /// <param name="maxResearch">Nombre maximum d'ami(e)s à retourner</param>
        /// <returns>IEnumerable{IUser}</returns>
        public IEnumerable<IUser> GetFriendsUser(IUser user = null, int maxResearch = 250)
        {
            try
            {
                // ReSharper disable once PossibleNullReferenceException
                return !Connexion ? null : (user.IsNull() ? null : user.GetFriends(maxResearch));
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        ///     Permet de récupérer les id des ami(e)s du client
        /// </summary>
        /// <param name="user">Le client cible</param>
        /// <param name="maxResearch">Nombre maximum d'id ami(e)s à retourner</param>
        /// <returns>IEnumerable{long}</returns>
        public IEnumerable<long> GetFriendsIdUser(IUser user = null, int maxResearch = 5000)
        {
            try
            {
                // ReSharper disable once PossibleNullReferenceException
                return !Connexion ? null : (user.IsNull() ? null : user.GetFriendIds(maxResearch));
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        ///     Permet d'envoyer un message privé à un client
        /// </summary>
        /// <param name="message">Le message</param>
        /// <param name="user">Le client cible</param>
        /// <returns>IMessage</returns>
        public IMessage SendMessagePrivate(string message = null, IUser user = null)
        {
            try
            {
                if (!Connexion) return null;
                if (message.IsNullOrEmpty() || user.IsNull()) return null;

                var result = Message.PublishMessage(message, user.Id);
                OnEventSendMessage(user, message);

                return result;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        ///     Retourne le nombre total d'abonnements qu'a le client
        /// </summary>
        /// <param name="user">Le client cible</param>
        /// <returns>long</returns>
        public long GetNumberTotalAbonnees(IUser user = null)
        {
            try
            {
                return user.IsNull() ? -1 : user.FollowersCount;
            }
            catch
            {
                return -1;
            }
        }

        /// <summary>
        ///     Permet de récupérer l'id de l'utilisateur
        /// </summary>
        /// <param name="user">Le client cible</param>
        /// <returns>long</returns>
        public long GetIdUser(IUser user = null)
        {
            try
            {
                return user.IsNull() ? -1 : user.Id;
            }
            catch
            {
                return -1;
            }
        }

        /// <summary>
        ///     Permet d'obtenir la langue de l'user
        /// </summary>
        /// <param name="user">L'user cible</param>
        /// <returns>Language?</returns>
        public Language? GetLanguageDefaultUser(IUser user = null)
        {
            try
            {
                if (user.IsNull()) return null;
                return user.Language;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public bool SendTweet(string message = null)
        {
            try
            {
                if (message.IsNullOrEmpty()) return false;
                var user = GetUserLoged();
                return Tweet.PublishTweet(message).IsNotNull();
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}