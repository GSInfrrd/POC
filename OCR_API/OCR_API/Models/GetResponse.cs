using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OCR_API.Models
{
	public class Field1
	{
		public string name { get; set; }
		public string values { get; set; }
	}

	public class Data1
	{
		public string scanId { get; set; }
		public List<Field1> fields { get; set; }
		public string s3ImagePath { get; set; }
	}

	public class GetResponse
	{
		public string status { get; set; }
		public Data1 data { get; set; }
	}
}