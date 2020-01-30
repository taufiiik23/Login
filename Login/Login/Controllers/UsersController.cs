using Login.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Login.Controllers
{
    public class UsersController : Controller
    {
        ApplicationDbContext myContext = new ApplicationDbContext();
        // GET: Users
        public ActionResult Index()
        {
            var list = myContext.Users.ToList();
            return View(list);
        }

        // GET: Users/Details/5
        public ActionResult Details(int id)
        {
            var details = myContext.Users.Find(id);
            return View(details);
        }

        // GET: Users/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Users/Create
        [HttpPost]
        public ActionResult Create(User user)
        {
            try
            {
                // TODO: Add insert logic here
                var mySalt = BCrypt.Net.BCrypt.GenerateSalt();
                user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password, mySalt);
                myContext.Users.Add(user);
                myContext.SaveChanges();
                //if(result > 0)
                //{
                //    Console.WriteLine("Data Berhasil di Tambahkan");
                //}
                //else
                //{
                //    Console.WriteLine("Data Gagal di Tambahkan");
                //}


                MailMessage mail = new MailMessage();
                mail.To.Add(user.Email);
                mail.From = new MailAddress("m.taufikhhh@gmail.com");
                mail.Subject = user.Email;
                string Body = user.Password;
                mail.Body = Body;
                mail.IsBodyHtml = true;
                SmtpClient smtp = new SmtpClient();
                smtp.Host = "smtp.gmail.com";
                smtp.Port = 587;
                smtp.UseDefaultCredentials = true;
                smtp.Credentials = new System.Net.NetworkCredential("m.taufikhhh@gmail.com", "Admin12345678");
                smtp.EnableSsl = true;
                smtp.Send(mail);


                return RedirectToAction ("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Users/Edit/5
        public ActionResult Edit(int id)
        {
            var edit = myContext.Users.Find(id);
            return View();
        }

        // POST: Users/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, User user)
        {
            try
            {
                // TODO: Add update logic here
                var edit = myContext.Users.Find(id);
                edit.Email = user.Email;
                edit.Password = user.Password;
                myContext.Entry(edit).State = System.Data.Entity.EntityState.Modified;
                myContext.SaveChanges();
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Users/Delete/5
        public ActionResult Delete(int id)
        {
            var delete = myContext.Users.Find(id);
            return View();
        }

        // POST: Users/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, User user)
        {
            try
            {
                // TODO: Add delete logic here
                var delete = myContext.Users.Find(id);
                myContext.Users.Remove(delete);
                myContext.SaveChanges();
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
        [HttpGet]
        public ActionResult FormLogin()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> FormLogin(User user)
        {
            var login = await myContext.Users.FirstOrDefaultAsync(a => a.Email.Equals(user.Email));
            if(login != null)
            {
                var password = BCrypt.Net.BCrypt.Verify(user.Password, login.Password);
                if(password == true)
                {
                    Session["id"] = user.Id;
                    Session.Add("Email", user.Email);

                    return RedirectToAction("Users","Index");
                }
                
            }
            return RedirectToAction("Index");
            //if (login != null && BCrypt.Net.BCrypt.Verify(user.Password, login.Password))
            //{
            //    return RedirectToAction("Index");
            //}
            //else
            //{
            //    return RedirectToAction("FormLogin");
            //}
        }

        public ActionResult LogOut()
        {
            Session.Remove("id");
            Session.Remove("Email");
            return RedirectToAction("Index", "FormLogin");
        }

    }
}
