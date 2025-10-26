🎓 Bitirme Projesi: Bilgi Yarışması Uygulaması
Bu proje, ASP.NET Core MVC teknolojisi kullanılarak geliştirilmiş bir bilgi yarışması uygulamasıdır. Kullanıcılar sisteme kayıt olabilir, giriş yapabilir ve kolaydan zora doğru sıralanmış çoktan seçmeli soruları cevaplayarak ödül kazanabilir.

🧩 Proje Özellikleri
✔️ Kullanıcı Kayıt ve Giriş Sistemi (Identity ile)

✔️ 3 Seviye Zorlukta Soru Sistemi (Kolay, Orta, Zor)

✔️ %50 Joker ve Seyirci Jokeri

✔️ Her soru için 30 saniyelik sayaç

✔️ Soruya özel cevap seçenekleri ve dinamik gösterim

✔️ Ödül tablosu ve kazanç takibi

✔️ Oyun başlangıç bilgilendirme sayfası

✔️ Oyun bitince kazanç ekranı

🛠️ Teknolojiler
ASP.NET Core MVC

Entity Framework Core (Code First)

MS SQL Server

Bootstrap 5

Identity Authentication

📦 Kurulum Talimatları
Projeyi Visual Studio ile açın.

appsettings.json içinde SQL bağlantınızı ayarlayın.

Package Manager Console üzerinden:

Add-Migration InitialCreate
Update-Database
Uygulamayı çalıştırmak için Ctrl + F5 (veya dotnet run) komutu.

🧠 Veritabanı Yapısı
Sorular tablosu:

Id, QuestionText, OptionA, OptionB, OptionC, OptionD, CorrectOption, Difficulty

📌 Notlar
Sorular rastgele karıştırılır ama aynı soru tekrar gelmez.

Joker hakları sadece bir kez kullanılabilir, sonraki sorularda devre dışı kalır.

Soruya verilen cevap doğruysa kullanıcı bir sonraki seviyeye geçer.

Tüm sorular biterse veya süre dolarsa oyun sona erer.

👤 Geliştirici

Bu proje, Said Benli tarafından yapılmıştır. 
