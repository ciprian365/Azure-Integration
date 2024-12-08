using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ManagedIdentityDemo.Model
{
    internal class Account
    {

        [JsonRequired]
        public string? AccountName{ get; set; }
        [JsonRequired]
        public string? Email { get; set; }
    }
}
