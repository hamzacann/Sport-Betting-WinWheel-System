﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace katarfelek.Models
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class katarfelekEntities : DbContext
    {
        public katarfelekEntities()
            : base("name=katarfelekEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<generalSettings> generalSettings { get; set; }
        public virtual DbSet<PanelSettings> PanelSettings { get; set; }
        public virtual DbSet<panelUsers> panelUsers { get; set; }
        public virtual DbSet<requests> requests { get; set; }
    }
}