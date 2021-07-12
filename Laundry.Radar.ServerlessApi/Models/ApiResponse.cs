using System;
using System.Collections.Generic;
using System.Text;

namespace Laundry.Radar.ServerlessApi.Models
{
    public class ApiResponse
    {
        public List<object> Entities { get; set; }
        public DateTime? RequestDateTime { get; set; }
        public DateTime? ResponseDateTime { get; set; }
        public int? TotalResults { get; set; }
        public bool? Success { get; set; }
    }
}
