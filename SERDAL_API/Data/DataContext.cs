using Microsoft.EntityFrameworkCore;
using SERDAL_API.Application.DTOs.UserDto;
using SERDAL_API.Application.Models.Datasets;
using SERDAL_API.Application.Models.Publication;
using SERDAL_API.Application.Models.Survey;
using SERDAL_API.Application.Models.User;

namespace SERDAL_API.Data
{
    public class DataContext: DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) 
        { 
        
        }

        //User
        public DbSet<User> users { get; set; }
        public DbSet<OTPs> otp { get; set; }

        //Publication
        public DbSet<Publication> publication { get; set; }
        public DbSet<University> university { get; set; }

        //Survey
        public DbSet<Survey> survey { get; set; }
        public DbSet<Field> fields { get; set; }


        //DataSet
        public DbSet<ChartDataset> chartDatasets { get; set; }
        public DbSet<Commodity> commodity { get; set; }
        public DbSet<Variable> variables { get; set; }
        public DbSet<VariableValue> variablesValue { get; set; }
        public DbSet<DataGroup> dataGroups { get; set; }



        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    modelBuilder.Entity<PublicationModel>()
        //        .Property(p => p.Summary)
        //        .HasColumnType("LONGTEXT");  // Explicitly set LONGTEXT for Summary
        //}


        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    if (!optionsBuilder.IsConfigured)
        //    {
        //        // Replace with your actual connection string
        //        optionsBuilder.UseSqlServer("Data Source=C55GB33\\SQLEXPRESS;Initial Catalog=DB_TEST;Integrated Security=true;TrustServerCertificate=True;");
        //    }
        //}

        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    base.OnModelCreating(modelBuilder);  // Ensure base configuration is applied

        //    // Force the CreateDateTime and ModifiedDateTime columns to use 'datetime' instead of 'datetime2'
        //    modelBuilder.Entity<UsersModel>(entity =>
        //    {
        //        entity.Property(e => e.CreateDateTime)
        //              .HasColumnType("datetime"); // Specify the 'datetime' SQL type for CreateDateTime

        //        entity.Property(e => e.ModifiedDateTime)
        //              .HasColumnType("datetime"); // Specify the 'datetime' SQL type for ModifiedDateTime
        //    });
        //}


    }
}
