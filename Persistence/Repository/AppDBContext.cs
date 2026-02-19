using Microsoft.EntityFrameworkCore;
using Registration.Persistence.entity;

namespace Registration.Persistence.Repository
{
    public class AppDBContext : DbContext
    {
        public AppDBContext(DbContextOptions<AppDBContext> options) : base(options) { }

        public DbSet<RegistrationUser> registrations { get; set; }
        public DbSet<UserFile> userFiles { get; set; }
        public DbSet<RoleEntity> Roles { get; set; }
        public DbSet<EmailVerification> emailVerification { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

        }

    }
}