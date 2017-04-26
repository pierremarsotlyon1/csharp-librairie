using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using HtmlAgilityPack;

namespace R.Web
{
    public class RGoogleTranslate
    {
        private readonly Uri _addressWithEvent;
        private readonly Uri _adresseWithoutEvent;
        private readonly Dictionary<string, string> _language;
        private WebClient _webClient;
        private readonly Object _lock;
        public string LanguageDefaut { get; set; }

        public RGoogleTranslate()
        {
            _language = new Dictionary<string, string>();
            LanguageGoogleApi.LanguageMap(_language);
            _addressWithEvent = new Uri("http://translate.google.com/translate_t");
            _adresseWithoutEvent = new Uri("http://translate.google.com/translate_t?");
            _lock = new object();
        }

        public void Translate(string text, string fromLanguage, string toLanguage, out string result,
            bool eventArgs = false)
        {
            lock (_lock)
            {
                try
                {
                    _webClient = new WebClient();
                    _webClient.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";

                    if (eventArgs)
                    {
                        _webClient.UploadStringCompleted += webClient_UploadStringCompleted;
                        _webClient.UploadStringAsync(_addressWithEvent,
                            GetPostData(_language[fromLanguage], _language[toLanguage], text));
                    }
                    else
                    {
                        //On split le text
                        string[] split = text.SplitString(". ");
                        string temp = null;
                        foreach (string s in split)
                        {
                            s.HtmlEncode();
                            string s2 = s.HtmlDecode();
                            temp = temp +
                                   DecrypterResponse(
                                       _webClient.DownloadString(_adresseWithoutEvent +
                                                                 GetPostData(_language[fromLanguage],
                                                                     _language[toLanguage],
                                                                     s2)))+". ";
                        }

                        result = temp;
                        return;
                    }
                    result = null;
                }
                catch (Exception)
                {
                    result = null;
                }
            }
        }

        private static string GetPostData(string fromLanguage, string toLanguage, string text)
        {
            string data = string.Format("hl=en&ie=UTF8&oe=UTF8submit=Translate&langpair={0}|{1}", fromLanguage,
                toLanguage);
            return data + ("&text=" + HttpUtility.UrlEncode(text));
        }

        private void webClient_UploadStringCompleted(object sender, UploadStringCompletedEventArgs e)
        {
            string result = DecrypterResponse(e.Result);
            result.WriteLn();
        }

        private string DecrypterResponse(string reponse)
        {
            if (reponse.IsNullOrEmpty()) return null;

            var document = new HtmlDocument();
            document.LoadHtml(reponse);
            //On récup le language par défaut
            LanguageDefaut = GetLanguageText(reponse, document);
            HtmlNode node = document.DocumentNode.SelectSingleNode("//span[@id='result_box']");
            string output = node != null ? node.InnerText : null;

            return output;
        }

        public static string GetLanguageText(string text, HtmlDocument document)
        {
            string languageDefaut = null;
            var collection = document.DocumentNode.SelectNodes("//*[@id='gt-sl']/option");

            if (collection == null) return null;
            foreach (var c in from c in collection let attrSelected = c.Attributes["selected"] where attrSelected != null select c)
            {
                languageDefaut = c.Attributes["value"].Value;
            }

            return languageDefaut;
        }
    }

    public class LanguageGoogleApi
    {
        public static void LanguageMap(Dictionary<string, string> language)
        {
            language.Add("Afrikaans", "af");
            language.Add("Albanian", "sq");
            language.Add("Arabic", "ar");
            language.Add("Armenian", "hy");
            language.Add("Azerbaijani", "az");
            language.Add("Basque", "eu");
            language.Add("Belarusian", "be");
            language.Add("Bengali", "bn");
            language.Add("Bulgarian", "bg");
            language.Add("Catalan", "ca");
            language.Add("Chinese", "zh-CN");
            language.Add("Croatian", "hr");
            language.Add("Czech", "cs");
            language.Add("Danish", "da");
            language.Add("Dutch", "nl");
            language.Add("English", "en");
            language.Add("French", "fr");
            language.Add("Espagnol", "es");
            language.Add("Allemand", "de");
            language.Add("Auto", "auto");
        }
    }
}