using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace R.Web.ViewModel
{
    /// <summary>
    /// Classe pour l'inscription d'un user
    /// </summary>
    public partial class ViewModelFormRegisterRUser
    {
        [Required(ErrorMessage = "Vous devez saisir un email")]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        private string _password;

        [Required(ErrorMessage = "Vous devez saisir un mot de passe")]
        [StringLength(100, ErrorMessage = "La chaîne {0} doit comporter au moins {2} caractères.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Mot de passe")]
        public string Password
        {
            get { return _password; }
            set { _password = value.ToMd5(); }
        }

        private string _confirmPassword;

        [Required(ErrorMessage = "Vous devez saisir un mot de passe")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirmer le mot de passe")]
        [Compare("Password", ErrorMessage = "Le mot de passe et le mot de passe de confirmation ne correspondent pas.")]
        public string ConfirmPassword
        {
            get { return _confirmPassword; }
            set { _confirmPassword = value.ToMd5(); }
        }

        [Required(ErrorMessage = "Vous devez saisir un prénom")]
        [Display(Name = "Prénom")]
        public string Prenom { get; set; }

        [Required(ErrorMessage = "Vous devez saisir un nom")]
        [Display(Name = "Nom")]
        public string Nom { get; set; }

        [Required(ErrorMessage = "Vous devez sélectionner un genre")]
        public string Gender { get; set; }
    }

    /// <summary>
    /// Classe pour la connexion d'un user
    /// </summary>
    public class ViewModelFormLoginRUser
    {
        [Required(ErrorMessage = "Vous devez saisir un email")]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        private string _password;

        [Required(ErrorMessage = "Vous devez saisir un mot de passe")]
        [StringLength(100, ErrorMessage = "La chaîne {0} doit comporter au moins {2} caractères.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Mot de passe")]
        public string Password
        {
            get { return _password; }
            set { _password = value.ToMd5(); }
        }
    }

    /// <summary>
    /// Classe pour le formulaire de changement de mot de passe
    /// </summary>
    public class ViewModelFormChangePassword
    {
        private string _oldPassword;

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Mot de passe actuel")]
        public string OldPassword
        {
            get
            {
                return _oldPassword;
            }
            set { _oldPassword = value.ToMd5(); }
        }

        private string _newPassword;

        [Required]
        [StringLength(100, ErrorMessage = "Le {0} doit compter au moins {2} caractères.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Nouveau mot de passe")]
        public string NewPassword
        {
            get
            {
                return _newPassword;;
            }
            set { _newPassword = value.ToMd5(); }
        }

        private string _confirmPassword;

        [DataType(DataType.Password)]
        [Display(Name = "Confirmer le nouveau mot de passe")]
        [Compare("NewPassword", ErrorMessage = "Le nouveau mot de passe et le mot de passe de confirmation ne correspondent pas.")]
        public string ConfirmPassword
        {
            get
            {
                return _confirmPassword;
            }
            set { _confirmPassword = value.ToMd5(); }
        }
    }
}
