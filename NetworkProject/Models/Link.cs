using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace NetworkProject.Models
{
    /// <summary>
    /// Link class
    /// </summary>
    public class Link
    {
        [Display(Name = "Node Target")]
        [JsonProperty("target")]
        //[JsonProperty(IsReference = true)]
        public virtual Node TargetNode { get; set; }


        [Display(Name = "Node Source")]
        [JsonProperty("source")]
        //[JsonProperty(IsReference = true)]
        public virtual Node SourceNode { get; set; }

    }
}