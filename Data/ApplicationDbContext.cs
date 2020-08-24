using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Duckify.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Duckify.Data {
    public class ApplicationDbContext : IdentityDbContext<User> {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        protected override void OnModelCreating(ModelBuilder builder) {
            builder.Entity<AppSettings>(entity => {
                entity.HasNoKey();
            });

            builder.Entity<UserStarredSongs>(entity => {
                entity.HasOne(pt => pt.User)
                    .WithMany(pt => pt.FavoriteSongs)
                    .HasForeignKey(pt => pt.SongId);
            });
            base.OnModelCreating(builder);
        }

        public DbSet<Song> Songs { get; set; }
        private DbSet<AppSettings> AppSettings { get; set; }
        public AppSettings Settings => AppSettings.FirstOrDefault();
    }
}
