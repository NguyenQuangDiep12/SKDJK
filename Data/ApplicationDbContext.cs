using Microsoft.EntityFrameworkCore;
using SKDJK.Models;
using SKDJK.Models.enums;

namespace SKDJK.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
        public DbSet<User> Users { get; set; }
        public DbSet<UserAnswer> UserAnswers { get; set; }
        public DbSet<Answer> Answers { get; set; }
        public DbSet<Language> Languages { get; set; }
        public DbSet<LearningProgress> LearningProgress { get; set; }
        public DbSet<PronunciationResult> PronunciationResults { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Test> Tests { get; set; }
        public DbSet<TestResult> TestResults { get; set; }
        public DbSet<Topic> Topics { get; set; }
        public DbSet<Vocabulary> Vocabularies { get; set; }
        public DbSet<Lesson> Lessons { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
            base.OnModelCreating(modelBuilder);


            modelBuilder.Entity<Role>().HasData(
                new Role()
                {
                    Id = 1,
                    RoleName = UserRole.ADMIN,

                },
                new Role()
                {
                    Id = 2,
                    RoleName = UserRole.USER,
                }
            );

            modelBuilder.Entity<User>().HasData(
                new User()
                {
                    Id = 1,
                    FullName = "Nguyen Van Bach",
                    Email = "Bach1994@gmail.com",
                    // Bach123
                    PasswordHash = "$2a$11$6yy1n7ZOuNvwVXiN78nTleFamsf5fpkodEtxod38CaHo9JRnpmQ8q",
                    RoleId = 1
                },
                new User()
                {
                    Id = 2,
                    FullName = "Nguyen Quang Diep",
                    Email = "nguyenquangdiepnx1@gmail.com",
                    PasswordHash = "$2a$11$1S.ZcePBd3lvAcCg..i1beVY/fhIQD9QX4POW1zY.Xk1AhxT53RwW",
                    RoleId = 2
                }
            );

            DemoData.Configure(modelBuilder);

        }
    }
}
