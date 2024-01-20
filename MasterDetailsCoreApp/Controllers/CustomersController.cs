using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MasterDetailsCoreApp.Models;
using Microsoft.Extensions.Hosting;

namespace MasterDetailsCoreApp.Controllers
{
    public class CustomersController : Controller
    {
        private readonly MyContext _context;
        private readonly IWebHostEnvironment _enc;

        public CustomersController(MyContext context, IWebHostEnvironment enc)
        {
            _context = context;
            _enc = enc;
        }

        // GET: Customers
        public async Task<IActionResult> Index()
        {
            var data = await _context.Customers.Include(i => i.InvoiceItems).ThenInclude(p => p.Items).ToListAsync();


            ViewBag.Count = data.Count;
            ViewBag.GrandTotal = data.Sum(i => i.InvoiceItems.Sum(l => l.ItemTotal));

            ViewBag.Average = data.Count > 0 ? data.Average(i => i.InvoiceItems.Sum(l => l.ItemTotal)) : 0;

            return View(data);
        }

        // GET: Customers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customer = await _context.Customers.Include(i => i.InvoiceItems).ThenInclude(p => p.Items)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (customer == null)
            {
                return NotFound();
            }

            return View(customer);
        }

        // GET: Customers/Create
        public IActionResult Create()
        {
            return View(new Customer());
        }

        // POST: Customers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Customer customer, string command = "")
        {
            if (customer.ImageUpload != null)
            {
                var webPath = _enc.WebRootPath;

                string fileName = Path.GetFileNameWithoutExtension(customer.ImageUpload.FileName);
                string fileExt = Path.GetExtension(customer.ImageUpload.FileName);

                string uploadName = $"\\Images\\{fileName}_{DateTime.Now:yyyymmddHHMMss}{fileExt}";
               

                string path = webPath + uploadName;

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await customer.ImageUpload.CopyToAsync(stream);
                }
                customer.Image = uploadName;

                TempData["ImageUrl"] = customer.Image;
            }
            else
            {
                customer.Image = TempData["ImageUrl"]?.ToString();
            }

            if (command == "Add")
            {
                customer.InvoiceItems.Add(new());
                return View(customer);
            }
            else if (command.Contains("delete"))
            {
                int idx = int.Parse(command.Split('-')[1]);

                customer.InvoiceItems.RemoveAt(idx);
                ModelState.Clear();
                return View(customer);
            }

            if (ModelState.IsValid)
            {
                //_context.Add(customer);
                //await _context.SaveChangesAsync();

                var rows = await _context.Database.ExecuteSqlRawAsync("exec spInsertCustomer @p0, @p1, @p2, @p3, @p4", customer.Name, customer.Address, customer.ContactNo, customer.IsPermanent, customer.Image);


                if (rows > 0)
                {
                    customer.Id = _context.Customers.Max(x => x.Id);

                    foreach (var item in customer.InvoiceItems)
                    {
                        await _context.Database.ExecuteSqlRawAsync("exec spInsertInvoice @p0, @p1, @p2, @p3", item.InvoiceDate, item.Quantity, item.ItemsId, customer.Id);
                    }
                }

                return RedirectToAction(nameof(Index));
            }
            return View(customer);
        }

        // GET: Customers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customer = await _context.Customers.Include(i => i.InvoiceItems).ThenInclude(p => p.Items).FirstOrDefaultAsync(i => i.Id == id);
            if (customer == null)
            {
                return NotFound();
            }
            return View(customer);
        }

        // POST: Customers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Customer customer, string command = "")
        {
            if (customer.ImageUpload != null)
            {
                var webPath = _enc.WebRootPath;

                string fileName = Path.GetFileNameWithoutExtension(customer.ImageUpload.FileName);
                string fileExt = Path.GetExtension(customer.ImageUpload.FileName);

                string uploadName = $"\\Images\\{fileName}_{DateTime.Now:yyyymmddHHMMss}{fileExt}";


                string path = webPath + uploadName;

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await customer.ImageUpload.CopyToAsync(stream);
                }
                customer.Image = uploadName;

                TempData["ImageUrl"] = customer.Image;
            }
            else
            {
                customer.Image = TempData["ImageUrl"]?.ToString();
            }

            if (command == "Add")
            {
                customer.InvoiceItems.Add(new());
                return View(customer);
            }
            else if (command.Contains("delete"))
            {
                int idx = int.Parse(command.Split('-')[1]);

                customer.InvoiceItems.RemoveAt(idx);
                ModelState.Clear();
                return View(customer);
            }
            if (id != customer.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    //_context.Update(customer);
                    //await _context.SaveChangesAsync();

                    var row = await _context.Database.ExecuteSqlRawAsync("exec spUpdateCustomer @p0, @p1, @p2, @p3, @p4, @p5", customer.Id, customer.Name, customer.Address, customer.ContactNo, customer.IsPermanent, customer.Image);


                    foreach (var item in customer.InvoiceItems)
                    {
                        await _context.Database.ExecuteSqlRawAsync("exec spInsertInvoice @p0, @p1, @p2, @p3", item.InvoiceDate, item.Quantity, item.ItemsId, customer.Id);
                    }

                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CustomerExists(customer.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(customer);
        }

        // GET: Customers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customer = await _context.Customers.Include(i => i.InvoiceItems).ThenInclude(p => p.Items)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (customer == null)
            {
                return NotFound();
            }

            return View(customer);
        }

        // POST: Customers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
			//var customer = await _context.Customers.FindAsync(id);
			//if (customer != null)
			//{
			//    _context.Customers.Remove(customer);
			//}

			await _context.Database.ExecuteSqlAsync($"exec spDeleteCustomer {id}");

			await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        [HttpDelete]
        [Route("~/deleteinvoice/{id}")]
        public async Task<IActionResult> DeleteAjax(int id)
        {
            
            await _context.Database.ExecuteSqlAsync($"exec spDeleteCustomer {id}");

            return Ok();
        }
        private bool CustomerExists(int id)
        {
            return _context.Customers.Any(e => e.Id == id);
        }
    }
}
