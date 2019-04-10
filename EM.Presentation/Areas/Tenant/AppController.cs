using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EM.Presentation.Areas.Tenant
{
    public class AppController : Controller
    {
        public EM.Domain.Entities.Tenant current_tenant
        {
            get
            {
                object apptenant;
                if (!Request.GetOwinContext().Environment.TryGetValue("Tenant", out apptenant))
                {
                    return null;
                }
                return (EM.Domain.Entities.Tenant)apptenant;
            }

        }
    }
}