using System.Web.Mvc;
using CourtlandDox.DataAccess;
using CourtlandDox.Models;

namespace CourtlandDox.Controllers
{
    public class BankController : Controller
    {
        private readonly BankContext _bankContext;

        public BankController()
        {
                _bankContext=new BankContext();
        }
        // GET: Bank
        public ActionResult Index()
        {
            var res = _bankContext.GetAll();
            return View(res);
        }
        // GET: Bank/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Bank/Create
        [HttpPost]
        public ActionResult Create(BankCollection collection)
        {
            try
            {
                collection.DocumentType = "Bank";
                _bankContext.Create(collection);
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
        
    }
}
