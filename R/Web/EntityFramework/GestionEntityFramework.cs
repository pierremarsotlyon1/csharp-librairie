using R.Web.ViewModel;

namespace R.Web.EntityFramework
{
    using System;
    using System.Data.Entity;
    using System.Linq;

    public class GestionEntityFramework : DbContext
    {
        // Votre contexte a été configuré pour utiliser une chaîne de connexion « GestionEntityFramework » du fichier 
        // de configuration de votre application (App.config ou Web.config). Par défaut, cette chaîne de connexion cible 
        // la base de données « R.Web.EntityFramework.GestionEntityFramework » sur votre instance LocalDb. 
        // 
        // Pour cibler une autre base de données et/ou un autre fournisseur de base de données, modifiez 
        // la chaîne de connexion « GestionEntityFramework » dans le fichier de configuration de l'application.
        public GestionEntityFramework(string nameConnexionBdd)
            : base(nameConnexionBdd)
        {
        }

        // Ajoutez un DbSet pour chaque type d'entité à inclure dans votre modèle. Pour plus d'informations 
        // sur la configuration et l'utilisation du modèle Code First, consultez http://go.microsoft.com/fwlink/?LinkId=390109.

        // public virtual DbSet<MyEntity> MyEntities { get; set; }

        public virtual DbSet<RUser> RUsers { get; set; }

        public Func<ViewModelFormRegisterRUser, RUser> FuncRegisterRUser { get; set; }
        //gestionServeur.FuncRegisterRUser = (ViewModelFormRegisterRUser user) => new RUser
        //    {
        //        Password = user.Password
        //};

        /// <summary>
        /// Enumération de base
        /// </summary>
        public enum RStatus
        {
            Null = -2,
            Exception = -1,
            Failed = 0,
            Success = 1,
        }

        /// <summary>
        /// Enumération de login
        /// </summary>
        public enum RLoginStatus
        {
            Null = RStatus.Null,
            Exception = RStatus.Exception,
            Failed = RStatus.Failed,
            Success = RStatus.Success,
        }

        /// <summary>
        /// Enumération de register
        /// </summary>
        public enum RRegisterStatus
        {
            Null = RStatus.Null,
            Exception = RStatus.Exception,
            Failed = RStatus.Failed,
            Success = RStatus.Success,
            Exist = 2,
        }

        /// <summary>
        /// Permet de créer un RUser
        /// </summary>
        /// <param name="model">Le model de l'user</param>
        /// <returns>RRegisterStatus</returns>
        public RRegisterStatus RegisterUser(ViewModelFormRegisterRUser model = null)
        {
            try
            {
                //On regarde si le model est non null et qu'on a pas déjà un compte avec l'email
                if (model.IsNull()) return RRegisterStatus.Null;
                if (ExistUser(model)) return RRegisterStatus.Exist;

                //Création de l'objet RUser en fonction du delegate ou par défaut
                var user = FuncRegisterUserIsNull() ? new RUser
                {
                    Email = model.Email,
                    Nom = model.Nom,
                    Password = model.Password,
                    Prenom = model.Prenom,
                    Genre = model.Gender
                } : FuncRegisterRUser(model);
                if (user.IsNull()) return RRegisterStatus.Null;

                RUsers.Add(user);
                SaveChanges();
                return RRegisterStatus.Success;
            }
            catch (Exception)
            {
                return RRegisterStatus.Exception;
            }
        }

        /// <summary>
        /// Permet de savoir si un compte user existe
        /// </summary>
        /// <param name="model">Le model de login user</param>
        /// <returns>RRegisterStatus</returns>
        public RLoginStatus LoginUser(ViewModelFormLoginRUser model = null)
        {
            try
            {
                //On regarde si le modele est null et si email et password sont aussi vide ou null
                if (model.IsNull() || model.Email.IsNullOrEmpty() || model.Password.IsNullOrEmpty()) return RLoginStatus.Null;

                //On regarde si il existe un RUser via l'email et mot de passe
                var user = RUsers.FirstOrDefault(u => u.Email.Equals(model.Email) && u.Password.Equals(model.Password));

                return user.IsNull()
                    ? RLoginStatus.Failed
                    : RLoginStatus.Success;
            }
            catch (Exception)
            {
                return RLoginStatus.Exception;
            }
        }

        /// <summary>
        /// Permet de récupérer un user via un id
        /// </summary>
        /// <param name="id">L'id de l'user</param>
        /// <returns>RUser</returns>
        public RUser GetUser(int? id)
        {
            try
            {
                return id.IsNull() ? null : RUsers.Find(id);
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Permet de récupérer un user via un email
        /// </summary>
        /// <param name="email">L'email de l'user</param>
        /// <returns>RUser</returns>
        public RUser GetUser(string email = null)
        {
            try
            {
                return email.IsNullOrEmpty() ? null : RUsers.FirstOrDefault(u => u.Email.Equals(email));
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Permet de supprimer un user via un id
        /// </summary>
        /// <param name="id">L'id de l'user</param>
        /// <returns>RStatus</returns>
        public RStatus DeleteUser(int? id)
        {
            try
            {
                if (id.IsNull()) return RStatus.Null;

                //On récup l'user associé à l'id
                var user = GetUser(id);
                if (user.IsNull()) return RStatus.Failed;

                Entry(user).State = EntityState.Deleted;
                SaveChanges();
                return RStatus.Success;
            }
            catch (Exception)
            {
                return RStatus.Exception;
            }
        }

        /// <summary>
        /// Permet de supprimer un user via un email
        /// </summary>
        /// <param name="email">L'email de l'user</param>
        /// <returns>RStatus</returns>
        public RStatus DeleteUser(string email = null)
        {
            try
            {
                if (email.IsNullOrEmpty()) return RStatus.Null;

                //On récup l'user associé à l'id
                var user = GetUser(email);
                if (user.IsNull()) return RStatus.Failed;

                Entry(user).State = EntityState.Deleted;
                SaveChanges();
                return RStatus.Success;
            }
            catch (Exception)
            {
                return RStatus.Exception;
            }
        }

        /// <summary>
        /// Permet de changer le mot de passe d'un user via son id
        /// </summary>
        /// <param name="id">L'id de l'user</param>
        /// <param name="model">ViewModel</param>
        /// <returns>RStatus</returns>
        public RStatus ChangePasswordUser(int? id, ViewModelFormChangePassword model = null)
        {
            try
            {
                if(id.IsNull() || model.IsNull()) return RStatus.Null;

                var user = GetUser(id);
                if(user.IsNull()) return RStatus.Failed;

                if(user.Password.Equals(model.OldPassword).IsFalse()) return RStatus.Failed;

                Entry(user).State = EntityState.Modified;
                user.Password = model.NewPassword;
                SaveChanges();
                return RStatus.Success;
            }
            catch (Exception)
            {
                return RStatus.Exception;
            }
        }

        /// <summary>
        /// Permet de changer le mot de passe d'un user via son id
        /// </summary>
        /// <param name="email">L'email de l'user</param>
        /// <param name="model">ViewModel</param>
        /// <returns>RStatus</returns>
        public RStatus ChangePasswordUser(string email = null, ViewModelFormChangePassword model = null)
        {
            try
            {
                if (email.IsNullOrEmpty() || model.IsNull()) return RStatus.Null;

                var user = GetUser(email);
                if (user.IsNull()) return RStatus.Failed;

                if (user.Password.Equals(model.OldPassword).IsFalse()) return RStatus.Failed;

                Entry(user).State = EntityState.Modified;
                user.Password = model.NewPassword;
                SaveChanges();
                return RStatus.Success;
            }
            catch (Exception)
            {
                return RStatus.Exception;
            }
        }

        /// <summary>
        /// Permet de dire si un RUser existe déjà avec l'email en BDD
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        private bool ExistUser(ViewModelFormRegisterRUser model = null)
        {
            try
            {
                //On regarde si le model est vide
                if (model.IsNull()) return true;

                //On select l'user associé au mail (pour voir si il existe)
                return RUsers.FirstOrDefault(u => u.Email.Equals(model.Email)).IsNotNull();
            }
            catch (Exception)
            {
                return true;
            }
        }

        /// <summary>
        /// Permet de dire si le delegate pour l'enregistrement d'un RUser est null
        /// </summary>
        /// <returns>bool</returns>
        private bool FuncRegisterUserIsNull()
        {
            try
            {
                return FuncRegisterRUser.IsNull();
            }
            catch (Exception)
            {
                return true;
            }
        }

        private void AutoLoadRelation(bool autoLoad)
        {
            Configuration.LazyLoadingEnabled = autoLoad;
        }
    }
}