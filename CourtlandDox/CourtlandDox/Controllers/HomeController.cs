using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using CourtlandDox.DataAccess;
using CourtlandDox.Models;
using CourtlandDox.Properties;
using CourtlandDox.Util;
using Newtonsoft.Json;
using Spire.Pdf;
using System.Drawing;
using System;
using Tesseract;

namespace CourtlandDox.Controllers
{
    public class HomeController : Controller
    {
        private readonly NoticeContext _noticeContext;
        private readonly IdentifierContext _identifierContext;
        private readonly BankContext _bankContext;
        private readonly TagDetailIdentifierContext _tagDetailIdentifierContext;
        private static readonly HttpClient RestClient = new HttpClient();
        public HomeController()
        {

            _identifierContext = new IdentifierContext();
            _noticeContext = new NoticeContext();
            _bankContext = new BankContext();
            _tagDetailIdentifierContext = new TagDetailIdentifierContext();
        }
        public ActionResult Index()
        {
            return View(new TagDetail());
        }



        [HttpPost]
        public ActionResult Index(string s)
        {
            var files = Request.Files;
            var res = new TagDetail();
            try
            {
                if (files?.Count > 0)
                {
                    for (int i = 0; i < files.Count; i++)
                    {
                        var file = files[i];
                        if (file != null)
                        {
                            var fileName = file.FileName;
                            string filePath = Path.GetFileName(fileName);
                            if (filePath != null)
                            {
                                string fullPath = Path.Combine(Server.MapPath("~/File"), filePath);
                                file.SaveAs(fullPath);
                                var extractedText = Helper.ExtractTextFromPdf(fullPath);
                                if (string.IsNullOrEmpty(extractedText))
                                {
                                    extractedText = ExtractTesseract(fullPath);
                                }
                                var forBank = _bankContext.GetBankDetail(extractedText);
                                var allIdentifiers = _identifierContext.GetAll();
                                var (noticeTypesName, noticeTypesValue) =
                                    _noticeContext.GetTypes(extractedText?.ToLower(), allIdentifiers);
                                res.NoticeType = string.Join(",", noticeTypesName);
                                res.FileName = fileName;
                                res.Bank = forBank.Name;
                                res.Tags = new List<Tag>();
                                foreach (var notice in noticeTypesValue)
                                {
                                    var resTags =
                                        _tagDetailIdentifierContext.GetTagDetailIdentifier(notice,
                                            extractedText?.ToLower());
                                    res.Tags.AddRange(resTags);
                                }
                            }
                        }
                    }
                }
                TempData["Tags"] = res.Tags;
                res.Executed = true;
            }
            catch (Exception e)
            {
                throw e;
            }
            int milliseconds = 2000;
            Thread.Sleep(milliseconds);
            //return RedirectToAction("Extracted", res);
            return View(res);
        }

        private string ExtractTesseract(string fullPath)
        {
            string extractedText = string.Empty;

            try
            {
                PdfDocument doc = new PdfDocument();
                doc.LoadFromFile(fullPath);
                List<Image> images = new List<Image>();
                foreach (PdfPageBase page in doc.Pages)
                {
                    foreach (Image image in page.ExtractImages())
                    {
                        images.Add(image);
                    }
                }
                doc.Close();
                int index = 0;
                string path;
                List<string> paths = new List<string>();
                foreach (Image image in images)
                {
                    String imageFileName
                        = String.Format("Image-{0}.jpeg", index++);
                    path = Path.Combine(Server.MapPath("~/File"), imageFileName);
                    image.Save(path, System.Drawing.Imaging.ImageFormat.Jpeg);
                    paths.Add(path);
                }
                //save text
                var dataPath = Server.MapPath("~/bin/tessdata");
                foreach (var item in paths)
                {
                    var imagePath = item;
                    using (var tEngine = new TesseractEngine(dataPath, "eng", EngineMode.Default)
                    ) //creating the tesseract OCR engine with English as the language
                    {
                        using (var img = Pix.LoadFromFile(imagePath)
                        ) // Load of the image file from the Pix object which is a wrapper for Leptonica PIX structure
                        {
                            using (var page = tEngine.Process(img)) //process the specified image
                            {
                                var text = page.GetText();
                                extractedText += text;
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            return extractedText;
        }

        private static async Task<RootObject> GetOcrText(HttpPostedFileBase file)
        {
            RootObject resultPost;
            MultipartFormDataContent requestContent = new MultipartFormDataContent();

            //Reading file content
            MemoryStream target = new MemoryStream();
            file.InputStream.CopyTo(target);
            byte[] data = target.ToArray();
            var xmltext = "true";
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
            HttpResponseMessage response = await RestClient.PostAsync(Resources.URI, requestContent).ConfigureAwait(false);
            string result = response.Content.ReadAsStringAsync().Result;
            resultPost = JsonConvert.DeserializeObject<RootObject>(result);
            return resultPost;
        }

        public ActionResult Extracted(TagDetail tagDetail)
        {
            if (TempData["Tags"] != null)
                tagDetail.Tags = (List<Tag>)TempData["Tags"];
            return View(tagDetail);
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