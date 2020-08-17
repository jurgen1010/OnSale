using Microsoft.EntityFrameworkCore;
using OnSale.Common.Entities;

namespace OnSale.Web.Data
{
    public class DataContext : DbContext //hereda de DbContext 
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)//escribiendo la palabra ctor + tab, tab puedo crear un constructor de manera rapida
        {
        }

        public DbSet<Country> Countries { get; set; }//Mapeo el modelo con DbSet<>, e indico dentro <> a cual entity deseo mapear

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Con este bloque de Codigo indico que el atributo Name de la entidad Country en unico
            modelBuilder.Entity<Country>()
            .HasIndex(t => t.Name)
            .IsUnique();

        }

    }

}
