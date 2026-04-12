using BlindMatchPAS.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BlindMatchPAS.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<Project> Projects => Set<Project>();
        public DbSet<ResearchArea> ResearchAreas => Set<ResearchArea>();
        public DbSet<SupervisorExpertise> SupervisorExpertises => Set<SupervisorExpertise>();
        public DbSet<Match> Matches => Set<Match>();

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Project>()
                .HasOne(p => p.Student)
                .WithMany(u => u.SubmittedProjects)
                .HasForeignKey(p => p.StudentId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Match>()
                .HasOne(m => m.Project)
                .WithOne(p => p.Match)
                .HasForeignKey<Match>(m => m.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Match>()
                .HasOne(m => m.Supervisor)
                .WithMany(u => u.SupervisedMatches)
                .HasForeignKey(m => m.SupervisorId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<SupervisorExpertise>()
                .HasIndex(se => new { se.SupervisorId, se.ResearchAreaId })
                .IsUnique();

            builder.Entity<Project>()
                .HasIndex(p => p.Status);


            // Seed Roles
            builder.Entity<IdentityRole>().HasData(
                new IdentityRole { Id = "role-student",      Name = "Student",      NormalizedName = "STUDENT" },
                new IdentityRole { Id = "role-supervisor",   Name = "Supervisor",   NormalizedName = "SUPERVISOR" },
                new IdentityRole { Id = "role-moduleleader", Name = "ModuleLeader", NormalizedName = "MODULELEADER" },
                new IdentityRole { Id = "role-admin",        Name = "Admin",        NormalizedName = "ADMIN" }
            );

            // Seed Research Areas
            builder.Entity<ResearchArea>().HasData(
                new ResearchArea { Id = 1,  Name = "Artificial Intelligence",         Description = "Machine learning, deep learning, NLP, and AI-driven systems.",             IsActive = true, CreatedAt = new DateTime(2026, 1, 1) },
                new ResearchArea { Id = 2,  Name = "Web Development",                 Description = "Frontend, backend, and full-stack web technologies.",                       IsActive = true, CreatedAt = new DateTime(2026, 1, 1) },
                new ResearchArea { Id = 3,  Name = "Cybersecurity",                   Description = "Network security, ethical hacking, and cryptography.",                      IsActive = true, CreatedAt = new DateTime(2026, 1, 1) },
                new ResearchArea { Id = 4,  Name = "Cloud Computing",                 Description = "Distributed systems, containerisation, and cloud platforms.",               IsActive = true, CreatedAt = new DateTime(2026, 1, 1) },
                new ResearchArea { Id = 5,  Name = "Mobile Development",              Description = "iOS, Android, and cross-platform mobile applications.",                     IsActive = true, CreatedAt = new DateTime(2026, 1, 1) },
                new ResearchArea { Id = 6,  Name = "Data Science",                    Description = "Data analysis, visualisation, and statistical modelling.",                  IsActive = true, CreatedAt = new DateTime(2026, 1, 1) },
                new ResearchArea { Id = 7,  Name = "Internet of Things",              Description = "Embedded systems, sensor networks, and IoT platforms.",                     IsActive = true, CreatedAt = new DateTime(2026, 1, 1) },
                new ResearchArea { Id = 8,  Name = "Software Engineering",            Description = "SDLC, agile methodologies, and software architecture.",                     IsActive = true, CreatedAt = new DateTime(2026, 1, 1) },
                new ResearchArea { Id = 9,  Name = "Blockchain & Web3",               Description = "Distributed ledgers, smart contracts, and decentralised applications.",    IsActive = true, CreatedAt = new DateTime(2026, 1, 1) },
                new ResearchArea { Id = 10, Name = "Computer Vision",                 Description = "Image recognition, object detection, and visual AI systems.",              IsActive = true, CreatedAt = new DateTime(2026, 1, 1) },
                new ResearchArea { Id = 11, Name = "Natural Language Processing",     Description = "Text analysis, language models, and conversational AI.",                   IsActive = true, CreatedAt = new DateTime(2026, 1, 1) },
                new ResearchArea { Id = 12, Name = "DevOps & Automation",             Description = "CI/CD pipelines, infrastructure as code, and deployment automation.",      IsActive = true, CreatedAt = new DateTime(2026, 1, 1) },
                new ResearchArea { Id = 13, Name = "Human-Computer Interaction",      Description = "UX research, accessibility, and user-centred design.",                     IsActive = true, CreatedAt = new DateTime(2026, 1, 1) },
                new ResearchArea { Id = 14, Name = "Database Systems",                Description = "Relational and NoSQL databases, query optimisation, and data modelling.",  IsActive = true, CreatedAt = new DateTime(2026, 1, 1) },
                new ResearchArea { Id = 15, Name = "Game Development",                Description = "Game engines, graphics programming, and interactive media.",               IsActive = true, CreatedAt = new DateTime(2026, 1, 1) },
                new ResearchArea { Id = 16, Name = "Augmented & Virtual Reality",     Description = "AR/VR systems, spatial computing, and immersive experiences.",             IsActive = true, CreatedAt = new DateTime(2026, 1, 1) },
                new ResearchArea { Id = 17, Name = "Robotics & Embedded Systems",     Description = "Microcontrollers, autonomous systems, and hardware-software integration.", IsActive = true, CreatedAt = new DateTime(2026, 1, 1) },
                new ResearchArea { Id = 18, Name = "Health Informatics",              Description = "Digital health systems, medical data, and clinical decision support.",     IsActive = true, CreatedAt = new DateTime(2026, 1, 1) },
                new ResearchArea { Id = 19, Name = "E-Commerce & FinTech",            Description = "Online payment systems, digital banking, and financial technology.",       IsActive = true, CreatedAt = new DateTime(2026, 1, 1) },
                new ResearchArea { Id = 20, Name = "Network Engineering",             Description = "Network protocols, wireless systems, and infrastructure design.",          IsActive = true, CreatedAt = new DateTime(2026, 1, 1) }
            );
        }
    }
}