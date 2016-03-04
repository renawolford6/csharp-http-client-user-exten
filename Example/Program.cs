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
            // dynamic temp = client.Foo;
            dynamic ret = client.api_keys.Get();
            Console.WriteLine(ret.Content.ReadAsStringAsync().Result);
            // dynamic ret2 = temp.Will.Do.Get();
            // String string2 = ret2.url_path;
            // Console.WriteLine("After second Get: " + string2);
            Console.WriteLine("\n\nPress any key to continue.");
            Console.ReadLine();
        }
    }
}
