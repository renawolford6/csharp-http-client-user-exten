using System;
using System.Collections.Generic;

namespace Example
{
    class Program
    {
        static void Main(string[] args)
        {
            String host = "https://e9sk3d3bfaikbpdq7.stoplight-proxy.io";
            Dictionary<String, String> request_headers = new Dictionary<String, String>();
            request_headers.Add("X-Test", "test");
            String version = "v3";
            dynamic client = new CSharpHTTPClient.Client(host, request_headers, version);

            dynamic ret2 = client.asm.suppressions.global._("test1+v2@example.com").Get();
            Console.WriteLine(ret2.Content.ReadAsStringAsync().Result);

            dynamic ret = client.api_keys;
            dynamic ret3 = ret.Get();
            Console.WriteLine(ret3.Content.ReadAsStringAsync().Result);

            Console.WriteLine("\n\nPress any key to continue.");
            Console.ReadLine();
        }
    }
}
