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
                var futureEvents = await _context.Events
                    .Include(e => e.Venue)
                    .Include(e => e.EventType)
                    .Where(e => e.EventDate >= DateTime.Today)
                    .OrderBy(e => e.EventDate)
                    .Take(5)
                    .ToListAsync();

                var featuredVenues = await _context.Venues
                    .Where(v => v.Availability)
                    .OrderBy(v => Guid.NewGuid())
                    .Take(5)
                    .ToListAsync();

                var recentBookings = await _context.Bookings
                    .Include(b => b.Event)
                    .Include(b => b.Venue)
                    .OrderByDescending(b => b.BookingDate)
                    .Take(5)
                    .ToListAsync();

                var viewModel = new HomeViewModel
                {
                    FutureEvents = futureEvents,
                    FeaturedVenues = featuredVenues,
                    RecentBookings = recentBookings
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in HomeController: {ex.Message}");
                return View("Error");
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