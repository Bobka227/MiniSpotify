using System;
using System.Collections.Generic;

namespace MiniSpotify.Models
{
    public class Artist
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string? Country { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public ICollection<Album> Albums { get; set; } = new List<Album>();
    }
}
