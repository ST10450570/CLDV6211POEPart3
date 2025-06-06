using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EventEase.Data;
using EventEase.Models;
using EventEase.ViewModels;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace EventEase.Controllers
{
    public class HomeController : Controller
    {
        private readonly EventEaseDBContext _context;

        public HomeController(EventEaseDBContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                // Get future events with venue and event type information
                var futureEvents = await _context.Events  // Fixed from _contextEvents to _context.Events
                    .Include(e => e.Venue)
                    .Include(e => e.EventType)
                    .Where(e => e.EventDate >= DateTime.Today)  // Fixed from DateTime Today
                    .OrderBy(e => e.EventDate)
                    .Take(5)  // Fixed from Take(E) to Take(5)
                    .ToListAsync();

                // Get random featured venues with availability check
                var featuredVenues = await _context.Venues
                    .Where(v => v.Availability)  // Fixed incomplete condition
                    .OrderBy(v => Guid.NewGuid())  // Fixed from Gc to Guid.NewGuid()
                    .Take(5)  // Fixed from Take(S) to Take(5)
                    .ToListAsync();

                var viewModel = new HomeViewModel
                {
                    FutureEvents = futureEvents,
                    FeaturedVenues = featuredVenues
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                // Log the error (you should implement proper logging)
                Console.WriteLine($"Error in HomeController: {ex.Message}");
                return View("Error", new ErrorViewModel { RequestId = HttpContext.TraceIdentifier });
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel
            {
                RequestId = System.Diagnostics.Activity.Current?.Id ?? HttpContext.TraceIdentifier
            });
        }
    }
}