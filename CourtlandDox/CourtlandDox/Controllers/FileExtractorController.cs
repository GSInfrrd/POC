using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Results;
using CourtlandDox.DataAccess;
using CourtlandDox.Models;
using CourtlandDox.Properties;

namespace CourtlandDox.Controllers
{
    public class FileExtractorController : ApiController
    {
        private static readonly HttpClient restClient = new HttpClient();

        private readonly InvoiceContext _invoiceContext;


        public FileExtractorController()
        {
            _invoiceContext = new InvoiceContext();
        }
        // GET: api/FileExtractor
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/FileExtractor/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/FileExtractor
        public async Task<JsonResult<InvoiceResponse>> Post()
        {
            string output = string.Empty;
            var httpRequest = HttpContext.Current.Request;
            var files = httpRequest.Files;
            var file = files[0];
            var resultPost = new RootObject();
            var resList = new List<Field>();
            if (file.ContentLength > 0)
            {
                resultPost = await GetOcrText(file);
            }
            if (resultPost != null)
            {
                resList = resultPost.data.fields;
                var returnList = _invoiceContext.GetTagDetailIdentifier(resultPost.data.text);
                resList.AddRange(returnList);
            }
            var res = new InvoiceResponse { fields = resList };
            return Json(res);

        }

        private static async Task<RootObject> GetOcrText(HttpPostedFile file)
        {
            RootObject resultPost;
            MultipartFormDataContent requestContent = new MultipartFormDataContent();

            //Reading file content
            MemoryStream target = new MemoryStream();
            file.InputStream.CopyTo(target);
            byte[] data = target.ToArray();
            var xmltext = "true";
            string status = string.Empty;
            ByteArrayContent bytesContent = new ByteArrayContent(data);

            StreamContent fileContent = new StreamContent(file.InputStream);
            fileContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);

            //Adding file headers
            requestContent.Headers.Add("apikey", Resources.API_KEY);
            requestContent.Headers.Add("xmltext", Resources.XML_TEXT);
            requestContent.Add(fileContent, "fileUploaded", file.FileName);
            requestContent.Add(bytesContent, "fileUploaded", file.FileName);
            requestContent.Add(new StringContent(xmltext), "xmltext");

            //Post request
            HttpResponseMessage response = await restClient.PostAsync(Resources.URI, requestContent);
            string result = response.Content.ReadAsStringAsync().Result;
            resultPost = JsonConvert.DeserializeObject<RootObject>(result);
            return resultPost;
        }

        // PUT: api/FileExtractor/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/FileExtractor/5
        public void Delete(int id)
        {
        }
    }
}
