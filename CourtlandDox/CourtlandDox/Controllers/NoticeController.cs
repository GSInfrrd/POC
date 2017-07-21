using System.Web.Mvc;
using CourtlandDox.DataAccess;
using CourtlandDox.Models;

namespace CourtlandDox.Controllers
{
    public class NoticeController : Controller
    {
        private readonly NoticeContext _noticeContext;
        public NoticeController()
        {
            _noticeContext = new NoticeContext();
        }
        // GET: Notice
        public ActionResult Index()
        {
            var res = _noticeContext.GetAll();
            return View(res);
        }

        // GET: Notice/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Notice/Create
        [HttpPost]
        public ActionResult Create(NoticeCollection notice)
        {
            try
            {
                notice.DocumentType = "NoticeType";
                _noticeContext.Create(notice);
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

    }
}
