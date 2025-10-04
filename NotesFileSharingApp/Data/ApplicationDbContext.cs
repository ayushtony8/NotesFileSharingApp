using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NotesFileSharingApp.Models;

namespace NotesFileSharingApp.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Note> Notes { get; set; }
        public DbSet<FileUpload> Files { get; set; }
        public DbSet<SharedNote> SharedNotes { get; set; }
        public DbSet<SharedFile> SharedFiles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Note entity
            modelBuilder.Entity<Note>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Content).IsRequired();
                entity.Property(e => e.UserId).IsRequired();
                
                entity.HasOne(e => e.User)
                    .WithMany(u => u.Notes)
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure FileUpload entity
            modelBuilder.Entity<FileUpload>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.FileName).IsRequired().HasMaxLength(255);
                entity.Property(e => e.FilePath).IsRequired().HasMaxLength(500);
                entity.Property(e => e.UserId).IsRequired();
                
                entity.HasOne(e => e.User)
                    .WithMany(u => u.Files)
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure SharedNote entity
            modelBuilder.Entity<SharedNote>(entity =>
            {
                entity.HasKey(e => e.Id);
                
                entity.HasOne(e => e.Note)
                    .WithMany(n => n.SharedNotes)
                    .HasForeignKey(e => e.NoteId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.SharedByUser)
                    .WithMany(u => u.SharedNotes)
                    .HasForeignKey(e => e.SharedByUserId)
                    .OnDelete(DeleteBehavior.NoAction);

                entity.HasOne(e => e.SharedWithUser)
                    .WithMany(u => u.ReceivedSharedNotes)
                    .HasForeignKey(e => e.SharedWithUserId)
                    .OnDelete(DeleteBehavior.NoAction);

                // Prevent duplicate sharing
                entity.HasIndex(e => new { e.NoteId, e.SharedWithUserId }).IsUnique();
            });

            // Configure SharedFile entity
            modelBuilder.Entity<SharedFile>(entity =>
            {
                entity.HasKey(e => e.Id);
                
                entity.HasOne(e => e.File)
                    .WithMany(f => f.SharedFiles)
                    .HasForeignKey(e => e.FileId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.SharedByUser)
                    .WithMany(u => u.SharedFiles)
                    .HasForeignKey(e => e.SharedByUserId)
                    .OnDelete(DeleteBehavior.NoAction);

                entity.HasOne(e => e.SharedWithUser)
                    .WithMany(u => u.ReceivedSharedFiles)
                    .HasForeignKey(e => e.SharedWithUserId)
                    .OnDelete(DeleteBehavior.NoAction);

                // Prevent duplicate sharing
                entity.HasIndex(e => new { e.FileId, e.SharedWithUserId }).IsUnique();
            });
        }
    }
}