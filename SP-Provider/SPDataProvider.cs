using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SP_Provider.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace SP_Provider
{
    public class SPDataProvider
    {
        #region SharePoint URL
        string urlSharePointSite = "https://{XXXXXXXXXXX}.sharepoint.com/sites/{XXXXXXXXXXX}";
        string targetHost = "{XXXXXXXXXXX}.sharepoint.com";
        string sharePointSite = "/sites/{XXXXXXXXXXX}";
        #endregion

        #region ClientID && ClientSecret
        string clientId = "XXXXXXXX-XXXX-XXXX-XXXX-XXXXXXXXXXXX";
        string clientSecret = "XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX= ";
        #endregion

        string targetRealm = "XXXXXXXX-XXXX-XXXX-XXXX-XXXXXXXXXXXX";

        public HttpClient client = null;

        public SPDataProvider()
        {
            client = new HttpClient();

            OAuthToken token = SharePointOAuth.GetAppOnlyAccessToken(clientId, clientSecret, targetHost, targetRealm);
            if (token != null)
            {
                client.BaseAddress = new Uri(urlSharePointSite);
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(token.TokenType, token.AccessToken);
            }
        }

        public List<ListItem> GetItemsFromList(string list)
        {
            string path = sharePointSite + "/_api/web/lists/getbytitle('" + list + "')/items?$select=Id,Title,Created";

            List<ListItem> allItems = new List<ListItem>();

            GetAllSPItems(path, allItems);

            return allItems;
        }

        private void GetAllSPItems(string path, List<ListItem> mpAlls)
        {
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            HttpResponseMessage response = client.GetAsync(path).Result;

            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                path = GetSPItems(result, mpAlls);
                if (!string.IsNullOrEmpty(path))
                    GetAllSPItems(path, mpAlls);
            }
        }

        public static string GetSPItems(string jsonObject, List<ListItem> mpAlls)
        {
            string next = string.Empty;
            try
            {
                JObject item = JObject.Parse(jsonObject);

                JProperty prop = item.Properties().FirstOrDefault(p => p.Name.CompareTo("value") == 0);
                string value = prop != null ? prop.Value.ToString() : string.Empty;
                mpAlls.AddRange(JsonConvert.DeserializeObject<List<ListItem>>(value));

                prop = item.Properties().FirstOrDefault(p => p.Name.CompareTo("odata.nextLink") == 0);
                next = prop != null ? prop.Value.ToString() : string.Empty;
            }
            catch (Exception ex)
            {
            }
            return next;
        }
    }
}
