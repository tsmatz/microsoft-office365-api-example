using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using O365APISample.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Mvc;

namespace O365APISample.Controllers
{
    public class MailController : Controller
    {
        //
        // This is a simple sample !
        // (Combine ASP.NET Identity and OAuth flow in actual application.)

        public ActionResult Inbox(string code)
        {
            // redirect to Azure AD (and returning code)
            if(string.IsNullOrEmpty(code))
            {
                var clientId = ConfigurationManager.AppSettings["ClientId"];
                var redirectUri = ConfigurationManager.AppSettings["RedirectUri"];
                return Redirect(string.Format("https://login.microsoftonline.com/common/oauth2/v2.0/authorize?response_type=code&response_mode=query&client_id={0}&scope=https%3a%2f%2fgraph.microsoft.com%2fmail.read+offline_access&redirect_uri={1}",
                    HttpUtility.UrlEncode(clientId),
                    HttpUtility.UrlEncode(redirectUri)));
            }

            // get access token form code
            HttpClient cl1 = new HttpClient();
            var requestBody = new List<KeyValuePair<string, string>>();
            requestBody.Add(
                new KeyValuePair<string, string>("grant_type", "authorization_code"));
            requestBody.Add(
                new KeyValuePair<string, string>("code", code));
            requestBody.Add(
                new KeyValuePair<string, string>("client_id",
                    ConfigurationManager.AppSettings["ClientId"]));
            requestBody.Add(
                new KeyValuePair<string, string>("client_secret",
                    ConfigurationManager.AppSettings["ClientSecret"]));
            // &scope=https%3A%2F%2Fgraph.microsoft.com%2Fmail.read
            requestBody.Add(
                new KeyValuePair<string, string>("scope",
                    @"https://graph.microsoft.com/mail.read"));
            requestBody.Add(
                new KeyValuePair<string, string>("redirect_uri",
                    ConfigurationManager.AppSettings["RedirectUri"]));
            var resMsg1 = cl1.PostAsync("https://login.microsoftonline.com/common/oauth2/v2.0/token",
                new FormUrlEncodedContent(requestBody)).Result;
            var resStr1 = resMsg1.Content.ReadAsStringAsync().Result;
            JObject json1 = JObject.Parse(resStr1);
            var tokenType = ((JValue)json1["token_type"]).ToObject<string>();
            var accessToken = ((JValue)json1["access_token"]).ToObject<string>();

            // get inbox using access token
            HttpClient cl2 = new HttpClient();
            var acceptHeader =
                new MediaTypeWithQualityHeaderValue("application/json");
            // This is not need at Oct 2014 Update
            //acceptHeader.Parameters.Add(
            //  new NameValueHeaderValue("odata", "minimalmetadata"));
            cl2.DefaultRequestHeaders.Accept.Add(acceptHeader);
            cl2.DefaultRequestHeaders.Authorization
              = new AuthenticationHeaderValue(tokenType, accessToken);
            //var resMsg2 =
            //  cl2.GetAsync("https://graph.microsoft.com/v1.0/me/messages?$orderby=receivedDateTime%20desc&$top=20&$select=subject,receivedDateTime,from").Result;
            var resMsg2 =
              cl2.GetAsync("https://graph.microsoft.com/v1.0/me/mailFolders/Inbox/messages?$orderby=receivedDateTime%20desc&$top=20&$select=subject,receivedDateTime,from").Result;
            var resStr2 = resMsg2.Content.ReadAsStringAsync().Result;
            JObject json2 = JObject.Parse(resStr2);
            IEnumerable<MailItem> mails = JsonConvert.DeserializeObject<IEnumerable<MailItem>>(json2["value"].ToString());
            return View(new Inbox { Mails = mails });
        }
    }
}
