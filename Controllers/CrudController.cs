using Microsoft.AspNetCore.Mvc;
using ProductStoreTestTask.Service.Interface;
using ProductStoreTestTask.DataLayer.Models;
using ProductStoreTask.Properties;
using Microsoft.AspNetCore.Authorization;

namespace ProductStoreTestTask.Controllers
{
    public class CrudController : Controller
    {
        private readonly IProductService _product;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public CrudController(IProductService product, IWebHostEnvironment webHostEnvironment)
        {
            _product = product;
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpGet]
        public IActionResult CreateProduct()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateCurrentProduct(Product product)
        {
            if (ModelState.IsValid)
            {
                var files = HttpContext.Request.Form.Files;
                string webRootPath = _webHostEnvironment.WebRootPath;
                if(product.ProductId == 0)
                {
                    string upload = webRootPath + Resources.ImagePath;
                    string fileName = Guid.NewGuid().ToString();
                    string extension = Path.GetExtension(files[0].FileName);


                    using (FileStream fileStream = new(Path.Combine(upload, fileName + extension), FileMode.Create))
                    {
                        files[0].CopyTo(fileStream);
                    }

                    product.Image = fileName + extension;
                     await _product.CreateProductAsync(product);
                    return RedirectToAction("GetListProducts");
                    //create
                }
                else
                {
                    var itemObj = await _product.GetProductByIdNoTracking(product.ProductId);
                   
                    if (files.Count > 0)
                    {
                        string upload = webRootPath + Resources.ImagePath;
                        string fileName = Guid.NewGuid().ToString();
                        string extension = Path.GetExtension(files[0].FileName);

                        var oldFile = Path.Combine(upload, itemObj.Image);

                        if (System.IO.File.Exists(oldFile))
                        {
                            System.IO.File.Delete(oldFile);
                        }

                        using (FileStream fileStream = new(Path.Combine(upload, fileName + extension), FileMode.Create))
                        {
                            files[0].CopyTo(fileStream);
                        }

                        product.Image = fileName + extension;
                    }
                    else
                    {
                        product.Image = itemObj.Image;
                    }

                    await _product.UpdateProduct(product);
                }
               
            }
            return View("CreateProduct", product);
        }

      
        public IActionResult DeleteProduct(int id)
        {
            _product.DeleteProductAsync(id);
            return RedirectToAction("GetListProducts");
        }

        
        public  async Task<IActionResult> UpdateProduct(int id)
        {
            if(id == 0)
            {
                NotFound();
            }
            var  product = await _product.GetProductByIdAsync(id);
            if(product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        [HttpPost]
        public IActionResult UpdateProduct(Product product)
        {
            if (ModelState.IsValid)
            {
                _product.UpdateProduct(product);
                return RedirectToAction("GetListProducts");
            }
            return View("UpdateProduct", product);
        }

        [HttpGet]
        public IActionResult GetProductById(int id)
        {
            return View(_product.GetProductByIdAsync(id));
        }

        [HttpGet]
        [Authorize(Roles ="Admin")]
        public  IActionResult GetListProducts()
        {
            var listProduct =_product.GetProductsListAsync();
            return View(listProduct);
        }
    }
}
