using System;
using System.Collections.Generic;
using SendGrid.CSharp.HTTP.Client;

namespace Example
{
    class Example
    {
        static void Main(string[] args)
        {
            String host = "https://e9sk3d3bfaikbpdq7.stoplight-proxy.io";
            Dictionary<String, String> requestHeaders = new Dictionary<String, String>();
            requestHeaders.Add("X-Test", "test");
            String version = "v3";
            dynamic client = new Client(host, requestHeaders, version);

            dynamic response0 = client.asm.suppressions.global._("test1+v2@example.com").Get();
            Console.WriteLine(response0.Content.ReadAsStringAsync().Result);

            dynamic response1 = client.api_keys;
            dynamic response2 = response1.Get();
            Console.WriteLine(response2.Content.ReadAsStringAsync().Result);

            Console.WriteLine("\n\nPress any key to continue.");
            Console.ReadLine();
        }
    }
}
