using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Utube.Models
{
    public class Video
    {

        public string Vkey = "AIzaSyCrnoGrX9vk_WI3Xi2b16xEoawpcJ5q7RI";
        public int Id { set; get; }
        public string Videotitle { set; get; }
        public string Vlink { get; set; }
        public bool Shared { get; set; }
        public DateTime Datein { set; get; }
        public int Like { set; get; }
        public int Dislike { set; get; }

        public int Profileid { set; get; }
        public int Views { get; set; }
        public string Info { set; get; }

    }
}