﻿using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
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
            if(binder.Name == "Get")
            {
                result = RequestAsync(Methods.GET).Result;
            }
            else
            {
                result = null;
            }
            return true;
        }

        private async Task<Response> RequestAsync(Methods method, string endpoint = null, String data = null)
        {
            using (var client = new HttpClient())
            {
                try
                {
                    client.BaseAddress = new Uri(_host);
                    client.DefaultRequestHeaders.Accept.Clear();
                    _apiKey = Environment.GetEnvironmentVariable("SENDGRID_APIKEY", EnvironmentVariableTarget.User);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", this._apiKey);

                    switch (method)
                    {
                        case Methods.GET:
                            endpoint = BuildUrl();
                            HttpResponseMessage response = await client.GetAsync(endpoint);
                            return new Response(response.StatusCode, response.Content, response.Headers);
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
                    return new Response(response.StatusCode, response.Content, response.Headers);
                }
            }
        }
    }
}
