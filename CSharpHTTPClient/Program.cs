using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace CSharpHTTPClient
{
    public class Client : DynamicObject
    {
        private String _apiKey;
        private String _host;
        private Dictionary <String,String> request_headers;
        private String version;
        private const string MediaType = "application/json";
        public enum Methods
        {
            DELETE, GET, PATCH, POST, PUT
        }
        public string url_path;

        public Client(String host, Dictionary<string,string> request_headers = null, string version = null, string url_path = null)
        {
            this._host = host;
            if (this.request_headers != null && request_headers != null)
            {
                this.request_headers.Union(request_headers);
            } else if (this.request_headers == null && request_headers != null)
            {
                this.request_headers = request_headers;
            }

            if (version != null)
            {
                this.version = version;
            }
            
            if (url_path != null)
            {
                this.url_path = url_path;
            }
        }

        public String build_url()
        {
            Console.WriteLine(this._host + "/" + this.version + this.url_path);
            return this._host + "/" + this.version + this.url_path;
        }

        public Client build_client(String name = null)
        {
            if (name != null)
            {
                this.url_path += "/" + name;
            }
            return new Client(this._host, this.request_headers, this.version, this.url_path);
        }

        // Magic method to handle special cases
        public Client _(String magic)
        {
            return build_client(magic);
        }

        // Reflection
        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            result = build_client(binder.Name);
            return true;
        }

        // Catch final method call
        public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
        {
            if(binder.Name == "Get")
            {
                result = this.RequestAsync(Methods.GET);
            }
            result = this.RequestAsync(Methods.GET).Result;
            // result = null;
            return true;
        }

        private async Task<HttpResponseMessage> RequestAsync(Methods method, string endpoint = null, JObject data = null)
        {
            using (var client = new HttpClient())
            {
                try
                {
                    client.BaseAddress = new Uri(this._host);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(MediaType));
                    this._apiKey = Environment.GetEnvironmentVariable("SENDGRID_APIKEY", EnvironmentVariableTarget.User);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", this._apiKey);

                    switch (method)
                    {
                        case Methods.GET:
                            endpoint = this.build_url();
                            Console.WriteLine("endpoint " + endpoint);
                            return await client.GetAsync(endpoint);
                    }
                    return null;
                }
                catch (Exception ex)
                {
                    HttpResponseMessage response = new HttpResponseMessage();
                    string message;
                    message = (ex is HttpRequestException) ? ".NET HttpRequestException" : ".NET Exception";
                    message = message + ", raw message: \n\n";
                    response.Content = new StringContent(message + ex.Message);
                    return response;
                }
            }
        }

        public async Task<HttpResponseMessage> Get(string endpoint)
        {
            return await RequestAsync(Methods.GET, endpoint, null);
        }
    }
}
