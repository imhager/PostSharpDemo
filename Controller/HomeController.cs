using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using PostSharpDemo.Services;

namespace PostSharpDemo.Pages
{
    public class HomeController : Controller
    {

        private readonly IDemoUserService demoUserService;

        public HomeController(IDemoUserService demoUserService)
        {
            this.demoUserService = demoUserService;
        }

        // GET: HomeController
        public ActionResult Index()
        {
            return View();
        }

        // GET: HomeController/Details/5
        public ActionResult Details(int id)
        {
            var user = demoUserService.GetUser(id);

            return Json(user);
        }
    }
}
