using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OnSale.Common.Entities;
using OnSale.Web.Data.Entities;

namespace OnSale.Web.Data
{
    public class DataContext : IdentityDbContext<User>
    //hereda de DbContext 
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)//escribiendo la palabra ctor + tab, tab puedo crear un constructor de manera rapida
        {
        }

        //Mapeo el modelo con DbSet<>, e indico dentro <> a cual entity deseo mapear, por cada entidad
        // es muy importante crearla en el DataContext
        public DbSet<Category> Categories { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Order> Orders { get; set; }// Incluimos nuestras tablas Orders y OrderDetails
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }
       



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            //Creamos indices para cada entidad con este bloque de Codigo indico que el atributo Name de la entidad Country en unico
            modelBuilder.Entity<Category>()
            .HasIndex(t => t.Name)
            .IsUnique();

            modelBuilder.Entity<Country>(cou =>
            {
                cou.HasIndex("Name").IsUnique();
                cou.HasMany(c => c.Departments).WithOne(d => d.Country).OnDelete(DeleteBehavior.Cascade);//Un pais tiene muchos departamentos con un pais, cuando se elimine un pais tenga un comportamiento en cascada
            });

            modelBuilder.Entity<Department>(dep =>
            {
                dep.HasIndex("Name", "CountryId").IsUnique();//Creamos un indice compuesto, me permitira validar departamentos unicos pero dentro un unico pais
                dep.HasOne(d => d.Country).WithMany(c => c.Departments).OnDelete(DeleteBehavior.Cascade);//Un departamento tiene un pais con muchos departamentos y el comportamiento de borrado sera en cascada
            });

            modelBuilder.Entity<City>(cit =>
            {
                cit.HasIndex("Name", "DepartmentId").IsUnique();//Creamos un indice compuesto, me permitira validar ciudades unicos pero dentro un unico departamento
                cit.HasOne(c => c.Department).WithMany(d => d.Cities).OnDelete(DeleteBehavior.Cascade);
            });


            modelBuilder.Entity<Product>()
            .HasIndex(t => t.Name)
            .IsUnique();
        }

    }

}
