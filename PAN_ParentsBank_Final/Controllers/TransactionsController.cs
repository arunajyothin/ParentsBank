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
    public class TransactionsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Transactions
        public ActionResult Index()
        {
            string currentlyLoggedInUser = User.Identity.Name;
            var transactions = db.Transactions
                .Include(t => t.Account)
                 .Where(x => x.Account.Owner == currentlyLoggedInUser || x.Account.Recipient == currentlyLoggedInUser);
            return View(transactions.ToList());
        }

        // GET: Transactions/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Transaction transaction = db.Transactions.Find(id);
            if (transaction == null)
            {
                return HttpNotFound();
            }
            if (!transaction.Account.IsOwnerOrRecipient(User.Identity.Name))
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            }
            return View(transaction);
        }

        // GET: Transactions/Create
        public ActionResult Create()
        {

            string currentlyLoggedInUsername = User.Identity.Name;
            if (checkListForValue(getListOfRecipients(), User.Identity.Name))
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            }
            var accounts = db.Accounts
                .Where(x => x.Owner == currentlyLoggedInUsername
                || x.Recipient == currentlyLoggedInUsername).ToList();
            ViewBag.AccountId = new SelectList(accounts, "Id", "Recipient");
            return View();
        }

        // POST: Transactions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,AccountId,TransactionDate,Amount,Note,TypeOfTransaction")] Transaction transaction)
        {
            var account = db.Accounts.Find(transaction.AccountId);
            if (transaction.TypeOfTransaction == "Debit" && transaction.Amount > account.getAccountBalance())
            {
                ModelState.AddModelError("Amount", "Insufficient Balance");
            }

            if (ModelState.IsValid)
            {
                db.Transactions.Add(transaction);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.AccountId = new SelectList(db.Accounts, "Id", "Owner", transaction.AccountId);
            return View(transaction);            
        }

        // GET: Transactions/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Transaction transaction = db.Transactions.Find(id);
            if (transaction == null)
            {
                return HttpNotFound();
            }
            if (!transaction.Account.IsOwner(User.Identity.Name))
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            }
            string currentlyLoggedInUsername = User.Identity.Name;
            var accounts = db.Accounts
                .Where(x => x.Owner == currentlyLoggedInUsername
                || x.Recipient == currentlyLoggedInUsername).ToList();
            ViewBag.AccountId = new SelectList(accounts, "Id", "Recipient");
            return View(transaction);
        }

        // POST: Transactions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,AccountId,TransactionDate,Amount,Note,TypeOfTransaction")] Transaction transaction)
        {
            if (ModelState.IsValid)
            {
                db.Entry(transaction).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.AccountId = new SelectList(db.Accounts, "Id", "Owner", transaction.AccountId);
            return View(transaction);
        }

        // GET: Transactions/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Transaction transaction = db.Transactions.Find(id);
            if (transaction == null)
            {
                return HttpNotFound();
            }
            if (!transaction.Account.IsOwner(User.Identity.Name))
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            }
            return View(transaction);
        }

        // POST: Transactions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Transaction transaction = db.Transactions.Find(id);
            db.Transactions.Remove(transaction);
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
