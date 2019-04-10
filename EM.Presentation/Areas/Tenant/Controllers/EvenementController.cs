using EM.Domain.Entities;
using EM.Presentation.Areas.Tenant.Models;
using EM.Service;
using EM.Service.UserService;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace EM.Presentation.Areas.Tenant.Controllers
{
    public class EvenementController : AppController
    {

        IUserService UserService;
        private ApplicationUserManager _userManager;
        private ApplicationRoleManager _roleManager;
        private EM.Domain.Entities.Tenant _tenant;

        Evenement_service aa = new Evenement_service();
        public string recieverEmail;
        public string subject = "Nouveau Suivi effectuer";
        public string message = "";
        public string subject2 = "Votre Suivi a ete supprimer";
        public string message2 = "Bonjour, votre suivi a ete supprimé avec succé cordialement";
        public string subject3 = "Votre Suivi a ete modifier";
        public string message3 = "";

        public EvenementController()
        {
            UserService = new UserService();
        }
        public EvenementController(ApplicationUserManager userManager, ApplicationRoleManager roleManager):base()
        {
            UserManager = userManager;
            RoleManager = roleManager;
        }

        public EM.Domain.Entities.Tenant Current_Tenant
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


        // GET: Tenant/Evenement
        public ActionResult Home()
        {
            return View();
        }

        public ActionResult Index()
        {
            var tasks = aa.GetMany();
            return View(tasks);
        }

        // GET: Evenement/Details/5
        public ActionResult Details(int id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Evenement conge = aa.GetById(id);
            if (conge == null)
            {
                return HttpNotFound();
            }
            return View(conge);
        }

        // GET: Evenement/Create
        public ActionResult Create()
        {
            return View();
        }

        public ActionResult ExportData()
        {
            var datasource = aa.GetMany().ToList();

            GridView gv = new GridView();
            gv.DataSource = datasource;
            gv.DataBind();
            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename=Report.xls");
            Response.ContentType = "application/ms-excel";
            Response.Charset = "";
            StringWriter sw = new StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(sw);
            gv.RenderControl(htw);
            Response.Output.Write(sw.ToString());
            Response.Flush();
            Response.End();

            return Json("Success");
        }

        // POST: Evenement/Create
        [HttpPost]
        public ActionResult Create(Evenementmodel pm)
        {
            Evenement p = new Evenement();
            p.Picture = pm.Picture;
            p.theme = pm.theme;
            p.StartDate = pm.StartDate;
            p.EndDate = pm.EndDate;
            p.location = pm.location;
            //2eme etape
            //prods.Add(p);
            //3eme etape
            //Session["Products"] = prods;
            aa.Add(p);
            aa.Commit();
            Evenement e = new Evenement();
            var emp = aa.GetMany().Where(c => c.EventId.Equals(p.EventId)).FirstOrDefault();

            var senderemail = new MailAddress("achrefmejri32@gmail.com", "NeoXam Salaire");
            var recieveremail = "achref.mejri@esprit.tn";
            var password = "Achrefmejri1";

            message = "Bonjour, " + "\n" + "votre suivi a ete ajouter avec succé ! " + "\n" +
            " " + "\n" + "Nouvelle Evaluation : " + p.StartDate + "\n" +

            " " + "\n" + "Nouveau Entretient : " + p.EndDate + "\n" +

            " " + "\n" + "Nouvelle Date : " + p.theme + "\n" +

            " " + "\n" + "" + "\n" + "cordialement ";
            var sub = subject;
            var body = message;
            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(senderemail.Address, password)
            };
            using (var mess = new MailMessage(senderemail.Address, recieveremail)
            {
                Subject = subject,
                Body = body
            }
                )
            {
                smtp.Send(mess);
            }
            aa.Commit();
            return RedirectToAction("Index");
            //enregistrer l'image


        }

        // GET: Evenement/Edit/5
        public ActionResult Edit(int id)
        {
            Evenement p = aa.GetById(id);

            //    Domain.Entities.employe cat = se.GetById(id);

            Evenementmodel pm = new Evenementmodel();


            var congeModel = new Evenementmodel
            {
                EventId = p.EventId,
                theme = p.theme,
                Picture = p.Picture,
                StartDate = p.StartDate,
                EndDate = p.EndDate,
                location = p.location,


            };


            return View(congeModel);
        }

        // POST: Evenement/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, Evenementmodel congemodel)
        {
            try
            {
                var conge = aa.GetMany().Where(c => c.EventId.Equals(id)).FirstOrDefault();

                // TODO: Add update logic here
                conge.EventId = congemodel.EventId;
                conge.location = congemodel.location;
                conge.Picture = congemodel.Picture;
                conge.theme = congemodel.theme;
                conge.StartDate = congemodel.StartDate;
                conge.EndDate = congemodel.EndDate;




                aa.Update(conge);



                aa.Commit();
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Evenement/Delete/5
        public ActionResult Delete(int id)
        {
            var Conge = aa.GetMany().Where(c => c.EventId.Equals(id)).FirstOrDefault();
            aa.Delete(Conge);
            aa.Commit();
            return RedirectToAction("Index");
        }

        // POST: Evenement/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                var Conge = aa.GetMany().Where(c => c.EventId.Equals(id)).FirstOrDefault();
                aa.Delete(Conge);
                aa.Commit();
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }


        public JsonResult GetSearch(string s)
        {
            List<Evenement> listerconge = aa.GetMany(c => c.theme.Contains(s)).ToList<Evenement>();

            return new JsonResult { Data = listerconge, JsonRequestBehavior = JsonRequestBehavior.AllowGet };

        }


        [HttpPost]
        public JsonResult GetStat()
        {


            var dt = aa.GetMany().GroupBy(a => a.theme)
                        .Select(g => new { g.Key, Count = g.Count() });
            return new JsonResult { Data = dt };


        }


    }
}
