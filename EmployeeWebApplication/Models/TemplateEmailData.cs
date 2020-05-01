using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeWebApplication.Models
{
    public class TemplateEmailData
    {
        [JsonProperty("subject")]
        public string Subject { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("redirect")]
        public string RedirectUrl { get; set; }
    }
    public class Location
    {
        [JsonProperty("city")]
        public string City { get; set; }
        [JsonProperty("country")]
        public string Country { get; set; }
    }
}
