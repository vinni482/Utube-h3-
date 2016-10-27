using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Utube.Models
{
    public class Profile
    {
        public int Id { set; get; }
        public string Name { set; get; }
        public Guid UserId { set; get; }
        public byte[] Picture { set; get; }


    }
}