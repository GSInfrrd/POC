using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web.Mvc;
using CourtlandDox.DataAccess;
using CourtlandDox.Models;

namespace CourtlandDox.Controllers
{
    public class IdentifierController : Controller
    {
        private readonly NoticeContext _noticeContext;
        private readonly IdentifierContext _identifierContext;
        public IdentifierController()
        {
            _noticeContext = new NoticeContext();
            _identifierContext=new IdentifierContext();
        }
        // GET: Identifier
        public ActionResult Index()
        {
            var res=_identifierContext.GetAll();
            return View(res);
        }
        
        // GET: Identifier/Create
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

        // POST: Identifier/Create
        [HttpPost]
        public ActionResult Create(IdentifierCollection collection)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    collection.DocumentType = "IdentifierType";
                    _identifierContext.Create(collection);
                }
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
        
    }
}
