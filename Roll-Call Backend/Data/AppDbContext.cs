using Microsoft.EntityFrameworkCore;
using RollCallBackend.Models;

namespace RollCallBackend.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<SavedCharacter> SavedCharacters => Set<SavedCharacter>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<SavedCharacter>(entity =>
        {
            entity.HasKey(character => character.Id);

            entity.Property(character => character.Name)
                .HasMaxLength(200)
                .IsRequired();

            entity.Property(character => character.Ancestry)
                .HasMaxLength(100);

            entity.Property(character => character.CharacterClass)
                .HasMaxLength(100);

            entity.Property(character => character.Data)
                .IsRequired();
        });
    }
}
