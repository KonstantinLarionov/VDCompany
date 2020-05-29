﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VDCompany.Models.Interfaces;
using VDCompany.Models.Objects;

namespace VDCompany.Models.Entitys
{
    public class StartContext : DbContext
    {

        public DbSet<Admin> Admins { get; set; }
        public DbSet<Case> Cases { get; set; }
        public DbSet<Doc> Docs { get; set; }
        public DbSet<Dialog> Dialogs { get; set; }
        public DbSet<Lawyer> Lawyers { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Message> Messages { get; set; }
        public StartContext(DbContextOptions options) : base(options)
        {
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySql(Conf.ConnectDb);
            base.OnConfiguring(optionsBuilder);
        }
    }
}
