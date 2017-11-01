using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using PAN_ParentsBank_Final.Models;

namespace PAN_ParentsBank_Final.Controllers
{
    [Authorize]
    public class AccountsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Accounts
        public ActionResult Index()
        {
            string currentlyLoggedInUsername = User.Identity.Name;

            var accountsOwner = db.Accounts
                .Where(x => x.Owner == currentlyLoggedInUsername).ToList();
            var accountRecipient = db.Accounts
              .Where(x => x.Recipient == currentlyLoggedInUsername).ToList();
            if (accountRecipient != null && accountRecipient.Count > 0)
            {
                Account recipientAccount = accountRecipient.First();
                return RedirectToAction("Details", new { Id = recipientAccount.Id });
            }
            if (accountsOwner == null || !(accountsOwner.Count > 0))
            {
                return RedirectToAction("Create");
            }
            else
            {
                return View(accountsOwner);
            }
        }

        // GET: Accounts/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Account account = db.Accounts.Find(id);
            if (account == null)
            {
                return HttpNotFound();
            }
            return View(account);
        }

        // GET: Accounts/Create
        public ActionResult Create()
        {
            if (checkListForValue(getListOfRecipients(), User.Identity.Name)){
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            }
            return View();
        }

        // POST: Accounts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Owner,Recipient,Name,InterestRate")] Account account)
        {
            if (checkListForValue(getListOfRecipients(), account.Recipient))
            {
                ModelState.AddModelError("Recipient", "Recipient already has an account");
            }

            if (checkListForValue(getListOfOwners(), account.Recipient))
            {
                ModelState.AddModelError("Recipient", "Recipient is present as owner in the system");
            }

            if (ModelState.IsValid)
            {
                account.OpenDate = DateTime.Now;
                db.Accounts.Add(account);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(account);
        }

        // GET: Accounts/Edit/5
        public ActionResult Edit(int? id)
        {
            
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Account account = db.Accounts.Find(id);
            if (account == null)
            {
                return HttpNotFound();
            }
            if (!account.IsOwner(User.Identity.Name))
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            }


            return View(account);
        }

        // POST: Accounts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Owner,Recipient,Name,InterestRate")] Account account)
        {
            if (ModelState.IsValid)
            {
                account.OpenDate = DateTime.Now;
                db.Entry(account).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(account);
        }

        // GET: Accounts/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Account account = db.Accounts.Find(id);
            if (account == null)
            {
                return HttpNotFound();
            }
            if (!account.IsOwner(User.Identity.Name))
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            }
            return View(account);
        }

        // POST: Accounts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Account account = db.Accounts.Find(id);
            db.Accounts.Remove(account);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }


        private List<string> getListOfOwners()
        {
            var accountsOwner = db.Accounts.Select(x => x.Owner);
            return accountsOwner.ToList();
        }

        private List<string> getListOfRecipients()
        {
            var accountsRecipient = db.Accounts.Select(x => x.Recipient);
            return accountsRecipient.ToList();
        }


        private bool checkListForValue(List<string> list, string value)
        {
            if (list != null && list.Count > 0 && !string.IsNullOrWhiteSpace(value))
            {
                return list.Contains(value);
            }
            else
            {
                return false;
            }
        }

    }
}
