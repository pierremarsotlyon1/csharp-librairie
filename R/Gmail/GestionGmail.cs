using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Gmail.v1;
using Google.Apis.Gmail.v1.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;

namespace R.Gmail
{
    public class GestionGmail
    {
        private readonly string[] _scopes = { GmailService.Scope.GmailReadonly };
        private const string ApplicationName = "Gmail API Quickstart";
        private readonly GmailService _gmailService;
        public event Action<List<Message>> NewMessage; 

        public GestionGmail()
        {
            try
            {
                UserCredential credential;
                using (
                    var stream = new FileStream(RFichier.DirectoryAppWithFile("google/client_secret.json"),
                        FileMode.Open,
                        FileAccess.Read))
                {
                    string credPath = System.Environment.GetFolderPath(System.Environment
                        .SpecialFolder.Personal);
                    credPath = Path.Combine(credPath, ".credentials");

                    credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                        GoogleClientSecrets.Load(stream).Secrets,
                        _scopes,
                        "user",
                        CancellationToken.None,
                        new FileDataStore(credPath, true)).Result;
                }

                // Create Gmail Service.
                _gmailService = new GmailService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = credential,
                    ApplicationName = ApplicationName,
                });
            }
            catch (Exception)
            {
                
            }
        }

        public bool RunMessageUnRead(int timer = 10)
        {
            try
            {
                if (_gmailService.IsNull()) return false;

                RThread.DoAsync(() =>
                {
                    var firstPassage = false;
                    while (true)
                    {
                        if(firstPassage)
                            timer.SleepToMin();
                        firstPassage = true;

                        //Get des messages unread
                        var listeMessageUnRead = GetMessageUnRead();
                        if (listeMessageUnRead.IsEmpty()) continue;

                        //Si il y en a, on notifie
                        OnNewMessage(listeMessageUnRead);
                    }
                    
                    // ReSharper disable once FunctionNeverReturns
                });
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private List<Message> GetMessageUnRead()
        {
            try
            {
                List<Message> result = new List<Message>();

                //On récup les 100 derniers id des threads message
                var request = _gmailService.Users.Messages.List("me").Execute();
                if (request.IsNull() || request.Messages.IsEmpty()) return null;

                foreach (var threadMessage in request.Messages)
                {
                    if (threadMessage.IsNull()) continue;
                    //On récup le message
                    var message = _gmailService.Users.Messages.Get("me", threadMessage.Id).Execute();
                    if (message.IsNull() || message.LabelIds.IsEmpty()) continue;
                    //On regarde si il a le label "UNREAD"
                    foreach (var labelId in message.LabelIds)
                    {
                        if (labelId.IsEmpty()) continue;
                        if (labelId.IsNotEquals("UNREAD")) continue;
                        result.Add(message);
                        break;
                    }
                }

                return result;
            }
            catch (Exception)
            {
                return null;
            }
        }

        protected virtual void OnNewMessage(List<Message> obj)
        {
            var handler = NewMessage;
            if (handler != null) handler(obj);
        }
    }
}
