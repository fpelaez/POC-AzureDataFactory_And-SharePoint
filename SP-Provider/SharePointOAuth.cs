using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SP_Provider
{
    public class OAuthToken
    {
        public string TokenType { get; set; }
        public string AccessToken { get; set; }
    }

    public static class SharePointOAuth
    {
        private const string SHAREPOINTPRINCIPAL = "XXXXXXXX-XXXX-XXXX-XXXX-XXXXXXXXXXXX";

        public static OAuthToken GetAppOnlyAccessToken(string clientId, string clientSecret, string targetHost, string targetRealm)
        {
            OAuthToken token = null;

            string resource = GetFormattedPrincipal(SHAREPOINTPRINCIPAL, targetHost, targetRealm);
            string tenantId = GetFormattedPrincipal(clientId, string.Empty, targetRealm);
            string stsUrl = GetSecurityTokenServiceUrl(targetRealm);

            using (var client = new HttpClient())
            {
                var requestToken = new HttpRequestMessage
                {
                    Method = HttpMethod.Post,
                    RequestUri = new Uri(stsUrl)
                };

                var contentParams = new List<KeyValuePair<string, string>>();
                contentParams.Add(new KeyValuePair<string, string>("grant_type", "client_credentials"));
                contentParams.Add(new KeyValuePair<string, string>("client_id", tenantId));
                contentParams.Add(new KeyValuePair<string, string>("client_secret", clientSecret));
                contentParams.Add(new KeyValuePair<string, string>("resource", resource));
                contentParams.Add(new KeyValuePair<string, string>("scope", resource));
                requestToken.Content = new FormUrlEncodedContent(contentParams);

                var result = client.SendAsync(requestToken).Result;
                if (result.IsSuccessStatusCode)
                {
                    string resultContent = result.Content.ReadAsStringAsync().Result;
                    token = JsonToOAuthToken(resultContent);
                }
                else
                {
                    //loggin error
                }
            }

            return token;
        }

        private static OAuthToken JsonToOAuthToken(string jsonObject)
        {
            OAuthToken token = new OAuthToken();
            JObject item = JObject.Parse(jsonObject);

            JProperty prop = item.Properties().FirstOrDefault(p => p.Name.CompareTo("token_type") == 0);
            token.TokenType = prop != null ? prop.Value.ToString() : string.Empty;

            prop = item.Properties().FirstOrDefault(p => p.Name.CompareTo("access_token") == 0);
            token.AccessToken = prop != null ? prop.Value.ToString() : string.Empty;

            return token;
        }

        private static string GetFormattedPrincipal(string principalName, string hostName, string realm)
        {
            if (!string.IsNullOrEmpty(hostName))
            {
                return string.Concat(principalName, "/", hostName, "@", realm);
            }
            return string.Concat(principalName, "@", realm);
        }

        private static string GetSecurityTokenServiceUrl(string realm)
        {
            return string.Concat("https://accounts.accesscontrol.windows.net/", realm, "/tokens/OAuth/2");
        }

        public static string GetDigestForSharePoint(string siteUrl, string token)
        {
            HttpClient client = new HttpClient();

            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
            client.DefaultRequestHeaders.Add("accept", "application/json;odata=verbose");
            StringContent content = new StringContent("");

            string digest = "";

            HttpResponseMessage response = client.PostAsync($"{siteUrl}/_api/contextinfo", content).Result;
            if (response.IsSuccessStatusCode)
            {
                string contentJson = response.Content.ReadAsStringAsync().Result;
                JObject val = JObject.Parse(contentJson);
                JToken d = val["d"];
                JToken wi = d["GetContextWebInformation"];
                digest = wi.Value<string>("FormDigestValue");
            }

            return digest;
        }
    }
}
