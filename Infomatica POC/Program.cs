using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Configuration;

namespace Infomatica_POC
{
    class Program
    {
        

        private static string URL = ConfigurationManager.AppSettings["URL"];
        private static string urlParameters = ConfigurationManager.AppSettings["urlParameters"];

        public static object Username { get; private set; } = ConfigurationManager.AppSettings["Username"];
        public static object Password { get; private set; } = ConfigurationManager.AppSettings["Password"];
        public static string EntityName { get; private set; } = ConfigurationManager.AppSettings["EntityName"];

        static void Main(string[] args)
        {
            
            var watch = new System.Diagnostics.Stopwatch();
            using (var handler = new WinHttpHandler())
            {
                handler.ReceiveDataTimeout = new TimeSpan(0, 2, 0);
                handler.ReceiveHeadersTimeout = new TimeSpan(0, 2, 0);
                handler.SendTimeout = new TimeSpan(0, 2, 0);
                using (var client = new HttpClient(handler))
                {
                    client.BaseAddress = new Uri(URL);

                    // Add an Accept header for JSON format.
                    client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Add($"Authorization", $"Basic {Base64Encode($"{Username}:{Password}")}");
                    //HttpContent Content = new StringContent("{\"name\":\"John Doe\",\"age\":33}", Encoding.UTF8, "application/json");
                    var request = new HttpRequestMessage(HttpMethod.Get, urlParameters)
                    {
                        Content = new StringContent("\"enityInfo\":{\"input\":{\"enityName\":\""+ EntityName +"\"}}", Encoding.UTF8, "application/json"),
                    };
                    // List data response.
                    watch.Start();
                    HttpResponseMessage response = client.SendAsync(request).Result;  // Blocking call! Program will wait here until a response is received or a timeout occurs.
                    Console.WriteLine("========= Request ElapsedMilliseconds inside try ========\n Time elapsed : " + watch.Elapsed);
                    if (response.IsSuccessStatusCode)
                    {
                        // Parse the response body.
                        var dataObjects = response.Content.ReadAsAsync<object>().Result;
                        //var dataObjects = response.Content.ReadAsAsync<PRResponse>().Result;  //Make sure to add a reference to System.Net.Http.Formatting.dll
                        //Console.WriteLine($"EntityName = {EntityName}");
                        //Console.WriteLine($"Message = {dataObjects.Response.Data.Messages}");
                        //Console.WriteLine($"Success = {dataObjects.Response.Data.Success}");
                        //Console.WriteLine("===================================================");
                        //Console.WriteLine($"Total Count = {dataObjects.Response.Data.Results.Count()}");
                        //foreach (Entity entity in dataObjects.Response.Data.Results)
                        //{
                        //    Console.WriteLine($"EntityName = {entity.EntityName}\nEntityNumber = {entity.EntityNumber}\nEntitySFId = {entity.EntitySFId}\nEntityType = {entity.EntityType}");
                        //    Console.WriteLine("Contacts ->");
                        //    foreach (PREntityContact contact in entity.Contacts)
                        //    {
                        //        Console.WriteLine($"\tFirstName = {contact.FirstName}\n\tLastName = {contact.LastName}\n\tEmail = {contact.Email}");
                        //        Console.WriteLine("-----------------------------------");
                        //    }
                        //    Console.WriteLine("=================================================");
                        //}
                        //Console.WriteLine($"Data = {dataObjects.Response.Data.Results.}")
                        //var data = JsonConvert.SerializeObject(dataObjects);
                        Console.WriteLine(dataObjects);


                    }
                    else
                    {
                        Console.WriteLine("{0} ({1})", (int)response.StatusCode, response.ReasonPhrase);
                    }

                    // Make any other calls using HttpClient here.
                    Console.ReadLine();
                    // Dispose once all HttpClient calls are complete. This is not necessary if the containing object will be disposed of; for example in this case the HttpClient instance will be disposed automatically when the application terminates so the following call is superfluous.
                }
            }
        }

        private static object Base64Encode(string textToEncode)
        {
            byte[] textAsBytes = Encoding.UTF8.GetBytes(textToEncode);
            return Convert.ToBase64String(textAsBytes);
        }

        public class PRResponse
        { 

            [JsonProperty("response")]
            public Result Response { get; set; }
        }

        public class Result
        {
            [JsonProperty("result")]
            public ResultData Data { get; set; }
        }

    public class ResultData
        {
            [JsonProperty("Messages")]
            public string Messages { get; set; }
            [JsonProperty("Success")]
            public bool Success { get; set; }
            [JsonProperty("results")]
            public IEnumerable<Entity> Results { get; set; }
        }
        public class Entity
        {
            [JsonProperty("entityName")]
            public string EntityName { get; set; }
            [JsonProperty("entityNumber")]
            public string EntityNumber { get; set; }
            [JsonProperty("entitySFId")]
            public string EntitySFId { get; set; }
            [JsonProperty("entityType")]
            public string EntityType { get; set; }
            [JsonProperty("contacts")]
            public IEnumerable<PREntityContact> Contacts { get; set; }
        }
        public class PREntityContact
        {
            [JsonProperty("email")]
            public string Email { get; set; }
            [JsonProperty("firstName")]
            public string FirstName { get; set; }
            [JsonProperty("lastName")]
            public string LastName { get; set; }

        }
    }
}
