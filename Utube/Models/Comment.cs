using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Utube.Models
{
    public class Comment
    {
        public int Id { set; get; }
        public string Content { set; get; }
        public DateTime DateIn { set; get; }
        public int VideoId { set; get; }
        public Guid UserId { set; get; }


    }
}