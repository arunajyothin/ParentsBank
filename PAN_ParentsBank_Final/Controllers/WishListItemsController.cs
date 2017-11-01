using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using PAN_ParentsBank_Final.Models;
using System.Text.RegularExpressions;

namespace PAN_ParentsBank_Final.Controllers
{
    [Authorize]
    public class WishListItemsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: WishListItems
        public ActionResult Index()
        {

            string currentlyLoggedInUser = User.Identity.Name;
            var wishListItems = db.WishListItems
                .Include(w => w.Account)
                .Where(x => x.Account.Owner == currentlyLoggedInUser || x.Account.Recipient == currentlyLoggedInUser);
            return View(wishListItems.ToList());
        }

        // GET: WishListItems/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            WishListItem wishListItem = db.WishListItems.Find(id);
            if (wishListItem == null)
            {
                return HttpNotFound();
            }
            if (!wishListItem.Account.IsOwnerOrRecipient(User.Identity.Name))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            return View(wishListItem);
        }

        // GET: WishListItems/Create
        public ActionResult Create()
        {
            string currentlyLoggedInUsername = User.Identity.Name;
            var accounts = db.Accounts
                .Where(x => x.Owner == currentlyLoggedInUsername
                || x.Recipient == currentlyLoggedInUsername).ToList();
            ViewBag.AccountId = new SelectList(accounts, "Id", "Recipient");
            return View();
        }

        // POST: WishListItems/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,AccountId,DateAdded,Cost,Description,Link,Purchased")] WishListItem wishListItem)
        {
            wishListItem.DateAdded = DateTime.Now;
            if (ModelState.IsValid)
            {
                db.WishListItems.Add(wishListItem);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.AccountId = new SelectList(db.Accounts, "Id", "Owner", wishListItem.AccountId);
            return View(wishListItem);
        }

        // GET: WishListItems/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            WishListItem wishListItem = db.WishListItems.Find(id);
            if (wishListItem == null)
            {
                return HttpNotFound();
            }
            if (!wishListItem.Account.IsOwnerOrRecipient(User.Identity.Name))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            string currentlyLoggedInUsername = User.Identity.Name;
            var accounts = db.Accounts
                .Where(x => x.Owner == currentlyLoggedInUsername
                || x.Recipient == currentlyLoggedInUsername).ToList();
            ViewBag.AccountId = new SelectList(accounts, "Id", "Recipient");
            return View(wishListItem);
        }

        // POST: WishListItems/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,AccountId,DateAdded,Cost,Description,Link,Purchased")] WishListItem wishListItem)
        {
            if (ModelState.IsValid)
            {
                db.Entry(wishListItem).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.AccountId = new SelectList(db.Accounts, "Id", "Owner", wishListItem.AccountId);
            return View(wishListItem);
        }

        // GET: WishListItems/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            WishListItem wishListItem = db.WishListItems.Find(id);
            if (wishListItem == null)
            {
                return HttpNotFound();
            }
            if (!wishListItem.Account.IsOwner(User.Identity.Name))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            return View(wishListItem);
        }

        // POST: WishListItems/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            WishListItem wishListItem = db.WishListItems.Find(id);
            db.WishListItems.Remove(wishListItem);
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

        public ActionResult Search(string description, string cost)
        {

            string currentlyLoggedInUser = User.Identity.Name;
            var wishListItems = db.WishListItems
              .Include(w => w.Account)
              .Where(x => x.Account.Owner == currentlyLoggedInUser || x.Account.Recipient == currentlyLoggedInUser);
            if (string.IsNullOrWhiteSpace(description) && string.IsNullOrWhiteSpace(cost))
            {
                return View(wishListItems.ToList());
            }
            else
            {

                if (!string.IsNullOrWhiteSpace(description))
                {
                    wishListItems = wishListItems.Where(x => x.Description.Contains(description));
                }

                if (!string.IsNullOrWhiteSpace(cost) && Regex.IsMatch(cost, @"^\d+([.]\d+)?$"))
                {
                    decimal costDecimal = Decimal.Parse(cost);
                    wishListItems = wishListItems.Where(s => s.Cost == costDecimal);
                }

                return View(wishListItems.ToList());
            }
        }
    }
}
