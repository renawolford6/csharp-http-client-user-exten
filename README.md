[![Travis Badge](https://travis-ci.org/sendgrid/csharp-http-client.svg?branch=master)](https://travis-ci.org/sendgrid/python-http-client)

**Quickly and easily access any REST or REST-like API.**

Here is a quick example:

`GET /your/api/{param}/call`

```csharp
using SendGrid.CSharp.HTTP.Client;
globalRequestHeaders.Add("Authorization", "Bearer XXXXXXX");
dynamic client = new Client(host: baseUrl, requestHeaders: globalRequestHeaders);
client.your.api._(param).call.get()
Console.WriteLine(response.StatusCode);
Console.WriteLine(response.ResponseBody.ReadAsStringAsync().Result);
Console.WriteLine(response.ResponseHeaders.ToString());
```

`POST /your/api/{param}/call` with headers, query parameters and a request body with versioning.

```csharp
using SendGrid.CSharp.HTTP.Client;
globalRequestHeaders.Add("Authorization", "Bearer XXXXXXX");
dynamic client = new Client(host: baseUrl, requestHeaders: globalRequestHeaders);
string queryParams = "{'Hello': 0, 'World': 1}";
requestHeaders.Add("X-Test", "test");
string requestBody = "{'some': 1, 'awesome': 2, 'data': 3}";
var response = client.your.api._(param).call.post(requestBody: requestBody,
                                                  queryParams: queryParams,
                                                  requestHeaders: requestHeaders)
Console.WriteLine(response.StatusCode);
Console.WriteLine(response.ResponseBody.ReadAsStringAsync().Result);
Console.WriteLine(response.ResponseHeaders.ToString());
```

# Installation

To use CSharp.HTTP.Client in your C# project, you can either <a href="https://github.com/sendgrid/csharp-http-client.git">download the SendGrid C# .NET libraries directly from our Github repository</a> or, if you have the NuGet package manager installed, you can grab them automatically.

```
PM> Install-Package SendGrid.CSharp.Http.Client 
```

Once you have the library properly referenced in your project, you can include calls to them in your code. 
For a sample implementation, check the [Example](https://github.com/sendgrid/csharp-http-client/tree/master/Example) folder.

Add the following namespace to use the library:
```csharp
using SendGrid.CSharp.HTTP.Client;
```

## Usage ##

Following is an example using SendGrid. You can get your free account [here](https://sendgrid.com/free?source=csharp-http-client).

First, update your Environment Variable with your [SENDGRID_APIKEY](https://app.sendgrid.com/settings/api_keys). 

Following is an abridged example, here is the [full working code](https://github.com/sendgrid/csharp-http-client/blob/master/Example/Example.cs).

```csharp
using System;
using System.Collections.Generic;
using SendGrid.CSharp.HTTP.Client;
using System.Web.Script.Serialization;

class Example
{
    static void Main(string[] args)
    {
        String host = "https://api.sendgrid.com";
        Dictionary<String, String> requestHeaders = new Dictionary<String, String>();
        string apiKey = Environment.GetEnvironmentVariable("SENDGRID_APIKEY", EnvironmentVariableTarget.User);
        requestHeaders.Add("Authorization", "Bearer " + apiKey);
        requestHeaders.Add("Content-Type", "application/json");

        String version = "v3";
        dynamic client = new Client(host, requestHeaders, version);

        // GET Collection
        string queryParams = @"{
            'limit': 100
        }";
        dynamic response = client.version("v3").api_keys.get(queryParams: queryParams);
        var dssResponseBody = response.DeserializeResponseBody(response.ResponseBody);
        foreach ( var value in dssResponseBody["result"])
        {
            Console.WriteLine("name: {0}, api_key_id: {1}",value["name"], value["api_key_id"]);
        }

        var dssResponseHeaders = response.DeserializeResponseHeaders(response.ResponseHeaders);
        foreach (var pair in dssResponseHeaders)
        {
            Console.WriteLine("{0}: {1}", pair.Key, pair.Value);
        }

        Console.WriteLine("\n\nPress any key to continue to POST.");
        Console.ReadLine();

        // POST
        string requestBody = @"{
            'name': 'My API Key 5',
            'scopes': [
                'mail.send',
                'alerts.create',
                'alerts.read'
            ]
        }";
        response = client.api_keys.post(requestBody: requestBody);
        Console.WriteLine(response.StatusCode);
        Console.WriteLine(response.ResponseBody.ReadAsStringAsync().Result);
        Console.WriteLine(response.ResponseHeaders.ToString());
        JavaScriptSerializer jss = new JavaScriptSerializer();
        var ds_response = jss.Deserialize<Dictionary<string, dynamic>>(response.ResponseBody.ReadAsStringAsync().Result);
        string api_key_id = ds_response["api_key_id"];

        Console.WriteLine("\n\nPress any key to continue to GET single.");
        Console.ReadLine();

        // GET Single
        response = client.api_keys._(api_key_id).get();
        Console.WriteLine(response.StatusCode);
        Console.WriteLine(response.ResponseBody.ReadAsStringAsync().Result);
        Console.WriteLine(response.ResponseHeaders.ToString());

        Console.WriteLine("\n\nPress any key to continue to PATCH.");
        Console.ReadLine();

        // PATCH
        request_body = @"{
            'name': 'A New Hope'
        }";
        response = client.api_keys._(api_key_id).patch(requestBody: requestBody);
        Console.WriteLine(response.StatusCode);
        Console.WriteLine(response.ResponseBody.ReadAsStringAsync().Result);
        Console.WriteLine(response.ResponseHeaders.ToString());

        Console.WriteLine("\n\nPress any key to continue to PUT.");
        Console.ReadLine();

        // PUT
        request_body = @"{
            'name': 'A New Hope',
            'scopes': [
                'user.profile.read',
                'user.profile.update'
            ]
        }";
        response = client.api_keys._(api_key_id).put(requestBody: requestBody);
        Console.WriteLine(response.StatusCode);
        Console.WriteLine(response.ResponseBody.ReadAsStringAsync().Result);
        Console.WriteLine(response.ResponseHeaders.ToString());

        Console.WriteLine("\n\nPress any key to continue to DELETE.");
        Console.ReadLine();

        // DELETE
        response = client.api_keys._(api_key_id).delete();
        Console.WriteLine(response.StatusCode);
        Console.WriteLine(response.ResponseHeaders.ToString());

        Console.WriteLine("\n\nPress any key to exit.");
        Console.ReadLine();
    }
}
```

# Announcements

[2016.XX.XX] - We hit version 1!

# Roadmap

[Milestones](https://github.com/sendgrid/csharp-http-client/milestones)

# How to Contribute

We encourage contribution to our libraries, please see our [CONTRIBUTING](https://github.com/sendgrid/csharp-http-client/blob/master/CONTRIBUTING.md) guide for details.

* [Feature Request](https://github.com/sendgrid/csharp-http-client/blob/master/CONTRIBUTING.md#feature_request)
* [Bug Reports](https://github.com/sendgrid/csharp-http-client/blob/master/CONTRIBUTING.md#submit_a_bug_report)
* [Improvements to the Codebase](https://github.com/sendgrid/csharp-http-client/blob/master/CONTRIBUTING.md#improvements_to_the_codebase)

# Thanks

We were inspired by the work done on [birdy](https://github.com/inueni/birdy) and [universalclient](https://github.com/dgreisen/universalclient).

# About

![SendGrid Logo]
(https://assets3.sendgrid.com/mkt/assets/logos_brands/small/sglogo_2015_blue-9c87423c2ff2ff393ebce1ab3bd018a4.png)

csharp-http-client is guided and supported by the SendGrid [Developer Experience Team](mailto:dx@sendgrid.com).

csharp-http-client is maintained and funded by SendGrid, Inc. The names and logos for python-http-client are trademarks of SendGrid, Inc.