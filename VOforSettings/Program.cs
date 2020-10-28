using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System;
using Newtonsoft.Json.Linq;

namespace D365S2S
{
    class Program
    {
        static void Main(string[] args)
        {
            var contacts = CrmRequest(
                HttpMethod.Get,
                "https://udstrialsdemo40.crm4.dynamics.com/api/data/v9.1/contacts")
                .Result.Content.ReadAsStringAsync();
            // Similarly you can make POST, PATCH & DELETE requests 
            

                    JObject jRetrieveResponse =
                        JObject.Parse(contacts.Result);
            //string fullname = jRetrieveResponse["fullname"].ToString();
            //Console.WriteLine("Fullname " + fullname);

            string test = "https://udstrialsdemo40.crm4.dynamics.com/api/data/v9.1/contacts";
            //int i = test.IndexOf("api");
            test = test.Remove(test.IndexOf("api"));

            Console.WriteLine(contacts.Result);
            Console.ReadLine();
            
        }

        public static async Task<string> AccessTokenGenerator()
        {
            string clientId = "00aea9ee-9733-41d2-b1c7-b2d2207fb471"; // Your Azure AD Application ID  
            string clientSecret = "9wPio6.2O9PPE~TG5-8xA9S-usAHFA~ZL2"; // Client secret generated in your App  
            string authority = "https://login.microsoftonline.com/29a54688-cd45-4308-9b73-435d36ddb378"; // Azure AD App Tenant ID  
            string resourceUrl = "https://udstrialsdemo40.crm4.dynamics.com/"; // Your Dynamics 365 Organization URL  

            var credentials = new ClientCredential(clientId, clientSecret);
            var authContext = new AuthenticationContext(authority);
            var result = await authContext.AcquireTokenAsync(resourceUrl, credentials);
            return result.AccessToken;
        }

        public static async Task<HttpResponseMessage> CrmRequest(HttpMethod httpMethod, string requestUri, string body = null)
        {
            // Acquiring Access Token  
            var accessToken = await AccessTokenGenerator();

            var client = new HttpClient();
            var message = new HttpRequestMessage(httpMethod, requestUri);

            // OData related headers  
            message.Headers.Add("OData-MaxVersion", "4.0");
            message.Headers.Add("OData-Version", "4.0");
            message.Headers.Add("Prefer", "odata.include-annotations=\"*\"");

            // Passing AccessToken in Authentication header  
            message.Headers.Add("Authorization", $"Bearer {accessToken}");

            // Adding body content in HTTP request   
            if (body != null)
                message.Content = new StringContent(body, Encoding.UTF8, "application/json");

            return await client.SendAsync(message);
        }
    }
}