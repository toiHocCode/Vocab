using LearnVocabulary.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LearnVocabulary.Data
{
    public class VocabularyContext : DbContext
    {
        public VocabularyContext(DbContextOptions<VocabularyContext> options) : base(options)
        {
        }

        public DbSet<Vocabulary> Vocabulary { get; set; }

        public DbSet<Definition> Definitions { get; set; }

        public DbSet<Usage> Usages { get; set; }

        public DbSet<VocabularyUsage> VocabularyUsages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Vocabulary>().ToTable("Vocabulary");
            modelBuilder.Entity<Definition>().ToTable("Definition");
            modelBuilder.Entity<Usage>().ToTable("Usage");
            modelBuilder.Entity<VocabularyUsage>().ToTable("VocabularyUsage");
            modelBuilder.Entity<VocabularyUsage>().HasKey(vu => new { vu.VocabularyId, vu.UsageId });
        }
    }
}
