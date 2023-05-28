using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using task4.Areas.Identity.Data;

namespace task4.Services
{
    public interface IUserService
    {
        public List<UserModel> GetUsersInList();
        public void BlockUsersById(string[] ids, ClaimsPrincipal userPrincipal);
        public void UnblockUsersById(string[] ids);
        public void DeleteUsersById(string[] ids, ClaimsPrincipal userPrincipal);
        public bool IsUserBlocked(string email);
        public void UpdateLastVisitDate(string username, DateTime newVisitDate);
        public void AddClaims(string username, string claimType, string claimValue);
        public void AddClaims(UserModel user, string claimType, string claimValue);
        public void RemoveClaims(UserModel user, string claimType, string claimValue);
        public void LogoutInactiveUserIfHeOnline(string username, ClaimsPrincipal userPrincipal);
    }

    public class UserService : IUserService
    {
        private readonly ILogger<UserService> _logger;
        private readonly ApplicationDbContext _db;
        private readonly UserManager<UserModel> _userManager;
        private readonly SignInManager<UserModel> _signInManager;

        public UserService(ILogger<UserService> logger, ApplicationDbContext db, UserManager<UserModel> userManager, SignInManager<UserModel> signInManager)
        {
            _logger = logger;
            _db = db;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public List<UserModel> GetUsersInList()
        {
            return _db.Users.ToList();
        }

        public void BlockUsersById(string[] ids, ClaimsPrincipal userPrincipal)
        {
            foreach (var id in ids)
            {
                UserModel? userToBlock = _db.Users.FirstOrDefault(x => x.Id.Equals(id));
                if (userToBlock == null) continue;
                userToBlock.isBlocked = true;
                LogoutInactiveUserIfHeOnline(userToBlock.UserName, userPrincipal);
            }
            _db.SaveChanges();
        }

        public void UnblockUsersById(string[] ids)
        {
            foreach (var id in ids)
            {
                UserModel? userToUnblock = _db.Users.FirstOrDefault(x => x.Id.Equals(id));
                if (userToUnblock == null) continue;
                userToUnblock.isBlocked = false;
            }
            _db.SaveChanges();
        }

        public void DeleteUsersById(string[] ids, ClaimsPrincipal userPrincipal)
        {
            foreach (var id in ids)
            {
                UserModel userToDelete = _userManager.FindByIdAsync(id).Result;
                _db.Remove(userToDelete);
                LogoutInactiveUserIfHeOnline(userToDelete.UserName, userPrincipal);
            }
            _db.SaveChanges();
        }

        public bool IsUserBlocked(string username)
        {
            UserModel userToCheck = _userManager.FindByNameAsync(username).Result;
            return (userToCheck != null ? userToCheck.isBlocked : false);
        }

        public void UpdateLastVisitDate(string username, DateTime newVisitDate)
        {
            UserModel user = _userManager.FindByNameAsync(username).Result;
            if (user == null) return;
            user.LastVisitDate = newVisitDate;
            _db.SaveChanges();
        }

        public void AddClaims(string username, string claimType, string claimValue)
        {
            UserModel user = _userManager.FindByNameAsync(username).Result;
            _userManager.AddClaimAsync(user, new Claim(claimType, claimValue)).Wait();
        }

        public async void AddClaims(UserModel user, string claimType, string claimValue)
        {
            await _userManager.AddClaimAsync(user, new Claim(claimType, claimValue));
        }

        public void RemoveClaims(UserModel user, string claimType, string claimValue)
        {
            _userManager.RemoveClaimAsync(user, new Claim(claimType, claimValue));
        }

        public void LogoutInactiveUserIfHeOnline(string username, ClaimsPrincipal userPrincipal)
        {
            if (userPrincipal.Identity != null && userPrincipal.Identity.IsAuthenticated && username.Equals(userPrincipal.Identity.Name))
            {
                _signInManager.SignOutAsync();
            }
        }
    }
}
