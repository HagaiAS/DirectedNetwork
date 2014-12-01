using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NetworkProject.Models
{
    /// <summary>
    /// Node class
    /// </summary>
    public class Node 
    {
        [Display(Name = "Node Id")]
        [JsonProperty("id")]
        public int NodeID { get; set; }

        [Required]
        [JsonProperty("name")]
        [Display(Name = "Node Name")]
        public string Name { get; set; }        

        [Display(Name = "Nodes Inside Links")]
        [JsonIgnore]
        public virtual ICollection<Link> InLinks { get; set; }

        [Display(Name = "Nodes Outside Links")]
        [JsonIgnore]
        public virtual ICollection<Link> OutLinks { get; set; }

    }
}