using Microsoft.AspNetCore.Mvc;
using ObjectBinderDemo.Common;
using ObjectBinderDemo.Models;
using System.Diagnostics;

namespace ObjectBinderDemo.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpGet]
        public IActionResult Setting(int id)
        { 
            var model = DataSource.DataList.FirstOrDefault(c => c.Id == id);
            return PartialView(model);
        }

        [HttpPost]
        public IActionResult Setting([ModelBinder(typeof(HiddenTypeModelBinder))]ParentModel model)
        {
            if(ModelState.IsValid)
            {
                var setting = default(ParentModel);
                if (model is ChildrenModel1)
                {
                    setting = model as ChildrenModel1;
                }
                else if (model is ChildrenModel2)
                {
                    setting = model as ChildrenModel2;
                }
            }

            return Json(new { Result = true });
        }
    }
}