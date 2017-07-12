using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OCR_API.Models
{
	public class Field
	{
		public string Name { get; set; }
		public string Values { get; set; }
	}

	public class Data
	{
		public string ScanId { get; set; }
		public string S3ImagePath { get; set; }
	}

	public class RootObject
	{
		public string Status { get; set; }
		public Data Data { get; set; }
	}
}