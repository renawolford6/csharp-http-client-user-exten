using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Web;

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

        private string BuildUrl(string query_params = null)
        {
            string endpoint = null;
            if( _version != null)
            {
                endpoint = _host + "/" + _version + _urlPath;
            }
            else
            {
                endpoint = _host + _urlPath;
            }

            if (query_params != null)
            {
                JavaScriptSerializer jss = new JavaScriptSerializer();
                var ds_query_params = jss.Deserialize<Dictionary<string, dynamic>>(query_params);
                var query = HttpUtility.ParseQueryString(string.Empty);
                foreach (var pair in ds_query_params)
                {
                    query[pair.Key] = pair.Value.ToString();
                }
                string queryString = query.ToString();
                endpoint = endpoint + "?" + queryString;
            }
            
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

        private void AddVersion(string version)
        {
            _version = version;
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
            if (binder.Name == "version")
            {
                AddVersion(args[0].ToString());
                result = BuildClient();
                return true;
            }

            var paramDict = new Dictionary<string, object>();
            string query_params = null;
            string request_body = null;
            int i = 0;
            foreach (object obj in args)
            {
                string name = binder.CallInfo.ArgumentNames.Count > i ?
                   binder.CallInfo.ArgumentNames[i] : null;
                if(name == "query_params")
                {
                    query_params = obj.ToString();
                }
                else if (name == "request_body")
                {
                    request_body = obj.ToString();
                }
                i++;
            }

            if( Enum.IsDefined(typeof(Methods), binder.Name.ToUpper()))
            {
                result = RequestAsync(binder.Name.ToUpper(), request_body: request_body, query_params: query_params).Result;
                return true;
            }
            else
            {
                result = null;
                return false;
            }
 
        }

        private async Task<Response> RequestAsync(string method, String request_body = null, String query_params = null)
        {
            using (var client = new HttpClient())
            {
                try
                {
                    client.BaseAddress = new Uri(_host);
                    string endpoint = BuildUrl(query_params);
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

                    StringContent content = null;
                    if (request_body != null)
                    {
                        content = new StringContent(request_body.ToString().Replace("'", "\""), Encoding.UTF8, MediaType);
                    }

                    HttpRequestMessage request = new HttpRequestMessage
                    {
                        Method = new HttpMethod(method),
                        RequestUri = new Uri(endpoint),
                        Content = content
                    };
                    HttpResponseMessage response = await client.SendAsync(request);
                    return new Response(response.StatusCode, response.Content, response.Headers);

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
