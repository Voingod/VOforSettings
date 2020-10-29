using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Net;
using System.Linq;
using System.Text.Json.Serialization;
using System.Collections.Generic;

namespace D365S2S
{
    class Program
    {
        static void Main(string[] args)
        {


            StartCrmRequest();

            Console.ReadLine();
        }



        public static void StartCrmRequest()
        {
            var contacts = CrmRequest(
                HttpMethod.Get,
                "https://udstrialsdemo40.crm4.dynamics.com/api/data/v9.1/contacts"
                ).Result.Content.ReadAsStringAsync(); 
            // Similarly you can make POST, PATCH & DELETE requests 
            Console.WriteLine(contacts.Result);
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


    public class ContactsModel
    {
        [JsonPropertyName("customertypecode")]
        public int CustomerTypeCode { get; set; }

        [JsonPropertyName("address1_addressid")]
        public string Address1AddressId { get; set; }

        [JsonPropertyName("address2_addressid")]
        public string Address2AddressId { get; set; }

        [JsonPropertyName("address3_addressid")]
        public string Address3AddressId { get; set; }

        [JsonPropertyName("contactid")]
        public string ContactId { get; set; }

        [JsonPropertyName("createdon")]
        public string CreatedOn { get; set; }

        [JsonPropertyName("firstname")]
        public string FirstName { get; set; }

        [JsonPropertyName("lastname")]
        public string LastName { get; set; }

        [JsonPropertyName("statecode")]
        public int StateCode { get; set; }

        [JsonPropertyName("statuscode")]
        public int StatusCode { get; set; }

        [JsonPropertyName("emailaddress1")]
        public string EmailAddress { get; set; }
    }

    public class DynamicsEntityCollection<T>
    {
        public IList<T> Value { get; set; }
    }
}