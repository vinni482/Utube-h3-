using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Utube.Models
{
    public class Video
    {
        public int Id { set; get; }
        public int ProfileId { set; get; }
        public string VideoTitle { set; get; }
        public string Vlink { set; get; }
        public bool Shared { set; get; }
        public DateTime DateIn { set; get; }
        public int Like { set; get; }
        public int DisLike { set; get; }
        public int Views { set; get; }
        public string Info { set; get; }

    }
}