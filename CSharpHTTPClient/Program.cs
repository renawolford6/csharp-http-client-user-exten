using System;
using System.Dynamic;

namespace CSharpHTTPClient
{
    public class Client : DynamicObject
    {
        public string url_path_name;
        
        public Client(string url_path_name = null)
        {
            if(url_path_name != null)
            {
                this.url_path_name = url_path_name;
            }
        }

        // Magic method to handle special cases
        public Client _(String magic)
        {
            this.url_path_name += magic + "/";
            return new Client(this.url_path_name);
        }

        // Reflection
        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            this.url_path_name += binder.Name + "/";
            result = new Client(this.url_path_name);
            return true;
        }

        // Catch final method call
        public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
        {
            if(binder.Name == "Get")
            {
                this.url_path_name += "GET";
            }
            result = new Client(this.url_path_name);
            return true;
        }
    }
}
