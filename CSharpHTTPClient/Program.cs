using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Threading.Tasks;

namespace CSharpHTTPClient
{
    public class Client : DynamicObject
    {
        private String _apiKey;
        private String _host;
        private Dictionary <String,String> _request_headers;
        private String _version;
        private const string MediaType = "application/json";
        public enum Methods
        {
            DELETE, GET, PATCH, POST, PUT
        }
        public String _url_path;

        public Client(String host, Dictionary<string,string> request_headers = null, string version = null, string url_path = null)
        {
            _host = host;
            if(request_headers != null)
            {
                _request_headers = (_request_headers != null)
                    ? _request_headers.Union(request_headers).ToDictionary(pair => pair.Key, pair => pair.Value) : request_headers;
            }
            _version = (version != null) ? version : null;
            _url_path = (url_path != null) ? url_path : null;
        }

        public String build_url()
        {
            String endpoint = _host + "/" + _version + _url_path;
            return endpoint;
        }

        public Client build_client(String name = null)
        {
            String endpoint;
            if (name != null)
            {
                endpoint = _url_path + "/" + name;
            }
            else
            {
                endpoint = _url_path;
            }
            Console.WriteLine("Building the URL: " + _url_path);
            _url_path = null;
            return new Client(_host, _request_headers, _version, endpoint);
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
                result = RequestAsync(Methods.GET).Result;
            }
            else
            {
                Console.WriteLine("Should not get here");
                result = null;
            }
            return true;
        }

        private async Task<HttpResponseMessage> RequestAsync(Methods method, string endpoint = null, JObject data = null)
        {
            using (var client = new HttpClient())
            {
                try
                {
                    client.BaseAddress = new Uri(_host);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(MediaType));
                    _apiKey = Environment.GetEnvironmentVariable("SENDGRID_APIKEY", EnvironmentVariableTarget.User);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", this._apiKey);

                    switch (method)
                    {
                        case Methods.GET:
                            endpoint = build_url();
                            Console.WriteLine("Endpoint" + endpoint);
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
    }
}
