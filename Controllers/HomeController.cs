using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebLogin.Models;

namespace WebLogin.Controllers
{
    public class HomeController : Controller
    {
        private UserREST rest;

        public HomeController(UserREST service)
        {
            rest = service;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Content()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("user")))
            {
                ViewBag.message = "Please login first";
                return View("Index");
            }
            else
            {
                return View();
            }
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(IFormCollection form)
        {
            string username = form["username"];
            string password = form["password"];
            User user = rest.getUser(username).Result;
            if (user.username != "" && user.password == password)
            {
                HttpContext.Session.SetString("user", user.fullname);
                ViewBag.message = user.fullname + ": welcome back";
                return View("Content");
            }
            else
            {
                ViewBag.message = "Username/password not found";
                return View();
            }
        }

        [HttpGet]
        public IActionResult Register()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("user")))
            {
                ViewBag.message = "Please login first";
                return View("Index");
            }
            else
            {
                return View();
            }
        }

        [HttpPost]
        public IActionResult Register(IFormCollection form)
        {
            User user = new User
            {
                fullname = form["fullname"],
                username = form["username"],
                password = form["password"]
            };
            User check = rest.getUser(user.username).Result;
            if (check.username == null)
            {
                ViewBag.message = rest.registerUser(user).Result;
                return View("Content");
            }
            else
            {
                ViewBag.message = "Username unavailable";
                return View();
            }
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            ViewBag.message = "You have been logged out";
            return View("Index");
        }

        public IActionResult Manage()
        {
            ViewBag.message = "Let's manage";
            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
        }
    }
}