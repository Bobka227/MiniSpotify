using MiniSpotify.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

public class PlaylistsController : Controller
{
    private readonly AppDbContext _db;
    public PlaylistsController(AppDbContext db) => _db = db;

    public async Task<IActionResult> Index()
        => View(await _db.Playlists.OrderBy(p => p.Name).ToListAsync());

    public async Task<IActionResult> Details(int id)
    {
        var playlist = await _db.Playlists.FindAsync(id);
        if (playlist == null) return NotFound();

        var tracks = await _db.PlaylistTracks
            .Where(pt => pt.PlaylistId == id)
            .OrderBy(pt => pt.Position)
            .Include(pt => pt.Track)!.ThenInclude(t => t.Album)!.ThenInclude(a => a.Artist)
            .Select(pt => pt.Track!)
            .ToListAsync();

        ViewBag.Playlist = playlist;
        ViewBag.Tracks = tracks;

        ViewBag.AllTracks = await _db.Tracks
            .Include(t => t.Album)!.ThenInclude(a => a.Artist)
            .OrderBy(t => t.Title)
            .ToListAsync();

        return View();
    }



    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddTrack(int playlistId, int trackId)
    {
        var exists = await _db.PlaylistTracks
            .AnyAsync(x => x.PlaylistId == playlistId && x.TrackId == trackId);

        if (exists)
        {
            TempData["msg"] = "Track is already in this playlist.";
            return RedirectToAction(nameof(Details), new { id = playlistId });
        }

        var maxPos = await _db.PlaylistTracks
            .Where(x => x.PlaylistId == playlistId)
            .Select(x => (int?)x.Position).MaxAsync() ?? 0;

        _db.PlaylistTracks.Add(new PlaylistTrack
        {
            PlaylistId = playlistId,
            TrackId = trackId,
            Position = maxPos + 1
        });

        await _db.SaveChangesAsync();
        TempData["msg"] = "Track added.";
        return RedirectToAction(nameof(Details), new { id = playlistId });
    }


    [HttpGet]
    public IActionResult Create() => View(new Playlist());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Name,Description")] Playlist model)
    {
        if (!ModelState.IsValid) return View(model);

        _db.Playlists.Add(model);
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Details), new { id = model.Id });
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var p = await _db.Playlists.FindAsync(id);
        if (p == null) return NotFound();
        return View(p);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Playlist model)
    {
        if (id != model.Id) return BadRequest();
        if (!ModelState.IsValid) return View(model);

        var p = await _db.Playlists.FindAsync(id);
        if (p == null) return NotFound();

        p.Name = model.Name;
        p.Description = model.Description;
        p.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        TempData["msg"] = "Playlist updated";
        return RedirectToAction(nameof(Details), new { id = p.Id });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var p = await _db.Playlists
            .Include(x => x.PlaylistTracks)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (p != null)
        {
            if (p.PlaylistTracks?.Any() == true)
                _db.PlaylistTracks.RemoveRange(p.PlaylistTracks);

            _db.Playlists.Remove(p);
            await _db.SaveChangesAsync();
            TempData["msg"] = "Playlist deleted";
        }
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> RemoveTrack(int playlistId, int trackId)
    {
        var row = await _db.PlaylistTracks
            .FirstOrDefaultAsync(x => x.PlaylistId == playlistId && x.TrackId == trackId);
        if (row != null)
        {
            _db.PlaylistTracks.Remove(row);
            await _db.SaveChangesAsync();
            TempData["msg"] = "Track removed";
        }
        return RedirectToAction(nameof(Details), new { id = playlistId });
    }
}
