using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiniSpotify.Models;

public class SearchController : Controller
{
    private readonly AppDbContext _db;
    public SearchController(AppDbContext db) => _db = db;

    public async Task<IActionResult> Index(string q)
    {
        q = (q ?? "").Trim();
        var vm = new SearchVm { Query = q };

        if (!string.IsNullOrEmpty(q))
        {
            vm.Artists = await _db.Artists
                .Where(a => EF.Functions.Like(a.Name, $"%{q}%"))
                .Take(20).ToListAsync();

            vm.Albums = await _db.Albums
                .Include(a => a.Artist)
                .Where(a => EF.Functions.Like(a.Title, $"%{q}%"))
                .Take(20).ToListAsync();

            vm.Tracks = await _db.Tracks
                .Include(t => t.Album).ThenInclude(a => a.Artist)
                .Where(t => EF.Functions.Like(t.Title, $"%{q}%"))
                .Take(30).ToListAsync();
        }

        return View(vm);
    }
}

public class SearchVm
{
    public string Query { get; set; } = "";
    public List<Artist> Artists { get; set; } = new();
    public List<Album> Albums { get; set; } = new();
    public List<Track> Tracks { get; set; } = new();
}
