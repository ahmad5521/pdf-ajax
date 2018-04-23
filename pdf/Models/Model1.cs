namespace pdf.Models
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class Model1 : DbContext
    {
        public Model1()
            : base("name=Data")
        {
        }

        public virtual DbSet<FileDetail> FileDetails { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<FileDetail>()
                .Property(e => e.FileName)
                .IsUnicode(false);
        }
    }
}
