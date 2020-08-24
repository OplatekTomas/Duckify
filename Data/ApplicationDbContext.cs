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

            builder.Entity<UserStarredSongs>().HasKey(x => new {x.SongId, x.UserId});
            
            builder.Entity<UserStarredSongs>(entity => {
                entity.HasOne(x => x.User)
                    .WithMany(x => x.FavoriteSongs)
                    .HasForeignKey(x => x.SongId);
            });
            builder.Entity<UserStarredSongs>(entity => {
                entity.HasOne(x => x.Song)
                    .WithMany(x => x.FavoriteBy)
                    .HasForeignKey(x => x.UserId);
            });
            base.OnModelCreating(builder);
        }

        public DbSet<Song> Songs { get; set; }
        private DbSet<AppSettings> AppSettings { get; set; }
        public AppSettings Settings => AppSettings.FirstOrDefault();
    }
}
