﻿using System.Web.Mvc;

namespace AppFactory.MVC.Areas.Admin
{
    public class AdminAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Admin";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "Admin_default",
                "Admin/{controller}/{action}/{id}",
                new { id = UrlParameter.Optional }, 
                namespaces: new[] { "AppFactory.MVC.Areas.Admin.Controllers" }
            );
        }
    }
}