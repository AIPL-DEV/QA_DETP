using Microsoft.EntityFrameworkCore;
using DETP.model;
using Microsoft.AspNetCore.Identity;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using DETP.model.QaViolation;

namespace DETP.data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)

        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<User>()
                .HasMany(x => x.Roles)
                .WithMany(x => x.Users)
            .UsingEntity<UserRole>(
                    r => r.HasOne<Role>().WithMany().HasForeignKey(e => e.RoleId),
                    l => l.HasOne<User>().WithMany().HasForeignKey(e => e.UserId)
                );
        }

        public DbSet<Division> Divisions { get; set; }
        public DbSet<Department> Department { get; set; }
        public DbSet<QAObservation> QAObservations { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<QAFlow> QAFlows { get; set;}
        public DbSet<HeadDetp> HeadDetps { get; set;}
        public DbSet<BusinessHead> BusinessHeads { get; set;}
        public DbSet<QaAtt> QaAtts { get; set;}
        public DbSet<Assignee> Assignees { get; set;}
        public DbSet<SiteIncharge> SiteIncharges { get; set;}
        public DbSet<ProjectIncharge> ProjectIncharges { get; set;}
        public DbSet<DeptHod> DeptHods { get; set;}
        public DbSet<HodQaSha> HodQaSha { get; set; }
        public DbSet<QAOfficer> QAOfficers { get; set;}
        public DbSet<EicDetp> EicDetps { get; set;}
        public DbSet<UserRole> UserRoles { get; set;}
        public DbSet<Role> Roles { get; set;}
        public DbSet<ShaRequestImage> ShaRequestImages { get; set; }
        public DbSet<LoginLog> LoginLogs { get; set; }
        public DbSet<QaViolationCategory> QaViolationCategories { get; set; }
        public DbSet<QaViolationSubCategory> QaViolationSubCategories { get; set; }
        public DbSet<PenaltyDetail> PenaltyDetails { get; set; }
        public DbSet<QaViolation> QaViolations { get; set; }
        public DbSet<QaViolationFlow> QaViolationFlows { get; set; }
        public DbSet<QaViolationStep> QaViolationSteps { get; set; }
        public DbSet<QaViolationApproval> QaViolationApproval { get; set; }
        public DbSet<QaViolationCFOReview> QaViolationCFOReviews { get; set; }
        public DbSet<QaViolationHeadProcurement> QaViolationHeadProcurements { get; set; }
    }
}
