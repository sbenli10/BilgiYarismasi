using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BilgiYarismasiMVC.Data;
using BilgiYarismasiMVC.Models;
using System.Threading.Tasks;

namespace BilgiYarismasiMVC.Controllers
{
    public class QuestionController : Controller
    {
        private readonly ApplicationDbContext _context;

        public QuestionController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Listeleme
        public async Task<IActionResult> Index()
        {
            return View(await _context.Questions.ToListAsync());
        }

        // Detay
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var question = await _context.Questions.FindAsync(id);
            if (question == null) return NotFound();
            return View(question);
        }

        // Ekleme GET
        public IActionResult Create()
        {
            return View();
        }

        // Ekleme POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Question question)
        {
            if (ModelState.IsValid)
            {
                _context.Add(question);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(question);
        }

        // Düzenleme GET
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var question = await _context.Questions.FindAsync(id);
            if (question == null) return NotFound();
            return View(question);
        }

        // Düzenleme POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Question question)
        {
            if (id != question.Id) return NotFound();

            if (ModelState.IsValid)
            {
                _context.Update(question);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(question);
        }

        // Silme GET
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var question = await _context.Questions.FindAsync(id);
            if (question == null) return NotFound();
            return View(question);
        }

        // Silme POST
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var question = await _context.Questions.FindAsync(id);
            _context.Questions.Remove(question);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
