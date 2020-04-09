using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MDMProject.Models
{
    public class Address
    {
        public long Id { get; set; }
        public string City { get; set; }
        public string StreetName { get; set; }
        public string HouseNumber { get; set; }
        public string FlatNumber { get; set; }
        public string PostalCode { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
    }
}