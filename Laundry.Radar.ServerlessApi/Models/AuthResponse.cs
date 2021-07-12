using System;
using System.Collections.Generic;
using System.Text;

namespace Laundry.Radar.ServerlessApi.Models
{
    public class AuthResponse
    {
        public AccessToken accessToken { get; set; }
        public string refreshToken { get; set; }
    }
}
