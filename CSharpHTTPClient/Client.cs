using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace SendGrid.CSharp.HTTP.Client
{
    public class Response
    {
        public HttpStatusCode StatusCode;
        public HttpContent ResponseBody;
        public HttpResponseHeaders ResponseHeaders;

        public Response(HttpStatusCode statusCode, HttpContent responseBody, HttpResponseHeaders responseHeaders)
        {
            StatusCode = statusCode;
            ResponseBody = responseBody;
            ResponseHeaders = responseHeaders;
        }
    }

    public class Client : DynamicObject
    {
        private string _apiKey;
        private string _host;
        private Dictionary <string,string> _requestHeaders;
        private string _version;
        private string _urlPath;
        public string MediaType;
        public enum Methods
        {
            DELETE, GET, PATCH, POST, PUT
        }

        public Client(string host, Dictionary<string,string> requestHeaders = null, string version = null, string urlPath = null)
        {
            _host = host;
            if(requestHeaders != null)
            {
                _requestHeaders = (_requestHeaders != null)
                    ? _requestHeaders.Union(requestHeaders).ToDictionary(pair => pair.Key, pair => pair.Value) : requestHeaders;
            }
            _version = (version != null) ? version : null;
            _urlPath = (urlPath != null) ? urlPath : null;
        }

        private string BuildUrl()
        {
            string endpoint = _host + "/" + _version + _urlPath;
            return endpoint;
        }

        private Client BuildClient(string name = null)
        {
            string endpoint;
            if (name != null)
            {
                endpoint = _urlPath + "/" + name;
            }
            else
            {
                endpoint = _urlPath;
            }
            _urlPath = null; // Reset the current object's state before we return a new one
            return new Client(_host, _requestHeaders, _version, endpoint);
        }

        // Magic method to handle special cases
        public Client _(string magic)
        {
            return BuildClient(magic);
        }

        // Reflection
        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            result = BuildClient(binder.Name);
            return true;
        }

        // Catch final method call
        public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
        {
            switch(binder.Name.ToUpper())
            {
                case("GET"):
                    result = RequestAsync(Methods.GET).Result;
                    return true;
                case ("PUT"):
                    result = RequestAsync(Methods.PUT).Result;
                    return true;
                case ("PATCH"):
                    result = RequestAsync(Methods.PATCH).Result;
                    return true;
                case ("POST"):
                    result = RequestAsync(Methods.POST).Result;
                    return true;
                case ("DELETE"):
                    result = RequestAsync(Methods.POST).Result;
                    return true;
                default:
                    result = null;
                    return false;
            }
        }

        private async Task<Response> RequestAsync(Methods method, string endpoint = null, String data = null)
        {
            using (var client = new HttpClient())
            {
                try
                {
                    client.BaseAddress = new Uri(_host);
                    client.DefaultRequestHeaders.Accept.Clear();
                    foreach (KeyValuePair<string, string> header in _requestHeaders)
                    {
                        if(header.Key == "Authorization")
                        {
                            string[] split = header.Value.Split(new char[0]);
                            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(split[0], split[1]); ;
                        }
                        else if(header.Key == "Content-Type")
                        {
                            MediaType = header.Value;
                            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(MediaType));
                        }
                        else
                        {
                            client.DefaultRequestHeaders.Add(header.Key, header.Value);
                        }
                    }

                    switch (method)
                    {
                        case Methods.GET:
                            endpoint = BuildUrl();
                            HttpResponseMessage response = await client.GetAsync(endpoint);
                            return new Response(response.StatusCode, response.Content, response.Headers);
                        case Methods.POST:
                            response = await client.PostAsJsonAsync(endpoint, data);
                            return new Response(response.StatusCode, response.Content, response.Headers);
                        case Methods.PUT:
                            HttpContent put_data = new StringContent(data, Encoding.UTF8, MediaType);
                            response = await client.PutAsync(endpoint, put_data);
                            return new Response(response.StatusCode, response.Content, response.Headers);
                        case Methods.PATCH:
                            endpoint = _host + endpoint;
                            StringContent content = new StringContent(data.ToString(), Encoding.UTF8, MediaType);
                            HttpRequestMessage request = new HttpRequestMessage
                            {
                                Method = new HttpMethod("PATCH"),
                                RequestUri = new Uri(endpoint),
                                Content = content
                            };
                            response = await client.SendAsync(request);
                            return new Response(response.StatusCode, response.Content, response.Headers);
                        case Methods.DELETE:
                            response = await client.DeleteAsync(endpoint);
                            return new Response(response.StatusCode, response.Content, response.Headers);
                        default:
                            response = new HttpResponseMessage();
                            response.StatusCode = HttpStatusCode.MethodNotAllowed;
                            var message = "{\"errors\":[{\"message\":\"Bad method call, supported methods are GET, POST, PUT, PATCH and DELETE\"}]}";
                            response.Content = new StringContent(message);
                            return new Response(response.StatusCode, response.Content, response.Headers);
                    }
                }
                catch (Exception ex)
                {
                    HttpResponseMessage response = new HttpResponseMessage();
                    string message;
                    message = (ex is HttpRequestException) ? ".NET HttpRequestException" : ".NET Exception";
                    message = message + ", raw message: \n\n";
                    response.Content = new StringContent(message + ex.Message);
                    return new Response(response.StatusCode, response.Content, response.Headers);
                }
            }
        }
    }
}
