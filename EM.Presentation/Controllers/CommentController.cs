using EM.Data;
using EM.Domain.Entities;
using EM.Presentation.Areas.Tenant;
using EM.Presentation.Areas.Tenant.Helpers;
using EM.Presentation.Models;
using EM.Service;
using EM.Service.UserService;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace EM.Presentation.Controllers
{
    public class CommentController : AppController
    {

        IUserService UserService;
        private ApplicationUserManager _userManager;
        private ApplicationRoleManager _roleManager;
        private Tenant _tenant;
        IParticipantService MyParticipantService;
        ICommentService MyCommentService;
        IReplyService MyReplyService;

        public CommentController()
        {
            UserService = new UserService();
            MyParticipantService = new ParticipantService();
            MyCommentService = new CommentService();
            MyReplyService = new ReplyService();
        }
        public CommentController(ApplicationUserManager userManager, ApplicationRoleManager roleManager):base()
        {
            UserManager = userManager;
            RoleManager = roleManager;
        }

        public Tenant Current_Tenant
        {
            get
            {
                return _tenant ?? base.current_tenant;
            }
            private set
            {
                _tenant = value;
            }
        }
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        public IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        public ApplicationRoleManager RoleManager
        {
            get
            {
                return _roleManager ?? Request.GetOwinContext().GetUserManager<ApplicationRoleManager>();
            }
            private set
            {
                _roleManager = value;
            }
        }


        /************************************ Actions ************************************************************/


        
        // GET: Comment
        public ActionResult Index()
        {
            var Comments = new List<CommentVM>();
            ApplicationUser current_user = UserManager.FindById(System.Web.HttpContext.Current.User.Identity.GetUserId());
            foreach (Comment c in MyCommentService.GetMany())
            {
                Comments.Add(new CommentVM()
                {
                    Id = c.Id,
                    Text = c.Text,
                    CreatedOn = c.CreatedOn,
                    ParticipantId = c.ParticipantId

                });

            }


            return View(Comments);
        }

        // GET: Comment/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Comment/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Comment/Create
        [HttpPost]
        [TenantAuthorize(Roles = "Participant")]
        public PartialViewResult Create(string contenu)
        {
            ApplicationUser current_user = UserManager.FindById(System.Web.HttpContext.Current.User.Identity.GetUserId());
            Comment CommentDoamin = new Comment()
            {

                Text = contenu,
                CreatedOn = DateTime.UtcNow,
                ParticipantId = current_user.Id
                
            };
            MyCommentService.Add(CommentDoamin);
            MyCommentService.Commit();


            return PartialView("OneComment",new CommentVM{Text= CommentDoamin.Text,
                CreatedOn= CommentDoamin.CreatedOn,
                ParticipantId= CommentDoamin.ParticipantId
            });
        }

        // GET: Comment/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Comment/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Comment/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Comment/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            Comment c = MyCommentService.GetById(id);
            MyCommentService.Delete(c);
            MyCommentService.Commit();
                

                return RedirectToAction("Index");
           
        }



        public ActionResult Edit2(int id)
        {

            Comment p = MyCommentService.GetById(id);
            CommentVM pm = new CommentVM();

            pm.ParticipantId = p.ParticipantId;
            pm.Text = p.Text;
            pm.CreatedOn = p.CreatedOn;
            return View(pm);
        }

        // POST: Resource/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit2(int id, CommentVM pm)
        {

            try
            {

                Comment p = MyCommentService.GetById(id);

                p.ParticipantId = pm.ParticipantId;
                p.Text = pm.Text;
                p.CreatedOn = pm.CreatedOn;
                MyCommentService.Update(p);
                MyCommentService.Commit();


                // TODO: Add update logic here




                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                return View(pm);
            }
        }
    }
}
