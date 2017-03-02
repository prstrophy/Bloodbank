using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BloodBank.Models;

namespace BloodBank.Controllers
{
    public class BloodRequestsController : Controller
    {
        private BloodBankDbContext db = new BloodBankDbContext();

        // GET: BloodRequests
        public ActionResult Index()
        {
            List<SelectListItem> bloodGroup = new List<SelectListItem>();
      
            bloodGroup.Add(new SelectListItem
            {
                Text = "A+",
                Value = "A+",

            });
            bloodGroup.Add(new SelectListItem
            {
                Text = "A-",
                Value = "A-",
                Selected = true
            });
            bloodGroup.Add(new SelectListItem
            {
                Text = "B+",
                Value = "B+",

            });
            bloodGroup.Add(new SelectListItem
            {
                Text = "B-",
                Value = "B-",
                Selected = true
            });
            bloodGroup.Add(new SelectListItem
            {
                Text = "O+",
                Value = "O+",

            });
            bloodGroup.Add(new SelectListItem
            {
                Text = "O-",
                Value = "O-",

            });
            bloodGroup.Add(new SelectListItem
            {
                Text = "AB+",
                Value = "AB+",

            });
            bloodGroup.Add(new SelectListItem
            {
                Text = "AB-",
                Value = "AB-",
                Selected = true
            });


            ViewBag.bloodGroup = bloodGroup;
            BloodRequestViewModel b = new BloodRequestViewModel();
            b.ListBloodRequest = db.BloodRequest.ToList();
            b.ListUsers = db.users.ToList();
            return View(b);
        }

        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BloodRequest req = db.BloodRequest.Find(id);
            if (req == null)
            {
                return HttpNotFound();
            }
            return View(req);
        }




        public ActionResult SaveBloodRequest(string patientName, string bloodGroup, string location, string hospitalName,
                                             string hospitalAddress, string contactName, string emailId,
                                             string contactNumber, string requiredDate)
        {
            BloodRequest bloodRequest = new BloodRequest();
            bloodRequest.PatientName = patientName;
            bloodRequest.BloodGroup = bloodGroup;
            bloodRequest.Location = location;
            bloodRequest.Hospital_Name = hospitalName;
            bloodRequest.Hospital_Address = hospitalAddress;
            bloodRequest.ContactName = contactName;
            bloodRequest.ContactNumber = contactNumber;
            bloodRequest.EmailId = emailId;
            bloodRequest.ContactNumber = contactNumber;
            bloodRequest.RequiredDate = Convert.ToDateTime(requiredDate);

            db.BloodRequest.Add(bloodRequest);
            var result = db.SaveChanges();

            if (result == 1)
            {
                return Json("1");
            }
            else
            {
                return Json(null);
            }
        }


        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BloodRequest bloodRequest = db.BloodRequest.Find(id);
            if (bloodRequest == null)
            {
                return HttpNotFound();
            }
            return View(bloodRequest);
        }

        // POST: BloodRequests1/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            BloodRequest bloodRequest = db.BloodRequest.Find(id);
            db.BloodRequest.Remove(bloodRequest);
            db.SaveChanges();
            return RedirectToAction("AdminPanel", "Home");
        }
        public ActionResult BloodRequests()
        {
            return View(db.BloodRequest.ToList());
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
