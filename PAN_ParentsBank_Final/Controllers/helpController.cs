using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PAN_ParentsBank_Final.Controllers
{
    public class helpController : Controller
    {
        // GET: help
        [ActionName("financial-resources")]
        public ActionResult Index()
        {
            return View();
        }
    }
}