// Models/AppDbContext.cs
using Microsoft.EntityFrameworkCore;

namespace MiniSpotify.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Artist> Artists => Set<Artist>();
        public DbSet<Album> Albums => Set<Album>();
        public DbSet<Track> Tracks => Set<Track>();
        public DbSet<Playlist> Playlists => Set<Playlist>();
        public DbSet<PlaylistTrack> PlaylistTracks => Set<PlaylistTrack>();

        protected override void OnModelCreating(ModelBuilder b)
        {
            b.Entity<Artist>().ToTable("Artist");
            b.Entity<Album>().ToTable("Album");
            b.Entity<Track>().ToTable("Track");
            b.Entity<Playlist>().ToTable("Playlist");
            b.Entity<PlaylistTrack>().ToTable("PlaylistTrack");

            b.Entity<PlaylistTrack>().HasKey(x => new { x.PlaylistId, x.TrackId });
        }
    }
}
