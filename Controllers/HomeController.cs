using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using task4.Models;
using System.Security.Claims;
using task4.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using NuGet.Packaging.Signing;
using task4.Services;
using Microsoft.AspNetCore.Authorization;

namespace task4.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUserService _userService;

        public HomeController(ILogger<HomeController> logger, IUserService userService)
        {
            _logger = logger;
            _userService = userService;
        }

        public IActionResult Index()
        {
            List<UserModel> users = _userService.GetUsersInList();
            return View(users);
        }

        [HttpPost]
        public IActionResult BlockUser()
        {
            var idsToBlock = HttpContext.Request.Form["userId"];
            _userService.BlockUsersById(idsToBlock, HttpContext.User);
            return RedirectPermanent("~/Home/Index");
        }

        [HttpPost]
        public IActionResult UnblockUser()
        {
            var idsToUnblock = HttpContext.Request.Form["userId"];
            _userService.UnblockUsersById(idsToUnblock);
            return RedirectPermanent("~/Home/Index");
        }

        [HttpPost]
        public IActionResult DeleteUser()
        {
            var idsToDelete = HttpContext.Request.Form["userId"];
            _userService.DeleteUsersById(idsToDelete, HttpContext.User);
            return RedirectPermanent("~/Home/Index");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}