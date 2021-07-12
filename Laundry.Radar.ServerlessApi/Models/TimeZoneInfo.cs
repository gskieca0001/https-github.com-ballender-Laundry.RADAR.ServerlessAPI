using System;
using System.Collections.Generic;
using System.Text;

namespace Laundry.Radar.ServerlessApi.Models
{
    public class TimeZoneInfo
    {
        public string CountryCode { get; set; }
        public string CountryName { get; set; }
        public TimeSpan DstOffset { get; set; }
        public TimeSpan Offset { get; set; }
        public string TimeZoneId { get; set; }
    }
}
