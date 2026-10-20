using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using WebApplication1.Data;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ProductRepository _productRepo;
        private readonly ProductTypeRepository _typeRepo;


        public ProductsController(ProductRepository productRepo,
                                  ProductTypeRepository typeRepo)
        {
            _productRepo = productRepo;
            _typeRepo = typeRepo;
        }


        private void LoadProductTypes(int? selectedId = null)
        {
            List<ProductType> types = _typeRepo.GetAll();
            ViewBag.TypeList = new SelectList(types, "Id", "Name", selectedId);
        }


        private bool IsValidProduct(Product product)
        {
            if (string.IsNullOrWhiteSpace(product.Name))
                ModelState.AddModelError("Name", "กรุณากรอกชื่อสินค้า");
            else if (product.Name.Length > 50)
                ModelState.AddModelError("Name", "ชื่อสินค้ายาวไม่เกิน 50 ตัวอักษร");

            if (product.Price < 0)
                ModelState.AddModelError("Price", "ราคาต้องไม่ติดลบ");

            if (product.TypeId <= 0)
                ModelState.AddModelError("TypeId", "กรุณาเลือกประเภทสินค้า");

            return ModelState.ErrorCount == 0;
        }
        public IActionResult Index(string? keyword)
        {
            try
            {
                List<Product> products = _productRepo.Search(keyword);
                ViewBag.Keyword = keyword;
                return View(products);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "โหลดข้อมูลสินค้าไม่สำเร็จ: " + ex.Message;
                return View(new List<Product>());
            }
        }

        public IActionResult Create()
        {
            LoadProductTypes();
            return View(new Product());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Product product)
        {

            if (!IsValidProduct(product))
            {
                LoadProductTypes(product.TypeId);
                return View(product);
            }

            try
            {
                int newId = _productRepo.Insert(product);
                TempData["Message"] = $"เพิ่มข้อมูลสำเร็จ (รหัสสินค้าใหม่คือ {newId})";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "เพิ่มข้อมูลไม่สำเร็จ: " + ex.Message);
                LoadProductTypes(product.TypeId);
                return View(product);
            }
        }

        public IActionResult Edit(int id)
        {
            Product? product = _productRepo.SearchById(id);

            if (product == null)
            {
                TempData["Error"] = "ไม่พบสินค้ารหัสนี้";
                return RedirectToAction("Index");
            }

            LoadProductTypes(product.TypeId);
            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Product product)
        {
            if (!IsValidProduct(product))
            {
                LoadProductTypes(product.TypeId);
                return View(product);
            }

            try
            {
                int rows = _productRepo.Update(product);
                TempData["Message"] = $"แก้ไขข้อมูลสำเร็จ (จำนวนแถวที่กระทบคือ {rows})";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "แก้ไขข้อมูลไม่สำเร็จ: " + ex.Message);
                LoadProductTypes(product.TypeId);
                return View(product);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            try
            {
                _productRepo.Delete(id);
                TempData["Message"] = "ลบข้อมูลสำเร็จ";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "ลบข้อมูลไม่สำเร็จ: " + ex.Message;
            }
            return RedirectToAction("Index");
        }
    }
}
