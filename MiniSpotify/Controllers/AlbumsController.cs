using MiniSpotify.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

public class AlbumsController : Controller
{
    private readonly AppDbContext _db;
    private readonly IWebHostEnvironment _env;
    public AlbumsController(AppDbContext db, IWebHostEnvironment env) { _db = db; _env = env; }

    public async Task<IActionResult> Index()
        => View(await _db.Albums.Include(a => a.Artist).OrderBy(a => a.Title).ToListAsync());

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        ViewBag.Artists = new SelectList(await _db.Artists.OrderBy(a => a.Name).ToListAsync(), "Id", "Name");
        return View();
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Album model, IFormFile? coverFile)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Artists = new SelectList(_db.Artists, "Id", "Name", model.ArtistId);
            return View(model);
        }

        if (coverFile != null && coverFile.Length > 0)
        {
            var uploads = Path.Combine(_env.WebRootPath, "covers");
            Directory.CreateDirectory(uploads);
            var fname = $"{Guid.NewGuid()}{Path.GetExtension(coverFile.FileName)}";
            var full = Path.Combine(uploads, fname);
            using var fs = System.IO.File.Create(full);
            await coverFile.CopyToAsync(fs);
            model.CoverPath = $"/covers/{fname}";
        }

        model.CreatedAt = DateTime.UtcNow;
        model.UpdatedAt = DateTime.UtcNow;

        _db.Albums.Add(model);
        await _db.SaveChangesAsync();
        TempData["msg"] = "Album created";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var al = await _db.Albums.FindAsync(id);
        if (al == null) return NotFound();
        ViewBag.Artists = new SelectList(await _db.Artists.OrderBy(a => a.Name).ToListAsync(), "Id", "Name", al.ArtistId);
        return View(al);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Album model, IFormFile? coverFile)
    {
        if (id != model.Id) return BadRequest();
        if (!ModelState.IsValid)
        {
            ViewBag.Artists = new SelectList(_db.Artists, "Id", "Name", model.ArtistId);
            return View(model);
        }

        var al = await _db.Albums.FindAsync(id);
        if (al == null) return NotFound();

        al.Title = model.Title;
        al.ArtistId = model.ArtistId;
        al.ReleaseDate = model.ReleaseDate;
        al.UpdatedAt = DateTime.UtcNow;

        if (coverFile != null && coverFile.Length > 0)
        {
            var uploads = Path.Combine(_env.WebRootPath, "covers");
            Directory.CreateDirectory(uploads);
            var fname = $"{Guid.NewGuid()}{Path.GetExtension(coverFile.FileName)}";
            var full = Path.Combine(uploads, fname);
            using var fs = System.IO.File.Create(full);
            await coverFile.CopyToAsync(fs);
            al.CoverPath = $"/covers/{fname}";
        }

        await _db.SaveChangesAsync();
        TempData["msg"] = "Album saved";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var al = await _db.Albums.FindAsync(id);
        if (al != null)
        {
            _db.Albums.Remove(al);
            await _db.SaveChangesAsync();
            TempData["msg"] = "Album deleted";
        }
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Details(int id)
    {
        var album = await _db.Albums
            .Include(a => a.Artist)
            .Include(a => a.Tracks)
            .FirstOrDefaultAsync(a => a.Id == id);

        if (album == null) return NotFound();
        return View(album);
    }
}
