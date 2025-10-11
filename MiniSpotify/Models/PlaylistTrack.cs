using System;

namespace MiniSpotify.Models
{
    public class PlaylistTrack
    {
        public int PlaylistId { get; set; }
        public int TrackId { get; set; }
        public int Position { get; set; }
        public DateTime AddedAt { get; set; }

        public Playlist? Playlist { get; set; }
        public Track? Track { get; set; }
    }
}
