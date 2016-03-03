using System;

namespace Example
{
    class Program
    {
        static void Main(string[] args)
        {
            dynamic client = new CSharpHTTPClient.Client();
            dynamic temp = client.Foo;
            dynamic ret = client.Foo.Bar.Will.Do._("magic").Get();
            String string1 = ret.url_path_name;
            Console.WriteLine("After first Get: " + string1);
            dynamic ret2 = temp.Will.Do.Get();
            String string2 = ret2.url_path_name;
            Console.WriteLine("After second Get: " + string2);
            Console.WriteLine("\n\nPress any key to continue.");
            Console.ReadLine();
        }
    }
}
