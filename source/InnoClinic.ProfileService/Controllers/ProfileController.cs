using Microsoft.AspNetCore.Mvc;

namespace InnoClinic.ProfileService.Controllers
{
    public class ProfileController : Controller
    {
        public ProfileController()
        {
            
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
