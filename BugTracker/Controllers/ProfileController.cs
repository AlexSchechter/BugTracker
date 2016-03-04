using BugTracker.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace BugTracker.Controllers
{
    public class ProfileController : BaseController
    {
        //[Authorize(Roles = "Admin")]
        public ActionResult Index()
        {
            List<ProfileViewModel> profiles = new List<ProfileViewModel>();
            foreach (ApplicationUser user in db.Users)
            {
                profiles.Add(new ProfileViewModel
                {
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    Username = user.UserName,
                    Role = GetRole(user.Id)
                });
            }
            return View(profiles);
        }
        
        //GET /Profile/UserProfile
        public ActionResult UserProfile()
        {
            ApplicationUser user = GetUserInfo();
            if (user == null)
                return RedirectToAction("Index", "Home");

            return View(new ProfileViewModel
            {
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Username = user.UserName,
                Role = GetRole(user.Id)
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