using Microsoft.AspNetCore.Mvc;
using ProductStoreTask.Data;
using ProductStoreTestTask.DataLayer;
using ProductStoreTestTask.DataLayer.Models;
using ProductStoreTestTask.Service.Interface;

namespace ProductStoreTestTask.Controllers
{
    public class HomeController : Controller
    {
        private readonly IProductService _product;
        private readonly ApplicationDbContext context;

        public HomeController(IProductService product)
        {
            _product = product;
        }

        public async Task<IActionResult> Index()
        {
          
            var products = await _product.GetProductsListAsync();
            return View(products);
        }
    }
}
