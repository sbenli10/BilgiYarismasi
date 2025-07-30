using BilgiYarismasiMVC.Data;
using BilgiYarismasiMVC.Models;
using Microsoft.AspNetCore.Mvc;
//using System.Data.Entity;

namespace BilgiYarismasiMVC.Controllers
{
    public class GameController : Controller
    {
        private readonly ApplicationDbContext _context;

        // Puan basamakları
        private static readonly List<int> prizeLevels = new List<int>
        {
            500, 1000, 2000, 3000, 5000,
            7500, 15000, 30000, 60000, 125000,
            250000, 500000, 1000000
        };

        public GameController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Start()
        {
            HttpContext.Session.SetInt32("Level", 0);
            HttpContext.Session.SetInt32("Earnings", 0);
            HttpContext.Session.SetString("FiftyUsed", "false");

            var questions = _context.Questions
                .OrderBy(q => Guid.NewGuid())
                .ToList();

            var orderedQuestions = questions
                .Where(q => q.Difficulty == "Kolay")
                .Take(5)
                .Concat(questions.Where(q => q.Difficulty == "Orta").Take(5))
                .Concat(questions.Where(q => q.Difficulty == "Zor").Take(3))
                .ToList();

            HttpContext.Session.SetString("QuestionList", System.Text.Json.JsonSerializer.Serialize(orderedQuestions));
            HttpContext.Session.SetInt32("QuestionIndex", 0);

            var currentQuestion = orderedQuestions.FirstOrDefault();

            // ✅ Bu satır çok önemli!
            if (currentQuestion != null)
            {
                HttpContext.Session.SetInt32("LastQuestionId", currentQuestion.Id);
            }

            return View("Start", currentQuestion);
        }



        [HttpPost]
        public IActionResult UseFiftyFifty()
        {
            HttpContext.Session.SetString("FiftyUsed", "true");

            var lastQuestionId = HttpContext.Session.GetInt32("LastQuestionId");
            var question = _context.Questions.Find(lastQuestionId);

            if (question == null) return RedirectToAction("GameOver");

            // Cevap dışındaki 2 yanlış şıkkı seç
            var allOptions = new Dictionary<string, string>
    {
        { "A", question.OptionA },
        { "B", question.OptionB },
        { "C", question.OptionC },
        { "D", question.OptionD }
    };

            var incorrectOptions = allOptions
                .Where(o => o.Key != question.CorrectOption)
                .OrderBy(_ => Guid.NewGuid())
                .Take(2)
                .Select(o => o.Key)
                .ToList();

            HttpContext.Session.SetString("HideOption1", incorrectOptions[0]);
            HttpContext.Session.SetString("HideOption2", incorrectOptions[1]);

            return View("Start", question);
        }

        [HttpPost]
        public IActionResult UseAudienceJoker(int id)
        {
            var question = _context.Questions.Find(id);
            if (question == null) return NotFound();

            var correct = question.CorrectOption;
            var random = new Random();

            // Doğru şık için yüksek yüzde, diğerleri için kalan
            int correctPercent = random.Next(50, 75);
            int remaining = 100 - correctPercent;

            var otherOptions = new List<string> { "A", "B", "C", "D" }.Where(x => x != correct).ToList();
            var otherPercents = new int[3];

            // Geriye kalan yüzdeyi 3 yanlış şıka dağıt
            for (int i = 0; i < 3; i++)
            {
                otherPercents[i] = i == 2 ? remaining - otherPercents.Take(2).Sum() : random.Next(0, remaining - otherPercents.Take(i).Sum());
            }

            // Hepsini tek yapıda topla
            var result = new Dictionary<string, int>
            {
                [correct] = correctPercent
            };

            for (int i = 0; i < 3; i++)
            {
                result[otherOptions[i]] = otherPercents[i];
            }

            // ViewBag ile şık-yüzde gönder
            ViewBag.AudiencePoll = result;
            ViewBag.AudienceUsed = true;

            return View("Start", question);
        }


        public IActionResult Ready()
        {
            // Kullanıcıya gösterilecek bilgiler burada View'a gönderilebilir
            ViewBag.TotalQuestions = 13;
            ViewBag.TotalJokers = 2;
            ViewBag.TotalTime = "30 saniye";
            return View();
        }


        [HttpPost]
        public IActionResult Answer(int id, string selectedOption)
        {
            var questionListJson = HttpContext.Session.GetString("QuestionList");
            var questionIndex = HttpContext.Session.GetInt32("QuestionIndex") ?? 0;

            if (string.IsNullOrEmpty(questionListJson))
                return RedirectToAction("Start");

            var questionList = System.Text.Json.JsonSerializer.Deserialize<List<Question>>(questionListJson);

            if (questionList == null || questionIndex >= questionList.Count)
                return RedirectToAction("GameOver");

            var currentQuestion = questionList[questionIndex];

            // ✅ Doğru cevap kontrolü
            if (selectedOption == currentQuestion.CorrectOption)
            {
                int level = HttpContext.Session.GetInt32("Level") ?? 0;
                int earnings = HttpContext.Session.GetInt32("Earnings") ?? 0;

                if (level < prizeLevels.Count)
                {
                    earnings = prizeLevels[level];
                    level++;

                    HttpContext.Session.SetInt32("Level", level);
                    HttpContext.Session.SetInt32("Earnings", earnings);
                }

                // ✅ Jokerleri sıfırla
                HttpContext.Session.SetString("FiftyUsed", "false");
                HttpContext.Session.Remove("HideOption1");
                HttpContext.Session.Remove("HideOption2");

                HttpContext.Session.Remove("AudienceUsed");
                HttpContext.Session.Remove("AudiencePoll");

                // ✅ Sıradaki soruya geç
                questionIndex++;
                HttpContext.Session.SetInt32("QuestionIndex", questionIndex);

                if (questionIndex >= questionList.Count)
                {
                    return RedirectToAction("GameOver");
                }

                var nextQuestion = questionList[questionIndex];

                // ✅ Yeni soruyu güncelle ve LastQuestionId'i set et
                HttpContext.Session.SetInt32("LastQuestionId", nextQuestion.Id);

                return View("Start", nextQuestion);
            }

            // ❌ Yanlış cevap
            return RedirectToAction("GameOver");
        }



        public IActionResult GameOver()
        {
            HttpContext.Session.Remove("ShownQuestionIds");
            return View();
        }



        private Question GetRandomQuestion(string difficulty)
        {
            // Session'dan gösterilen ID'leri al
            var shownQuestions = HttpContext.Session.GetString("ShownQuestionIds");
            List<int> shownIds = string.IsNullOrEmpty(shownQuestions)
                ? new List<int>()
                : shownQuestions.Split(',').Select(int.Parse).ToList();

            // Şartlara uyan soruları filtrele
            var questions = _context.Questions
                .Where(q => q.Difficulty == difficulty && !shownIds.Contains(q.Id))
                .OrderBy(q => Guid.NewGuid())
                .ToList();

            if (questions.Count == 0)
                return null;

            var selected = questions.First();

            // Seçilen ID'yi listeye ekle ve Session'a yaz
            shownIds.Add(selected.Id);
            HttpContext.Session.SetString("ShownQuestionIds", string.Join(",", shownIds));

            // Aynı soruyu %50 jokerinde tekrar gösterebilmek için
            HttpContext.Session.SetInt32("LastQuestionId", selected.Id);

            return selected;
        }


        private string GetDifficultyByLevel(int level)
        {
            if (level < 5)
                return "Kolay";
            else if (level < 10)
                return "Orta";
            else
                return "Zor";
        }


    }
}
