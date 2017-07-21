using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using CourtlandDox.DataAccess;
using CourtlandDox.Models;
using CourtlandDox.Util;

namespace CourtlandDox.Controllers
{
    public class ExtractController : ApiController
    {
        private readonly NoticeContext _noticeContext;
        private readonly IdentifierContext _identifierContext;
        private readonly BankContext _bankContext;
        private readonly TagDetailIdentifierContext _tagDetailIdentifierContext;

        public ExtractController()
        {
            _identifierContext = new IdentifierContext();
            _noticeContext = new NoticeContext();
            _bankContext = new BankContext();
            _tagDetailIdentifierContext = new TagDetailIdentifierContext();
        }
        // GET: api/Extract
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/Extract/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/Extract
        public ActionResult Post()
        {
            var httpRequest = HttpContext.Current.Request;
            var files = httpRequest.Files;
            var res = new TagDetail();
            if (files?.Count > 0)
            {
                for (int i = 0; i < files.Count; i++)
                {
                    var file = files[i];
                    {
                        var fileName = file.FileName;
                        string filePath = System.IO.Path.GetFileName(fileName);
                        {
                            string fullPath = HttpContext.Current.Server.MapPath("~/File/" + filePath);
                            file.SaveAs(fullPath);
                            var extractedText = Helper.ExtractTextFromPdf(fullPath);
                            var forBank = _bankContext.GetBankDetail(extractedText);
                            var allIdentifiers = _identifierContext.GetAll();
                            var (noticeTypesName, noticeTypesValue) = _noticeContext.GetTypes(extractedText.ToLower(), allIdentifiers);
                            res.NoticeType = string.Join(",", noticeTypesName);
                            res.FileName = fileName;
                            res.Bank = forBank.Name;
                            res.Tags = new List<Tag>();
                            foreach (var notice in noticeTypesValue)
                            {
                                var resTags = _tagDetailIdentifierContext.GetTagDetailIdentifier(notice, extractedText.ToLower());
                                res.Tags.AddRange(resTags);
                            }
                        }
                    }
                }
            }
            return new JsonResult() { Data = res, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        // PUT: api/Extract/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/Extract/5
        public void Delete(int id)
        {
        }
    }
}
