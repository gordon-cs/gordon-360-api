using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Cors;

namespace CCT_App
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {

            // Web API configuration and services
            var cors = new EnableCorsAttribute("*", "Origin, X-Requested-With, Content-Type, Accept", "POST, GET, OPTIONS, PUT, DELETE");
            config.EnableCors(cors);

            //var formatters = GlobalConfiguration.Configuration.Formatters;
            //var jsonFormatter = formatters.JsonFormatter;
            //var settings = jsonFormatter.SerializerSettings;
            //settings.Formatting = Newtonsoft.Json.Formatting.Indented;
            //settings.ContractResolver = new CamelCasePropertyNamesContractResolver();

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "KJzKJ6FOKx/api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }

       
    }
}
