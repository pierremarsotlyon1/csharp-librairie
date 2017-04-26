using System;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace R.Web
{
    public class REmailSender
    {
        private static string _cheminTxt;
        private readonly MailMessage _m;
        private readonly string _passwordEmail;
        private readonly string _smtp;
        private readonly string _userEmail;
        private readonly string _emailFrom;
        private readonly bool _checkEmail;
        public string Error;
        //Gestion des events
        public event Action<MailMessage> EventSendEmail;

        protected virtual void OnEventSendEmail(MailMessage obj)
        {
            Action<MailMessage> handler = EventSendEmail;
            if (handler != null) handler(obj);
        }

        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="emailFrom">Email de l'envoyeur</param>
        /// <param name="smtp">SMTP du service</param>
        /// <param name="userEmail">Login du compte qui va envoyer l'email</param>
        /// <param name="passwordEmail">Password du compte qui va envoyer l'email</param>
        /// <param name="fileError">Fichier de logs des erreur</param>
        /// <param name="checkEvent">Utilisation des events</param>
        public REmailSender(string emailFrom, string smtp, string userEmail, string passwordEmail, string fileError = "errorEmail.txt")
        {
            try
            {
                _m = new MailMessage();
                _m.Sender = _m.From = new MailAddress(emailFrom);
                _m.IsBodyHtml = true;

                _cheminTxt = fileError;
                _smtp = smtp;
                _userEmail = userEmail;
                _passwordEmail = passwordEmail;
                _emailFrom = emailFrom;

                _checkEmail = true;
            }
            catch (Exception)
            {
                _checkEmail = false;
            }
        }

        public bool IsHtml
        {
            set { _m.IsBodyHtml = value; }
        }

        public string To
        {
            set { _m.To.Add(value); }
        }

        public string From
        {
            set { _m.Sender = _m.From = new MailAddress(value); }
        }

        public string Subject
        {
            set { _m.Subject = value; }
        }

        public string Body
        {
            set { _m.Body = value; }
        }

        public void RemiseAZero()
        {
            _m.Body = "";
            _m.Subject = null;
            _m.To.Clear();
        }

        /// <summary>
        /// Permet d'envoyer un email
        /// </summary>
        /// <returns>bool</returns>
        private bool Send()
        {
            try
            {
                var mailClient =
                    new SmtpClient(_smtp, 587)
                        {
                            Credentials = new NetworkCredential(
                                _userEmail,
                                _passwordEmail),
                            EnableSsl = false,
                            DeliveryMethod = SmtpDeliveryMethod.Network,
                            UseDefaultCredentials = false
                        };

                //Envoie du mail
                mailClient.Send(_m);
                return true;
            }
            catch (Exception e)
            {
                Error = e.Message;
                return false;
            }
        }

        /// <summary>
        /// Permet la préparation d'envoie d'un email
        /// </summary>
        /// <param name="destinataire">Email du destinataire</param>
        /// <param name="sujet">Sujet de l'email</param>
        /// <param name="body">Contenu de l'email</param>
        /// <returns>bool</returns>
        public bool EnvoyerEmail(string destinataire, string sujet, string body)
        {
            try
            {
                if (!_checkEmail || !destinataire.ValidateEmail() || body.IsNullOrEmpty() || sujet.IsNullOrEmpty()) return false;
                //To = destinataire;
                //Subject = sujet;
                //Body = body;
                //var clientb = new SmtpClient
                //{
                //    Host = _smtp,
                //    Port = 587,
                //    Credentials = new NetworkCredential(_userEmail, _passwordEmail),
                //    EnableSsl = true,
                //};

                //var msg = new MailMessage
                //{
                //    From = new MailAddress(_emailFrom),
                //    To = { new MailAddress(destinataire) },
                //    IsBodyHtml = true,
                //    Subject = sujet,
                //    Body = body
                //};
                //clientb.Send(msg);

                

                var loginInfo = new NetworkCredential(_userEmail, _passwordEmail);
                var msg = new MailMessage();
                var smtpClient = new SmtpClient("smtp.gmail.com", 587);

                msg.From = new MailAddress(_userEmail);
                msg.To.Add(new MailAddress(destinataire));
                msg.Subject = sujet;
                msg.Body = body;
                msg.IsBodyHtml = true;

                smtpClient.EnableSsl = true;
                smtpClient.UseDefaultCredentials = false;
                smtpClient.Credentials = loginInfo;
                smtpClient.Send(msg);


                return true;
            }
            catch (Exception e)
            {
                var x = e.Message;
                return false;
            }
        }
    }
}
