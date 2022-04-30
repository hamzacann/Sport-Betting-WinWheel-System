using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.Sql;
using System.Data.SqlClient;

namespace katarfelek.Models
{
    public class RequestClass
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Prize { get; set; }
        public DateTime RequestTime { get; set; }
    }
}