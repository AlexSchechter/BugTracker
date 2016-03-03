using BugTracker.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace BugTracker.Controllers
{
    public class ProfileController : BaseController
    {
        //GET /Profile/UserProfile
        public ActionResult UserProfile()
        {
            ApplicationUser user = GetUserInfo();
            if (user == null)
                return RedirectToAction("Index", "Home");

            string RoleId = userManager.FindById(user.Id).Roles.First().RoleId;

            return View(new ProfileViewModel
            {
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Username = user.UserName,
                Role = (UserRole)Enum.Parse(typeof(UserRole), roleManager.FindById(RoleId).Name)
            });
        }

        //GET: /Manage/EditProfile
        [HttpGet]
        [ChildActionOnly]
        public ActionResult EditProfile(ProfileViewModel profile)
        {
            ViewBag.OldRole = profile.Role;
            return View(profile);
        }

        //POST: /Profile/EditProfile
        [HttpPost]
        public async Task<ActionResult> EditProfile(ProfileViewModel profile, UserRole oldRole)
        {
            if (profile == null)
                RedirectToAction("Index", "Home");

            ApplicationUser user = GetUserInfo();         
            user.UserName = profile.Username;
            user.Email = profile.Email;
            user.FirstName = profile.FirstName;
            user.LastName = profile.LastName;
            await userManager.RemoveFromRoleAsync(user.Id, oldRole.ToString());
            await userManager.AddToRoleAsync(user.Id, profile.Role.ToString());
            await db.SaveChangesAsync();
            return RedirectToAction("UserProfile");
        }
    }
}