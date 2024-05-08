using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SV20T1020607.Wed.Models;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SV20T1020607.Wed.Controllers
{
    public class TestController : Controller
    {
        // GET: /<controller>/
      

        public IActionResult Create()
        {
            var model = new Person()
            {
                Name = "Nguyễn Khắc Trung",
                Birthday = new DateTime(1990, 10, 28),
                Salary = 500.25m

            };
            return View(model);
        }
        public IActionResult Save(Models.Person model,string BirthdayInput = "")
        {
            //Chuyển chuỗi Birthday in thành gía trị ngày , nếu hợp  thì dùng giá trị mới do người dùng nhập
            DateTime? d = null;
            try
            {
                d = DateTime.ParseExact(BirthdayInput, "d/M/yyyy", CultureInfo.InvariantCulture);
            }
            catch
            {

            }
            if (d.HasValue)
                model.Birthday = d.Value;
            return Json(model);
        }
    }
}

