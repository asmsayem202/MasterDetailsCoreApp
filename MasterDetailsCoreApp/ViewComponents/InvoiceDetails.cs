using MasterDetailsCoreApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace MasterDetailsCoreApp.ViewComponents
{
	public class InvoiceDetails : ViewComponent
	{
        public IViewComponentResult Invoke(List<InvoiceItem> data)
        {

            ViewBag.Count = data.Count;
            ViewBag.Total = data.Sum(i => i.ItemTotal);

            return View(data);
        }
    }
}
