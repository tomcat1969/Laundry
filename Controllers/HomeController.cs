using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Laundry.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Laundry.Controllers
{
    public class HomeController : Controller
    {

         private MyContext dbContext;
     
        public HomeController(MyContext context)
        {
            dbContext = context;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Register(User user)
        {
            if(ModelState.IsValid)
            {
                if(dbContext.Users.Any(u => u.Email == user.Email))
                {
                    ModelState.AddModelError("Email","Email already in use!");
                    return View("Index");
                }

                PasswordHasher<User> Hasher = new PasswordHasher<User>();
                user.Password = Hasher.HashPassword(user,user.Password);


                dbContext.Add(user);
                dbContext.SaveChanges();

                //set the session
                HttpContext.Session.SetString("UserEmail",user.Email);



                return RedirectToAction("Success");
            }else{
                return View("Index");
            }
        }

        [HttpGet("login")]
        public IActionResult Login()
        {
            return View();
        }

        public IActionResult DoLogin(LoginUser userSubmission)
        {
            if(ModelState.IsValid)
            {
                var userInDB = dbContext.Users.FirstOrDefault(u => u.Email == userSubmission.Email);
                if(userInDB == null)
                {
                    ModelState.AddModelError("Email","Invalid Email");
                    return View("Login");
                } 
                var hasher = new PasswordHasher<LoginUser>();
                var result = hasher.VerifyHashedPassword(userSubmission,userInDB.Password,userSubmission.Password);
                if(result == 0)
                {
                     ModelState.AddModelError("Email","Password does not match!");
                     return View("Login");
                }
                //set the session
                HttpContext.Session.SetString("UserEmail",userSubmission.Email);
                return RedirectToAction("Success");
            }else{
                return View("Login");
            }
            
            
        }

        [HttpGet("success")]
        public IActionResult Success()
        {
            List<User> AllUsers = dbContext.Users.ToList();
            List<WashingMachine> AllWMs= dbContext.WashingMachines.ToList(); 
            foreach(WashingMachine w in AllWMs)
            {
                long elapseTicks = DateTime.Now.Ticks - w.UsedAt.Ticks;
                TimeSpan elapsedSpan = new TimeSpan(elapseTicks);
                w.DurationLeft= w.Duration - elapsedSpan.Minutes;
                if(w.DurationLeft <= 0 && w.status == "inUse"){
                    w.status = "doneWork";
                }
                w.UserId = w.UserId;
                dbContext.SaveChanges();

            }

             List<WashingMachine> AllWMs2= dbContext.WashingMachines.ToList(); 

            
             

            ViewBag.AllWMs = AllWMs2;

            //greetings for login user
            string email_from_session = HttpContext.Session.GetString("UserEmail");
            if(email_from_session == null) return RedirectToAction("Login");

            User the_user = dbContext.Users.FirstOrDefault(u => u.Email == email_from_session);
            ViewBag.Current_User = the_user;




            return View(AllUsers);
        }

        [HttpGet("logout")]
        public RedirectToActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        [HttpGet("editUser/{UserId}/{CurrentUserId}")]
        public IActionResult EditUser(int UserId,int CurrentUserId)
        {
            User the_user = dbContext.Users.FirstOrDefault(u => u.UserId == UserId);
            ViewBag.CurrentUser = dbContext.Users.FirstOrDefault(u => u.UserId == CurrentUserId);
            return View(the_user);
        }

        public IActionResult DoEditUser(User userformSummission)
        {
                
                User RetrievedUser = dbContext.Users.FirstOrDefault(user => user.UserId == userformSummission.UserId);
                
                RetrievedUser.FirstName = userformSummission.FirstName;
                RetrievedUser.LastName = userformSummission.LastName;
                RetrievedUser.Email = userformSummission.Email;
                RetrievedUser.IsAdmin = userformSummission.IsAdmin;

                PasswordHasher<User> Hasher = new PasswordHasher<User>();
                
                // RetrievedUser.Password = Hasher.HashPassword(RetrievedUser,userformSummission.Password);
                
                RetrievedUser.UpdatedAt = DateTime.Now;
                dbContext.SaveChanges();
                return RedirectToAction("success");
        }

        [HttpGet("deleteUser/{UserId}/{CurrentUserId}")]
        public IActionResult deleteUser(int UserId,int CurrentUserId)
        {
            User current_user = dbContext.Users.FirstOrDefault(u => u.UserId == CurrentUserId);
            if(!current_user.IsAdmin){
                return RedirectToAction("Success");
            }
            User the_user = dbContext.Users.SingleOrDefault(u => u.UserId == UserId);
            dbContext.Users.Remove(the_user);
            dbContext.SaveChanges();
            return RedirectToAction("Success");
        }

        [HttpGet("newWm/{UserId}")]
         public IActionResult NewWM(int UserId)
         {
                ViewBag.UserId = UserId;
                return View();
         }
        public IActionResult AddNewWM(WashingMachine wFromSubmit)
        {
             if(ModelState.IsValid)
             {
                  dbContext.Add(wFromSubmit);
                  dbContext.SaveChanges();
                  return RedirectToAction("Success");
             }else{
                 return View("NewWM");
             }
        }

        [HttpPost]
        [Route("addNewWMevent")]
        public IActionResult AddNewWMevent(int UserId,int WashingMachineId,int Duration)
        {
            WashingMachine WMinDB = dbContext.WashingMachines.FirstOrDefault(wm => wm.WashingMachineId == WashingMachineId);
            WMinDB.UsedAt = DateTime.Now;
            WMinDB.Duration = Duration;
            WMinDB.UserId = UserId;
            WMinDB.status = "inUse";
            

            dbContext.SaveChanges();


            return RedirectToAction("Success");

        }

        [HttpGet("resetWM/{WashingMachineId}/{UserId}")]
        public IActionResult ResetWM(int WashingMachineId,int UserId)
        {
            WashingMachine RetrievedWM = dbContext.WashingMachines.FirstOrDefault(w => w.WashingMachineId == WashingMachineId);
            RetrievedWM.status = "ready";
            dbContext.SaveChanges();
            return RedirectToAction("Success");
        }

        ////////////////////////
        [HttpGet("test")]
        public IActionResult Test()
        {
           

            return View();
        }

///////////////////////////////////////////////////////////////////////////////////////////////////
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
