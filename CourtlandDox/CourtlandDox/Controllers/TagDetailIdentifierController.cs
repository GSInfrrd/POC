using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web.Mvc;
using CourtlandDox.DataAccess;
using CourtlandDox.Models;

namespace CourtlandDox.Controllers
{
    public class TagDetailIdentifierController : Controller
    {
        private readonly TagDetailIdentifierContext _tagDetailIdentifierContext;
        private readonly NoticeContext _noticeContext;
        public TagDetailIdentifierController()
        {
            _tagDetailIdentifierContext = new TagDetailIdentifierContext();
            _noticeContext=new NoticeContext();
        }

        // GET: TagDetailIdentifier
        public ActionResult Index()
        {
            var res = _tagDetailIdentifierContext.GetAll();
            return View(res);
        }


        // GET: TagDetailIdentifier/Create
        public ActionResult Create()
        {
            List<SelectListItem> list = new List<SelectListItem>();
            var notices = _noticeContext.GetAll();
            foreach (var item in notices)
            {
                string notId = Convert.ToString(item.Id, CultureInfo.InvariantCulture);
                list.Add(new SelectListItem { Text = item.Name, Value = notId });
            }

            ViewBag.ListOfNotices = list;
            return View();
        }

        // POST: TagDetailIdentifier/Create
        [HttpPost]
        public ActionResult Create(TagDetailIdentifierCollection collection)
        {
            try
            {
                // TODO: Add insert logic here
                if (ModelState.IsValid)
                {
                    collection.DocumentType = "TagDetailIdentifier";
                    _tagDetailIdentifierContext.Create(collection);
                }
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: TagDetailIdentifier/Edit/5
       
    }
}
