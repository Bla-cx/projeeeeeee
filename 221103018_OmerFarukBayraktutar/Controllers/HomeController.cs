using System.Diagnostics;
using _221103018_OmerFarukBayraktutar.DAL;
using _221103018_OmerFarukBayraktutar.DAL.Models;
using _221103018_OmerFarukBayraktutar.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace _221103018_OmerFarukBayraktutar.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly EtkinliklerDbContext _context;

        public HomeController(ILogger<HomeController> logger, EtkinliklerDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            List<Etkinlik> populerEtkinlikler = new List<Etkinlik>();
            bool dbConnectionSuccessful = false;
            
            try
            {
                // Check if connection is available with timeout
                using (var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5)))
                {
                    dbConnectionSuccessful = _context.Database.CanConnect();
                }
                
                if (dbConnectionSuccessful)
                {
                    try 
                    {
                        populerEtkinlikler = await _context.Etkinlikler
                            .Include(e => e.Kategori)
                            .Include(e => e.Sehir)
                            .Where(e => e.AktifMi && e.Tarih > DateTime.Now)
                            .OrderBy(e => e.Tarih)
                            .Take(6)
                            .ToListAsync();
                            
                        return View(populerEtkinlikler);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Index sayfasında veritabanı sorgusu sırasında hata oluştu.");
                        ViewBag.DbError = "Veritabanı sorgusunda bir hata oluştu. Lütfen daha sonra tekrar deneyiniz.";
                        return View(populerEtkinlikler); // Return with empty list
                    }
                }
                else
                {
                    _logger.LogWarning("Veritabanına bağlanılamadı, yedek sayfa gösteriliyor.");
                    // Return backup view which doesn't require database
                    return View("Index.Backup", populerEtkinlikler);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Index sayfasını yüklerken kritik hata oluştu.");
                return View("Index.Backup", populerEtkinlikler);
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }
        
        public IActionResult Hakkimizda()
        {
            return View();
        }
        
        public IActionResult Iletisim()
        {
            return View();
        }
        
        public IActionResult Gizlilik()
        {
            return View("Privacy");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        
        // Custom error page for database connection issues
        public IActionResult DbError()
        {
            return View();
        }
    }
}
