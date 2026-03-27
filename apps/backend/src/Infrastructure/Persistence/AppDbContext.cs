using LexiFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LexiFlow.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Word> Words => Set<Word>();
    public DbSet<WordSample> WordSamples => Set<WordSample>();
    public DbSet<UserWordProgress> UserWordProgresses => Set<UserWordProgress>();
    public DbSet<ReviewHistory> ReviewHistories => Set<ReviewHistory>();
    public DbSet<GeneratedStory> GeneratedStories => Set<GeneratedStory>();
    public DbSet<WordleGame> WordleGames => Set<WordleGame>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // User Configuration
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.FirebaseUid).IsUnique();
            entity.HasIndex(e => e.Email).IsUnique();
            entity.Property(e => e.FirebaseUid).IsRequired().HasMaxLength(128);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(256);
            entity.Property(e => e.DisplayName).IsRequired().HasMaxLength(100);
        });

        // Category Configuration
        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(500);
        });

        // Word Configuration
        modelBuilder.Entity<Word>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.EnglishWord).IsRequired().HasMaxLength(100);
            entity.Property(e => e.TurkishTranslation).IsRequired().HasMaxLength(200);
            entity.HasOne(e => e.User)
                  .WithMany(u => u.Words)
                  .HasForeignKey(e => e.UserId)
                  .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.Category)
                  .WithMany(c => c.Words)
                  .HasForeignKey(e => e.CategoryId)
                  .OnDelete(DeleteBehavior.SetNull);
        });

        // WordSample Configuration
        modelBuilder.Entity<WordSample>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.SentenceText).IsRequired().HasMaxLength(500);
            entity.Property(e => e.TurkishTranslation).HasMaxLength(500);
            entity.HasOne(e => e.Word)
                  .WithMany(w => w.Samples)
                  .HasForeignKey(e => e.WordId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // UserWordProgress Configuration
        modelBuilder.Entity<UserWordProgress>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.UserId, e.WordId }).IsUnique();
            entity.HasOne(e => e.User)
                  .WithMany(u => u.WordProgresses)
                  .HasForeignKey(e => e.UserId)
                  .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.Word)
                  .WithMany(w => w.Progresses)
                  .HasForeignKey(e => e.WordId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // ReviewHistory Configuration
        modelBuilder.Entity<ReviewHistory>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.User)
                  .WithMany(u => u.ReviewHistories)
                  .HasForeignKey(e => e.UserId)
                  .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.Word)
                  .WithMany()
                  .HasForeignKey(e => e.WordId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // GeneratedStory Configuration
        modelBuilder.Entity<GeneratedStory>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.StoryText).IsRequired();
            entity.Property(e => e.WordsUsedJson).IsRequired();
            entity.HasOne(e => e.User)
                  .WithMany(u => u.GeneratedStories)
                  .HasForeignKey(e => e.UserId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // WordleGame Configuration
        modelBuilder.Entity<WordleGame>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.TargetWord).IsRequired().HasMaxLength(5);
            entity.Property(e => e.GuessesJson).IsRequired();
            entity.HasOne(e => e.User)
                  .WithMany(u => u.WordleGames)
                  .HasForeignKey(e => e.UserId)
                  .OnDelete(DeleteBehavior.Cascade);
        });
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
    {
        // Enforce UTC dates for UpdatedAtUtc if any entity is modified but not tracked in Domain logic
        // This is a safety net; Domain should ideally handle all business logic states.
        foreach (var entry in ChangeTracker.Entries<BaseEntity>())
        {
            if (entry.State == EntityState.Added)
            {
                // Let Domain handle CreatedAtUtc
            }
            else if (entry.State == EntityState.Modified)
            {
                // UpdatedAtUtc handled via MarkUpdated in domain, but in case they bypass:
                entry.Property(x => x.UpdatedAtUtc).CurrentValue = DateTime.UtcNow;
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }
}
