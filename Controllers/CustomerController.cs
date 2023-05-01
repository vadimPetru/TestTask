using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ProductStoreTask.Controllers
{
    public class CustomerController : Controller
    {
        [Authorize(Roles ="Customer")]
        public IActionResult Secret()
        {
            return View();
        }
    }
}
