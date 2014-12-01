using NetworkProject.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;

namespace NetworkProject.Controllers
{
    public class GraphController : Controller
    {
        Graph graph = Graph.Instance;

        // GET: Graph
        public ActionResult Level2()
        {            
            Graph.SaveChangesToJson();
            return View();
        }

        // GET: Graph
        public ActionResult Level3()
        {
            Graph.SaveChangesToJson();
            return View();
        }

        [HttpGet]
        public ActionResult VisualizationData()
        {            
            FileInfo file = new FileInfo(Path.Combine(HostingEnvironment.ApplicationPhysicalPath, @"App_Data\nodes.json"));

            return File(file.Open(FileMode.Open, FileAccess.Read), "application/json", "nodes.json");
        }


    }
}