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
    [Authorize(Roles = "Admin, ProjectManager")]
    public class ProjectsController : BaseController
    {
       
        // GET: Projects
        public async Task<ActionResult> Index()
        {
            List<Project> projects = await db.Projects.Include(p => p.Manager).ToListAsync();
            return View(projects);
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
        public async Task<ActionResult> Create([Bind(Include = "ProjectId,ProjectName,SelectedDevelopers,SelectedProjectManager")] ProjectViewModel model)
        {
            if (ModelState.IsValid)
            {
                Project project = new Project();
                project.Name = model.ProjectName;
                project.ManagerId = model.SelectedProjectManager;
                foreach (string userId in model.SelectedDevelopers)
                {
                    var user = db.Users.Find(userId);
                    project.Developers.Add(user);
                }
                db.Projects.Add(project);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(model);
        }


        // GET: Projects/Edit/5
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
            ViewBag.ManagerId = new SelectList(db.Users, "Id", "FirstName", project.ManagerId);
            return View(project);
        }

        // POST: Projects/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Name,ManagerId,CreationDate")] Project project)
        {
            if (ModelState.IsValid)
            {
                db.Entry(project).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.ManagerId = new SelectList(db.Users, "Id", "FirstName", project.ManagerId);
            return View(project);
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
