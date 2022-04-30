using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace katarfelek.Models
{
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class responseClass
    {
        public bool status { get; set; }
        public double addBalance { get; set; }
        public double newBalance { get; set; }
        public string errorMsg { get; set; }
    }


}