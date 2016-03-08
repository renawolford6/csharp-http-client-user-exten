using System;
using System.Collections.Generic;
using SendGrid.CSharp.HTTP.Client;
using System.Web.Script.Serialization;

namespace Example
{
    class Example
    {
        static void Main(string[] args)
        {
            String host = "https://e9sk3d3bfaikbpdq7.stoplight-proxy.io";
            Dictionary<String, String> requestHeaders = new Dictionary<String, String>();
            string apiKey = Environment.GetEnvironmentVariable("SENDGRID_APIKEY", EnvironmentVariableTarget.User);
            requestHeaders.Add("Authorization", "Bearer " + apiKey);
            requestHeaders.Add("Content-Type", "application/json");

            String version = "v3";
            dynamic client = new Client(host, requestHeaders, version);

            // GET Collection
            string query_params = @"{
                'limit': 100
            }";
            dynamic response = client.version("v3").api_keys.get(query_params: query_params);
            //Console.WriteLine(response.StatusCode);
            //Console.WriteLine(response.ResponseBody.ReadAsStringAsync().Result);
            //Console.WriteLine(response.ResponseHeaders.ToString());

            foreach ( var value in response.DSResponseBody["result"])
            {
                Console.WriteLine("name: {0}, api_key_id: {1}",value["name"], value["api_key_id"]);
            }
            foreach (var pair in response.DSResponseHeaders)
            {
                Console.WriteLine("{0}: {1}", pair.Key, pair.Value);
            }
            

            Console.WriteLine("\n\nPress any key to continue.");
            Console.ReadLine();

            // POST
            string request_body = @"{
                'name': 'My API Key 5',
                'scopes': [
                    'mail.send',
                    'alerts.create',
                    'alerts.read'
                ]
            }";
            response = client.api_keys.post(request_body: request_body);
            Console.WriteLine(response.StatusCode);
            Console.WriteLine(response.ResponseBody.ReadAsStringAsync().Result);
            Console.WriteLine(response.ResponseHeaders.ToString());
            JavaScriptSerializer jss = new JavaScriptSerializer();
            var ds_response = jss.Deserialize<Dictionary<string, dynamic>>(response.ResponseBody.ReadAsStringAsync().Result);
            string api_key_id = ds_response["api_key_id"];

            Console.WriteLine("\n\nPress any key to continue.");
            Console.ReadLine();

            // GET Single
            response = client.api_keys._(api_key_id).get();
            Console.WriteLine(response.StatusCode);
            Console.WriteLine(response.ResponseBody.ReadAsStringAsync().Result);
            Console.WriteLine(response.ResponseHeaders.ToString());

            Console.WriteLine("\n\nPress any key to continue.");
            Console.ReadLine();

            // PATCH
            request_body = @"{
                'name': 'A New Hope'
            }";
            response = client.api_keys._(api_key_id).patch(request_body: request_body);
            Console.WriteLine(response.StatusCode);
            Console.WriteLine(response.ResponseBody.ReadAsStringAsync().Result);
            Console.WriteLine(response.ResponseHeaders.ToString());

            Console.WriteLine("\n\nPress any key to continue.");
            Console.ReadLine();

            // PUT
            request_body = @"{
                'name': 'A New Hope',
                'scopes': [
                    'user.profile.read',
                    'user.profile.update'
                ]
            }";
            response = client.api_keys._(api_key_id).put(request_body: request_body);
            Console.WriteLine(response.StatusCode);
            Console.WriteLine(response.ResponseBody.ReadAsStringAsync().Result);
            Console.WriteLine(response.ResponseHeaders.ToString());

            Console.WriteLine("\n\nPress any key to continue.");
            Console.ReadLine();

            // DELETE
            response = client.api_keys._(api_key_id).delete();
            Console.WriteLine(response.StatusCode);
            Console.WriteLine(response.ResponseHeaders.ToString());

            Console.WriteLine("\n\nPress any key to continue.");
            Console.ReadLine();
        }
    }
}
