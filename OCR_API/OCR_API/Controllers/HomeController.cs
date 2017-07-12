using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Threading;
using Newtonsoft.Json;
using OCR_API.Models;
using OCR_API.Properties;

namespace OCR_API.Controllers
{
	public class HomeController : Controller
	{
		private static readonly HttpClient restClient = new HttpClient();
		public ActionResult Index()
		{
			return View();
		}

		[HttpPost]
		public async Task<ActionResult> UploadFile(HttpPostedFileBase file)
		{
			try
			{
				string output = string.Empty;
				if (file.ContentLength > 0)
				{

					MultipartFormDataContent requestContent = new MultipartFormDataContent();
					
					//Reading file content
					MemoryStream target = new MemoryStream();
					file.InputStream.CopyTo(target);
					byte[] data = target.ToArray();
					var strTrue = "true";
					string status = string.Empty;
					ByteArrayContent bytesContent = new ByteArrayContent(data);

					StreamContent fileContent = new StreamContent(file.InputStream);
					fileContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);
					
					//Adding file headers
					requestContent.Headers.Add("apikey", Resources.API_KEY);
					requestContent.Add(fileContent, "fileUploaded", file.FileName);
					requestContent.Add(bytesContent, "fileUploaded", file.FileName);
					requestContent.Add(new StringContent(strTrue), "async");

					//Post request
					HttpResponseMessage response = await restClient.PostAsync(Resources.URI, requestContent);
					string result = response.Content.ReadAsStringAsync().Result;
					var resultPost = JsonConvert.DeserializeObject<RootObject>(result);
					
					//Get request 
					HttpResponseMessage httpResponseMessage = null;
					var scanId = (resultPost.Data != null) ? resultPost.Data.ScanId : string.Empty;
					if (resultPost.Status.ToLower() == "failure")
					{
						ViewBag.Message = "File upload failed!";
						return View("Index");
					}
					for (int i = 0; i < 5; i++)
					{
						
						HttpClient requestClient = new HttpClient();

						var requestMessage = new HttpRequestMessage(HttpMethod.Get, Resources.URI + scanId);
						requestMessage.Headers.Add("apikey", Resources.API_KEY);
						var getResult = requestClient.SendAsync(requestMessage).Result;
						var serializedResponse = getResult.Content.ReadAsStringAsync().Result;
						var deserializedProduct = JsonConvert.DeserializeObject<GetResponse>(serializedResponse);
						status = deserializedProduct.status;
						if (deserializedProduct.status.ToLower() == "success")
						{
							httpResponseMessage = getResult;
							break;
						}
						requestClient.Dispose();
						Thread.Sleep(10000);

					}
					

					if (httpResponseMessage != null) output = httpResponseMessage.Content.ReadAsStringAsync().Result;
					else
					{
						ViewBag.Message = "File upload failed! Status:Uploaded/OCRED";
						return View("Index");
					}

					string path = Server.MapPath("~/App_Data/");

					// Write that JSON to txt file,  
					System.IO.File.WriteAllText(path + "Response.json", output);

				}
				ViewData.Add("Result", output);
				return View("Index");
			}
			catch(Exception ex)
			{
				ViewBag.Message = "File upload failed!";
				return View("Index");
			}

		}

		public ActionResult About()
		{
			ViewBag.Message = "Your application description page.";

			return View();
		}

		public ActionResult Contact()
		{
			ViewBag.Message = "Your contact page.";

			return View();
		}
	}
}