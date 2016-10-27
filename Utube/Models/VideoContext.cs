﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Utube.Models
{
    public class VideoContext:DbContext
    {
        public DbSet<Profile> Profiles { set; get; }
        public DbSet<Video> Videos { set; get; }
        public DbSet<Comment> Comments { set; get; }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)          
        {          
            base.OnModelCreating(modelBuilder);
        }


    }
}