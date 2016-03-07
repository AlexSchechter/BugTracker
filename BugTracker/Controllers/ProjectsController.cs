using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BugTracker.Models;
using Microsoft.AspNet.Identity;

namespace BugTracker.Controllers
{
    [Authorize(Roles = "Admin, ProjectManager, Developer")]
    public class ProjectsController : BaseController
    {     
        // GET: Projects
        public async Task<ActionResult> Index()
        {
            string userId = GetUserInfo().Id;
            UserRole role = GetRole();
            ViewBag.Role = role;
            switch (role)
            {
                case UserRole.Admin:
                    return View(await db.Projects.Include(p => p.Manager).ToListAsync());
                case UserRole.ProjectManager:
                    return View(db.Users.Find(userId).ManagedProjects.ToList());
                default:
                    return View(db.Users.Find(userId).AssignedProjects.ToList());
            }
 
        }

        // GET: Projects/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Project project = await db.Projects.FindAsync(id);
            if (project == null)
            {
                return HttpNotFound();
            }
            return View(project);
        }

        [Authorize(Roles = "Admin")]
        // GET: Projects/Create
        public ActionResult Create()
        {

            //var help1 = roleManager.FindByNameAsync(UserRole.Admin.ToString()).Result.Users;
            //var help = new MultiSelectList(roleManager.FindByNameAsync(UserRole.Admin.ToString()).Result.Users, "Id", "Id");

            return View(new ProjectViewModel
            {
                Developers = new MultiSelectList(GetDevelopers(), "Id", "Email"),               
                ProjectManagers = new SelectList(GetProjectManagers(), "Id", "Email")
            });
        }

        // POST: Projects/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "ProjectId,ProjectName,SelectedDevelopersId,SelectedProjectManagerId")] ProjectViewModel model)
        {
            if (ModelState.IsValid)
            {
                Project project = new Project();
                project.Name = model.ProjectName;
                project.ManagerId = model.SelectedProjectManagerId;
                project.CreationDate = DateTime.Now;
                foreach (string developerId in model.SelectedDevelopersId)
                {
                    project.Developers.Add(db.Users.Find(developerId));
                }
                db.Projects.Add(project);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(model);
        }

        // GET: Projects/Edit/5
        [Authorize(Roles="Admin, ProjectManager")]
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Project project = await db.Projects.FindAsync(id);
            if (project == null)
            {
                return HttpNotFound();
            }
            return View(new ProjectViewModel
            {
                Developers = new MultiSelectList(GetDevelopers(), "Id", "Email"),
                ProjectManagers = new SelectList(GetProjectManagers(), "Id", "Email"),
                SelectedDevelopersId = project.Developers.Select(d => d.Id),
                SelectedProjectManagerId = project.ManagerId,
                ProjectId = project.Id,
                ProjectName = project.Name
            });
        }

        // POST: Projects/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize(Roles="Admin,ProjectManager")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "ProjectId,ProjectName,SelectedDevelopersId,SelectedProjectManagerId")] ProjectViewModel model)
        {
            if (ModelState.IsValid)
            {
                Project project = await db.Projects.FindAsync(model.ProjectId);
                project.Name = model.ProjectName;
                project.ManagerId = model.SelectedProjectManagerId;

                var developersAddedId = model.SelectedDevelopersId.Except(project.Developers.Select(d => d.Id));
                var developersRemovedId = project.Developers.Select(d => d.Id).Except(model.SelectedDevelopersId);

                foreach (string developerId in developersAddedId.ToList())
                    project.Developers.Add(db.Users.Find(developerId));

                foreach (string developerId in developersRemovedId.ToList())
                    project.Developers.Remove(db.Users.Find(developerId));

                db.Entry(project).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(model);
        }

        // GET: Projects/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Project project = await db.Projects.FindAsync(id);
            if (project == null)
            {
                return HttpNotFound();
            }
            return View(project);
        }

        // POST: Projects/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Project project = await db.Projects.FindAsync(id);
            db.Projects.Remove(project);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }
    }
}
