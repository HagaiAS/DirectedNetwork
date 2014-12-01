using NetworkProject.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Web;

namespace NetworkProject.Models
{
    /// <summary>
    /// Graph repository Singleton class
    /// </summary>
    public class Graph
    {
         public static Collection<Node> Nodes { get; set; }
         public static Collection<Link> Links { get; set; }

         // static holder for instance, need to use lambda to construct since constructor private
         private static readonly Lazy<Graph> _instance = new Lazy<Graph>(() => new Graph());
  
         // private to prevent direct instantiation.
         private Graph()
         {
             Nodes = new Collection<Node>();
             Links = new Collection<Link>();
             InitDbWithJson();
             //SaveChangesToJson();
         }
  
         // accessor for instance
         public static Graph Instance
         {
             get
             {
                 return _instance.Value;
             }
         }

        /// <summary>
        /// Inserts the node to repository.
        /// </summary>
        /// <param name="nodeToAdd">The node object to insert.</param>
        public static void InsertNode(Node nodeToAdd)
        {
            if (nodeToAdd != null)
            {
                nodeToAdd.NodeID = Nodes.Count;
                nodeToAdd.InLinks = new Collection<Link>();
                nodeToAdd.OutLinks = new Collection<Link>();
                Nodes.Add(nodeToAdd);
            }
        }

        /// <summary>
        /// Deletes the node from repository.
        /// </summary>
        /// <param name="nodeToAdd">The node object to insert.</param>
        public static void DeleteNode(int nodeToDeleteID)
        {

            if (nodeToDeleteID > -1)
            {

                Node nodeToDelete = Nodes[nodeToDeleteID];
                // Delete all in links the contain the node.
                foreach (var link in nodeToDelete.InLinks)
	            {
                    link.SourceNode.OutLinks.Remove(link);

                    Links.Remove(link);
	            }

                // Delete all out links the contain the node.
                foreach (var link in nodeToDelete.OutLinks)
                {
                    link.TargetNode.InLinks.Remove(link);

                    Links.Remove(link);
                }

                Nodes.RemoveAt(nodeToDeleteID);
                for (int i = nodeToDeleteID; i < Nodes.Count; i++)
                {
                    Nodes[i].NodeID -= 1;
                    //FixDeathSpace(Nodes[i]);
                }                
            }
        }

        protected static void FixDeathSpace(Node nodeToUpdate)
        {
            nodeToUpdate.NodeID -= 1;

            foreach (Link link in nodeToUpdate.InLinks)
            {
                DeleteLink(link);
                link.TargetNode.NodeID -= 1;
                InsertLink(link);
            }

            foreach (Link link in nodeToUpdate.OutLinks)
            {
                DeleteLink(link);
                link.SourceNode.NodeID -= 1;
                InsertLink(link);
            }
        }

        /// <summary>
        /// Deletes link from repository.
        /// </summary>
        /// <param name="linkToDelete">The Link To Delete</param>
        public static void DeleteLink(Link linkToDelete)
        {
            if (linkToDelete != null)
            {
                Links.Remove(linkToDelete);
                linkToDelete.SourceNode.OutLinks.Remove(linkToDelete);
                linkToDelete.TargetNode.InLinks.Remove(linkToDelete);
            }
        }

        /// <summary>
        /// Insert link to repository.
        /// </summary>
        /// <param name="linkToAdd">The Link To Add</param>
        public static void InsertLink(Link linkToAdd)
        {
            if (linkToAdd != null)
            {
                linkToAdd.SourceNode = Nodes[linkToAdd.SourceNode.NodeID];
                linkToAdd.TargetNode = Nodes[linkToAdd.TargetNode.NodeID];
                Links.Add(linkToAdd);
                Nodes[linkToAdd.SourceNode.NodeID].OutLinks.Add(linkToAdd);
                Nodes[linkToAdd.TargetNode.NodeID].InLinks.Add(linkToAdd);                
            }
        }

        public static void SaveChangesToJson()
        {
            var path = System.Web.HttpContext.Current.Server.MapPath("\\App_Data\\nodes.json");
            JObject jsonObject = new JObject();

            //jsonObject["nodes"] = JsonConvert.SerializeObject(Nodes);
            
            //// serialize JSON to a string and then write string to a file
            //File.WriteAllText(path, JsonConvert.SerializeObject(Nodes));
            
            //// serialize JSON to a string and then write string to a file
            //File.WriteAllText(path, JsonConvert.SerializeObject(Links));

            JArray jNodesArray = new JArray(Nodes.Select(p => new JObject
               {
                   { "id", p.NodeID },
                   { "name", p.Name }
               })
            );
            jsonObject["nodes"] = jNodesArray;

            JArray jLinksArray = new JArray(Links.Select(p => new JObject
               {
                   { "source", p.SourceNode.NodeID },
                   { "target", p.TargetNode.NodeID }
               })
            );
            jsonObject["links"] = jLinksArray;           

            // serialize JSON directly to a file
            using (StreamWriter file = File.CreateText(path))
            {
                JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings();
                jsonSerializerSettings.Formatting = Formatting.Indented;
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(file, jsonObject);
            }
        }

        //public static void SaveChangesToJsonForVisualData()
        //{
        //    var path = System.Web.HttpContext.Current.Server.MapPath("\\App_Data\\links.json");
        //    JObject jsonObject = new JObject();

        //    JArray jLinksArray = new JArray(Links.Select(p => new JObject
        //       {
        //           { "source", p.SourceNode.Name },
        //           { "target", p.TargetNode.Name }
        //       })
        //    );
        //    jsonObject["links"] = jLinksArray;

        //    // serialize JSON directly to a file
        //    using (StreamWriter file = File.CreateText(path))
        //    {
        //        JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings();
        //        jsonSerializerSettings.Formatting = Formatting.Indented;
        //        JsonSerializer serializer = new JsonSerializer();
        //        serializer.Serialize(file, jsonObject);
        //    }
        //}

        public static void InitDbWithJson()
        {
            var path = System.Web.HttpContext.Current.Server.MapPath("\\App_Data\\nodes.json");
            JObject jsonObject = JObject.Parse(System.IO.File.ReadAllText(path));

            // read JSON directly from a file
            //using (StreamReader file = System.IO.File.OpenText(path))
            //using (JsonTextReader reader = new JsonTextReader(file))
            //{
            //    JObject jobject = (JObject)JToken.ReadFrom(reader);
            //}


            JArray nodesArray = (JArray)jsonObject["nodes"];
            IList<Node> NodesFromJson = nodesArray.Select(p => new Node
            {
                NodeID = (int)p["id"],
                Name = (string)p["name"],
                InLinks = new Collection<Link>(),
                OutLinks = new Collection<Link>()
            }).ToList<Node>();

            Nodes = new Collection<Node>(NodesFromJson);

            JArray linksArray = (JArray)jsonObject["links"];

            IList<Link> LinksFromJson = linksArray.Select(p => new Link
            {
                SourceNode = Nodes[(int)p["source"]],
                TargetNode = Nodes[(int)p["target"]]
            }).ToList<Link>();

            foreach (Link link in LinksFromJson)
            {
                InsertLink(link);
            }
        }    
    }    
}