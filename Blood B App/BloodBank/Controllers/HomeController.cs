using BloodBank.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace BloodBank.Controllers
{
    public class HomeController : Controller
    {
        BloodBankDbContext db = new BloodBankDbContext();
        public ActionResult Index()
        {
            ViewBag.bloodGroup = new SelectList(db.BloodGroup.ToList(), "Id", "Description");
            ViewBag.City = new SelectList(db.City.ToList(),"Id","Description");

            BloodRequestViewModel b = new BloodRequestViewModel();
            b.ListBloodRequest = db.BloodRequest.ToList();
            b.ListUsers = db.users.ToList();
            return View(b);
        }
        public ActionResult SearchPg()
        {
            ViewBag.bloodGroup = new SelectList(db.BloodGroup.ToList(), "Id", "Description");
            ViewBag.City = new SelectList(db.City.ToList(), "Id", "Description");

            BloodRequestViewModel b = new BloodRequestViewModel();
            b.ListBloodRequest = db.BloodRequest.ToList();
            b.ListUsers = db.users.ToList();
            return View(b);
        }

        public ActionResult Search(string bloodGroup, string city)
        {
            int bloodGroupId = Convert.ToInt32(bloodGroup);
            int cityId = Convert.ToInt32(city);

            var result = db.users.Where(x => x.BloodGroupId == bloodGroupId && x.CityId == cityId).ToList();

            return View(result);
        }

        //public ActionResult SearchResults(List<string> results)
        //{
        //    List<Users> searchResults = new List<Users>();
        //    foreach (var item in results)
        //    {
        //        Users u = new Users();
        //        u.FirstName = item[0];

        //    }

        //    return View();
        //}

        public ActionResult MyProfile()
        {
            HttpCookie id = Request.Cookies.Get("UserID");
            var userId = Convert.ToInt32(id.Value);
            var userType = db.users.FirstOrDefault(x => x.Id == userId).UserType;

            var result = db.users.Where(x => x.Id == userId).FirstOrDefault();
            return View(result);
        }

        public ActionResult AdminPanel()
        {
            BloodRequestViewModel b = new BloodRequestViewModel();

            b.ListBloodRequest = db.BloodRequest.ToList();
            b.ListUsers = db.users.Where(x => x.UserType != "Admin").ToList();
            return View(b);
        }

        public static string MD5Generator(string password)
        {
            var md5 = MD5.Create();
            var result = md5.ComputeHash(Encoding.ASCII.GetBytes(password));
            var hash = md5.ComputeHash(result);

            var sb = new StringBuilder();
            for (var i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("X2"));
            }
            return sb.ToString();
        }

        public ActionResult UpdateProfile(string firstName, string lastName, string email, string password, string age, string gender, string address, string number)
        {
            HttpCookie id = Request.Cookies.Get("UserID");
            var userId = Convert.ToInt32(id.Value);

            Users u = db.users.Find(userId);

            u.FirstName = firstName;
            u.LastName = lastName;
            u.Email = email;
            u.Password = MD5Generator(password);
            u.Age = Convert.ToInt32(age);
            u.Number = number;
            u.Gender = gender;
            u.Address = address;

            db.Entry(u).State = EntityState.Modified;

            var result = db.SaveChanges();

            if (result == 1)
                return Json("1");
            return Json("0");

        }


            public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}