using MiniSpotify.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

public class TracksController : Controller
{
    private readonly AppDbContext _db;
    private readonly IWebHostEnvironment _env;
    public TracksController(AppDbContext db, IWebHostEnvironment env) { _db = db; _env = env; }

    public async Task<IActionResult> Index()
        => View(await _db.Tracks.Include(t => t.Album)!.ThenInclude(a => a.Artist)
               .OrderBy(t => t.Title).ToListAsync());


    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var albums = await _db.Albums
            .Include(a => a.Artist)
            .OrderBy(a => a.Title)
            .Select(a => new
            {
                a.Id,
                Title = a.Title + (a.Artist != null ? $" — {a.Artist.Name}" : "")
            })
            .ToListAsync();

        ViewBag.Albums = new SelectList(albums, "Id", "Title");
        return View(new Track());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Track model, IFormFile? audioFile)
    {
        if (!ModelState.IsValid)
        {
            var albums = await _db.Albums
                .Include(a => a.Artist)
                .OrderBy(a => a.Title)
                .Select(a => new
                {
                    a.Id,
                    Title = a.Title + (a.Artist != null ? $" — {a.Artist.Name}" : "")
                })
                .ToListAsync();
            ViewBag.Albums = new SelectList(albums, "Id", "Title", model.AlbumId);
            return View(model);
        }

        model.CreatedAt = DateTime.UtcNow;
        model.UpdatedAt = DateTime.UtcNow;

        if (audioFile is not null && audioFile.Length > 0)
        {
            var uploads = Path.Combine(_env.WebRootPath, "music");
            Directory.CreateDirectory(uploads);

            var ext = Path.GetExtension(audioFile.FileName);
            var fname = $"{Guid.NewGuid()}{ext}";
            var full = Path.Combine(uploads, fname);

            using var fs = System.IO.File.Create(full);
            await audioFile.CopyToAsync(fs);

            model.FilePath = $"/music/{fname}";
        }

        _db.Tracks.Add(model);
        await _db.SaveChangesAsync();

        TempData["msg"] = "Track created";
        return RedirectToAction(nameof(Index));
    }
    public async Task<IActionResult> Edit(int id)
    {
        var t = await _db.Tracks.Include(t => t.Album).FirstOrDefaultAsync(t => t.Id == id);
        if (t == null) return NotFound();
        ViewBag.Albums = new SelectList(await _db.Albums.Include(a => a.Artist)
                                 .OrderBy(a => a.Title).ToListAsync(), "Id", "Title", t.AlbumId);
        return View(t);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Track model, IFormFile? audioFile)
    {
        if (id != model.Id) return BadRequest();
        if (!ModelState.IsValid)
        {
            ViewBag.Albums = new SelectList(_db.Albums, "Id", "Title", model.AlbumId);
            return View(model);
        }

        var t = await _db.Tracks.FindAsync(id);
        if (t == null) return NotFound();

        t.Title = model.Title;
        t.AlbumId = model.AlbumId;
        t.DurationSec = model.DurationSec;
        t.UpdatedAt = DateTime.UtcNow;

        if (audioFile != null && audioFile.Length > 0)
        {
            var uploads = Path.Combine(_env.WebRootPath, "music");
            Directory.CreateDirectory(uploads);
            var fname = $"{Guid.NewGuid()}{Path.GetExtension(audioFile.FileName)}";
            var full = Path.Combine(uploads, fname);
            using var fs = System.IO.File.Create(full);
            await audioFile.CopyToAsync(fs);
            t.FilePath = $"/music/{fname}";
        }

        await _db.SaveChangesAsync();
        TempData["msg"] = "Track saved";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var t = await _db.Tracks.FindAsync(id);
        if (t != null)
        {
            _db.Tracks.Remove(t);
            await _db.SaveChangesAsync();
            TempData["msg"] = "Track deleted";
        }
        return RedirectToAction(nameof(Index));
    }
    public async Task<IActionResult> Details(int id)
    {
        var track = await _db.Tracks
            .Include(t => t.Album)
            .ThenInclude(a => a.Artist)
            .FirstOrDefaultAsync(t => t.Id == id);

        if (track == null) return NotFound();
        return View(track);
    }
}
