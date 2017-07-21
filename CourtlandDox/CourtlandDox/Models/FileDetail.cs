using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CourtlandDox.Models
{
    public class FileDetail
    {
    }
    public class Field
    {
        public string name { get; set; }
        public string values { get; set; }
    }

    public class Data
    {
        public string scanId { get; set; }
        public List<Field> fields { get; set; }
        public string s3ImagePath { get; set; }
        public string text { get; set; }
        public string xml { get; set; }
    }

    public class RootObject
    {
        public string status { get; set; }
        public Data data { get; set; }
    }
}