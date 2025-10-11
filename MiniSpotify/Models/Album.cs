using System;
using System.Collections.Generic;

namespace MiniSpotify.Models
{
    public class Album
    {
        public int Id { get; set; }
        public int ArtistId { get; set; }
        public string Title { get; set; } = "";
        public DateTime? ReleaseDate { get; set; }
        public string? CoverPath { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public Artist? Artist { get; set; }
        public ICollection<Track> Tracks { get; set; } = new List<Track>();
    }
}
