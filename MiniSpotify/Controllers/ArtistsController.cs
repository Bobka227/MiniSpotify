using MiniSpotify.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

public class ArtistsController : Controller
{
    private readonly AppDbContext _db;
    public ArtistsController(AppDbContext db) => _db = db;

    public async Task<IActionResult> Index()
        => View(await _db.Artists.OrderBy(a => a.Name).ToListAsync());

    [HttpGet]
    public IActionResult Create() => View();


    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Artist model)
    {
        if (!ModelState.IsValid) return View(model);

        model.CreatedAt = DateTime.UtcNow;
        model.UpdatedAt = DateTime.UtcNow;

        _db.Artists.Add(model);
        await _db.SaveChangesAsync();
        TempData["msg"] = "Artist created";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var a = await _db.Artists.FindAsync(id);
        if (a == null) return NotFound();
        return View(a);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Artist model)
    {
        if (id != model.Id) return BadRequest();
        if (!ModelState.IsValid) return View(model);

        var a = await _db.Artists.FindAsync(id);
        if (a == null) return NotFound();

        a.Name = model.Name;
        a.Country = model.Country;
        a.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();

        TempData["msg"] = "Artist saved";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var a = await _db.Artists.FindAsync(id);
        if (a != null)
        {
            _db.Artists.Remove(a);
            await _db.SaveChangesAsync();
            TempData["msg"] = "Artist deleted";
        }
        return RedirectToAction(nameof(Index));
    }
}
