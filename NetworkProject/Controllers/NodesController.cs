using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using NetworkProject.Models;
using System.Collections.ObjectModel;
using Newtonsoft.Json.Linq;

namespace NetworkProject.Controllers
{
    public class NodesController : Controller
    {
        Graph graph = Graph.Instance;

        // GET: Nodes
        public ActionResult Level1()
        {
            ModelState.Clear();
            return View(Graph.Nodes.ToList());
        }

        // GET: Nodes/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Nodes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "NodeID,Name")] Node node)
        {
            if (ModelState.IsValid)
            {
                Graph.InsertNode(node);
                Graph.SaveChangesToJson();
                ViewBag.Nodes = Graph.Nodes;
                return RedirectToAction("Level1");
            }

            ViewBag.Nodes = Graph.Nodes;
            return View(node);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateLink([Bind(Include = "TargetNode,SourceNode")] Link link)
        {
            if ((link.TargetNode != null) || (link.SourceNode != null))
            {
                Graph.InsertLink(link);
                Graph.SaveChangesToJson();
                ViewBag.Nodes = Graph.Nodes;
                return RedirectToAction("Level1");
            }

            ViewBag.Nodes = Graph.Nodes;
            return View(link);
        }

        [HttpGet]
        public ActionResult CreateLink(int TargetNodeId = -1, int SourceNodeId = -1)
        {
            ViewBag.Nodes = Graph.Nodes;
            ViewBag.TargetNodeId = TargetNodeId;
            ViewBag.SourceNodeId = SourceNodeId;
            return View();
        }

        // GET: Nodes/Edit/5+
        [HttpGet]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Node node = Graph.Nodes.Single(i => i.NodeID == id);
            if (node == null)
            {
                return HttpNotFound();
            }

            ViewBag.Nodes = Graph.Nodes;
            return View(node);
        }

        // POST: Nodes/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "NodeID,Name,")] Node node)
        {
            if (ModelState.IsValid)
            {
                Node nodeToUpdate = Graph.Nodes.Single(i => i.NodeID == node.NodeID);
                if ((nodeToUpdate != null) && (node.Name != string.Empty))
                {
                    nodeToUpdate.Name = node.Name;
                    Graph.SaveChangesToJson();
                    ViewBag.Nodes = Graph.Nodes;
                }
                else
                {
                    return HttpNotFound();
                }
               
                return RedirectToAction("Level1");
            }
            return View(node);
        }

        [HttpGet]
        public ActionResult DeleteLink(int? TargetNodeId, int? SourceNodeId)
        {
            if ((TargetNodeId == null) || (SourceNodeId == null))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Link link = new Link();
            link = Graph.Links.Single(s => (s.SourceNode.NodeID == SourceNodeId) &&
                                           (s.TargetNode.NodeID == TargetNodeId));
            if (link == null)
            {
                return HttpNotFound();
            }
            return View(link);
        }

        // POST: Nodes/DeleteLink/5
        [HttpPost, ActionName("DeleteLink")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteLinkConfirmed(int TargetNodeId, int SourceNodeId)
        {
            Link link = new Link();
            link = Graph.Links.Single(s => (s.SourceNode.NodeID == SourceNodeId) && (s.TargetNode.NodeID == TargetNodeId));
            if (link != null)
            {
                Graph.DeleteLink(link);
                Graph.SaveChangesToJson();
                ViewBag.Nodes = Graph.Nodes;
            }
            
            return RedirectToAction("Level1");
        }


        // GET: Nodes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Node node = Graph.Nodes.Single(i => i.NodeID == id);
            if (node == null)
            {
                return HttpNotFound();
            }
            return View(node);
        }

        // POST: Nodes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Node node = Graph.Nodes.Single(i => i.NodeID == id);
            if (node != null)
            {
                Graph.DeleteNode(id);
                Graph.SaveChangesToJson();
                ViewBag.Nodes = Graph.Nodes;
            }
            
            return RedirectToAction("Level1");
        }
    }
}
