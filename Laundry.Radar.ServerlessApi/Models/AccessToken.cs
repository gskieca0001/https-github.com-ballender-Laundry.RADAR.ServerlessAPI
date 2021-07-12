using System;
using System.Collections.Generic;
using System.Text;

namespace Laundry.Radar.ServerlessApi.Models
{
    public class AccessToken
    {
        public string token { get; set; }
        public int expiresIn { get; set; }
    }
}
