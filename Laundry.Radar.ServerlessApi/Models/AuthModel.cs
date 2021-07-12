using System;
using System.Collections.Generic;
using System.Text;

namespace Laundry.Radar.ServerlessApi.Models
{
    public class AuthModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string ClientCode { get; set; }
    }
}
