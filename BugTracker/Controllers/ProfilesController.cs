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
    public class ProfilesController : BaseController
    {
        [Authorize(Roles = "Admin")]
        public ActionResult Index(int? position)
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
                    Role = (UserRole)GetRole(user.Id)
                });
            }
            ViewBag.PickPosition = position;
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
                Role = (UserRole)GetRole(user.Id),
            });
        }

        //GET: /Profiles/EditProfile //Edit Own Profile
        [HttpGet]
        [ChildActionOnly]
        public ActionResult EditProfile(ProfileViewModel profile)
        {           
            profile.UserId = db.Users.FirstOrDefault(u => u.UserName == profile.Username).Id;
            return View(profile);
        }

        //POST: /Profiles/EditProfile 
        [HttpPost]
        public async Task<ActionResult> EditProfile(ProfileViewModel profile, UserRole? oldRole)
        {
            if (profile == null)
                RedirectToAction("Index", "Home");

            ApplicationUser user = db.Users.Find(profile.UserId);    
            user.UserName = profile.Username;
            //user.Email = profile.Email;
            user.FirstName = profile.FirstName;
            user.LastName = profile.LastName;
            if(oldRole != null && GetRole() == UserRole.Admin)
            {
                await UserManager.RemoveFromRoleAsync(user.Id, oldRole.ToString());
                await UserManager.AddToRoleAsync(user.Id, profile.Role.ToString());
            }          
            await db.SaveChangesAsync();

            if (oldRole == null)
                return RedirectToAction("UserProfile");
            return RedirectToAction("Index");
        }

        //GET: /Profile/Edit  //edit someone's else profile
        [Authorize(Roles = "Admin")] 
        public ActionResult Edit(string email)
        {
            if (email == null)
                return RedirectToAction("Index");

            ApplicationUser user = db.Users.FirstOrDefault(u => u.Email == email);
            UserRole role = (UserRole)GetRole(user.Id);
            ViewBag.OldRole = role;
            return View(new ProfileViewModel
            {
                UserId = user.Id,
                //Email = user.Email,
                Username = user.UserName,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Role = role
            });

        }
    }
}