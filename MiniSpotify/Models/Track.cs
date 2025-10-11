using System;
using System.Collections.Generic;

namespace MiniSpotify.Models
{
    public class Track
    {
        public int Id { get; set; }
        public int AlbumId { get; set; }
        public string Title { get; set; } = "";
        public int? DurationSec { get; set; }
        public string? FilePath { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }   

        public Album? Album { get; set; }
        public ICollection<PlaylistTrack> PlaylistTracks { get; set; } = new List<PlaylistTrack>();
    }
}
