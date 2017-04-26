using System;

namespace R
{
    public class Bootstrap
    {
        /// <summary>
        /// Permet de générer un modal
        /// </summary>
        /// <param name="id">Id du modal</param>
        /// <param name="title">Titre du modal</param>
        /// <param name="body">Contenu du modal</param>
        /// <param name="valueButton">Text du boutton pour vallider</param>
        /// <returns>string</returns>
        public string Modal(string id = null, string title = null, string body = null, string valueButton = null)
        {
            try
            {
                //On regarde si l'un des paramétres est vide ou nul
                if (new[]
                {
                    id, title, body
                }.NullOrEmpty()) return null;

                //Génération du modal
                var response = "<!-- Button trigger modal -->" +
                               "<button type='button' class='btn btn-primary btn-lg' data-toggle='modal' data-target='#" +
                               id + "'>" +
                               title +
                               "</button>" +
                               "<!-- Modal -->" +
                               "<div class='modal fade' id='" + id + "' tabindex='-1' role='dialog' aria-labelledby='" +
                               id + "Label' aria-hidden='true'>" +
                               "<div class='modal-dialog'>" +
                               "<div class='modal-content'>" +
                               "<div class='modal-header'>" +
                               "<button type='button' class='close' data-dismiss='modal' aria-label='Close'><span aria-hidden='true'>&times;</span></button>" +
                               "<h4 class='modal-title' id='" + id + "Label'>Modal title</h4>" +
                               "</div>" +
                               "<div class='modal-body'>" +
                               body +
                               "</div>" +
                               "<div class='modal-footer'>" +
                               "<button type='button' class='btn btn-default' data-dismiss='modal'>Close</button>";
                if (valueButton.IsNotNullOrEmpty())
                    response += "<button type='button' class='btn btn-primary'>"+valueButton+"</button>";
                response += "</div>" +
                            "</div>" +
                            "</div>" +
                            "</div>";
                return response;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// PErmet de générer une alerte success CSS
        /// </summary>
        /// <param name="body"></param>
        /// <returns>string</returns>
        public string AlertSuccess(string body = null)
        {
            try
            {
                return body.IsNullOrEmpty() ? null : "<div class='alert alert-success' role='alert'>" + body + "</div>";
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}