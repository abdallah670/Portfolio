using Microsoft.EntityFrameworkCore;

using PortfolioApi.Domain.Entities;
using PortfolioApi.Infrastructure.Data;

namespace PortfolioApi.Infrastructure.Data;

public class SeedService
{
    private readonly AppDbContext _context;
    
    public SeedService(AppDbContext context)
    {
        _context = context;
    }
    
    public async Task SeedInitialDataAsync()
    {
        // Seed Hero (only if missing)
        if (!await _context.Heroes.AnyAsync())
        {
            var hero = new Hero
            {
                Name = "Abdullah Mohammed",
                HeadlineTop = "Hi, I'm",
                HeadlineMain = "Abdullah Mohammed",
                AvailabilityLabel = "Available for Opportunities",
                Subtitle = "Full-Stack .NET Developer",
                ProfileImage = "https://res.cloudinary.com/dmyrxpvnj/image/upload/v1776913623/Meno_arycos.png"
            };
            _context.Heroes.Add(hero);
            
            var stats = new List<HeroStats>
            {
                new() { Label = "Projects", Value = "5+", DisplayOrder = 1 },
                new() { Label = "Backend Focus", Value = "100%", DisplayOrder = 2 },
                new() { Label = "SQL Expertise", Value = "Advanced", DisplayOrder = 3 }
            };
            _context.HeroStats.AddRange(stats);
            await _context.SaveChangesAsync();
        }
        
        // Seed Skill Categories (only if missing)
        if (!await _context.SkillCategories.AnyAsync())
        {
            var skillCategories = new List<SkillCategory>
            {
                new() { Title = "Backend Development", Color = "emerald", DisplayOrder = 1 },
                new() { Title = "Database", Color = "cyan", DisplayOrder = 2 },
                new() { Title = "Software Engineering", Color = "purple", DisplayOrder = 3 },
                new() { Title = "Frontend Development", Color = "blue", DisplayOrder = 4 }
            };
            _context.SkillCategories.AddRange(skillCategories);
            await _context.SaveChangesAsync();
        }
        
        // Seed Skills (only if missing)
        if (!await _context.Skills.AnyAsync())
        {
            var backendSkills = new List<Skill>
            {
                new() { CategoryId = 1, Name = "C#", Level = 85 },
                new() { CategoryId = 1, Name = ".NET", Level = 80 },
                new() { CategoryId = 1, Name = "ADO.NET", Level = 80 },
                new() { CategoryId = 1, Name = "REST APIs", Level = 75 }
            };
            
            var dbSkills = new List<Skill>
            {
                new() { CategoryId = 2, Name = "SQL Server", Level = 85 },
                new() { CategoryId = 2, Name = "Query Optimization", Level = 75 },
                new() { CategoryId = 2, Name = "Database Design", Level = 80 },
                new() { CategoryId = 2, Name = "Stored Procedures", Level = 75 }
            };
            
            var seSkills = new List<Skill>
            {
                new() { CategoryId = 3, Name = "Design Patterns", Level = 70 },
                new() { CategoryId = 3, Name = "Layered Architecture", Level = 80 },
                new() { CategoryId = 3, Name = "OOP", Level = 85 },
                new() { CategoryId = 3, Name = "Debugging", Level = 85 }
            };
            
            var feSkills = new List<Skill>
            {
                new() { CategoryId = 4, Name = "Angular", Level = 70 },
                new() { CategoryId = 4, Name = "TypeScript", Level = 75 },
                new() { CategoryId = 4, Name = "HTML/CSS", Level = 75 },
                new() { CategoryId = 4, Name = "RxJS", Level = 60 }
            };
            
            _context.Skills.AddRange(backendSkills.Concat(dbSkills).Concat(seSkills).Concat(feSkills));
            await _context.SaveChangesAsync();
        }
        
        // Seed Projects (only if missing)
        if (!await _context.Projects.AnyAsync())
        {
            var projects = new List<Project>
            {
                new() { Title = "MenoPro - Gym Management System", Year = "2025", Category = "Web Application", Description = "Premium gym management with member/trainer portals, workout plans, diet tracking, and Chart.js analytics. Features Glassmorphism UI, Stripe payments, and Gemini AI integration.", Stack = "[\"ASP.NET Core MVC\",\"SQL Server\",\"Entity Framework\",\"Chart.js\",\"Stripe\",\"Gemini AI\"]", Image = "https://res.cloudinary.com/dmyrxpvnj/image/upload/v1776913594/gymmvc_oqc6tu.png", linkedinUrl = "https://www.linkedin.com/posts/abdullah-mohammed-334475294_aspnetcore-csharp-webdevelopment-activity-7424228685093994496-wSwl?utm_source=share&utm_medium=member_desktop&rcm=ACoAAEdCipgBcPb4fee5CeW-8yGc1BXjUpbeHs0", GithubUrl = "https://github.com/abdallah670/GymMVC",LiveUrl="", Status = "Production", Color = "emerald", IsFeatured = true, DisplayOrder = 1,IsPublished=true },
                new() { Title = "Labor Marketplace System", Year = "2026", Category = "Full-Stack Platform", Description = "Platform connecting workers with job posters. Features multi-role auth, real-time chat with SignalR, Stripe payments, Hangfire jobs, and geographic search with SQL Server spatial queries.", Stack = "[\"ASP.NET Core MVC\",\".NET 9\",\"SignalR\",\"Stripe Connect\",\"Hangfire\",\"NetTopologySuite\"]", Image = "https://res.cloudinary.com/dmyrxpvnj/image/upload/v1776913593/labormvc_olkw8r.png", linkedinUrl = "https://www.linkedin.com/posts/abdullah-mohammed-334475294_dotnet-architecture-systemdesign-activity-7444313186763358208-gBDD?utm_source=share&utm_medium=member_desktop&rcm=ACoAAEdCipgBcPb4fee5CeW-8yGc1BXjUpbeHs0", GithubUrl = "https://github.com/abdallah670/LaborMVC",LiveUrl="", Status = "Production", Color = "cyan", IsFeatured = true, DisplayOrder = 2 ,IsPublished=true},
                new() { Title = "Outfit Planner", Year = "2026", Category = "Web Application", Description = "Intelligent wardrobe management system that generates outfit suggestions by analyzing clothes against real-time weather, occasions, and personal style preferences. Built with Clean Architecture and CQRS.", Stack = "[\"ASP.NET Core 9\",\"Angular 17+\",\"NgRx\",\"SQL Server\",\"Clean Architecture\",\"CQRS\"]", Image = "https://res.cloudinary.com/dmyrxpvnj/image/upload/v1776913592/outfitplanner_zyzjur.jpg", GithubUrl = "https://github.com/abdallah670/Outfit-Planner",linkedinUrl="",LiveUrl="",Status="In development", Color = "pink", IsFeatured = false, DisplayOrder = 1,IsPublished=true }
            };
            _context.Projects.AddRange(projects);
            await _context.SaveChangesAsync();
        }
        
        // Seed Journey (only if missing)
        if (!await _context.JourneyItems.AnyAsync())
        {
            var journey = new List<JourneyItem>
            {
                new() { Title = "Computer Science Student", Period = "2023 - Present", Org = "Faculty of Computers and Information (FCI)", Description = "Built a strong foundation in programming, data structures, and problem solving. Focused on understanding how systems work rather than just writing code.", DisplayOrder = 1 },
                new() { Title = "Backend Development with .NET", Period = "2024 - Present", Org = "Self Learning & Projects", Description = "Specialized in backend development using C# and .NET. Built multiple systems focusing on clean architecture, layered design, and maintainable code.", DisplayOrder = 2 },
                new() { Title = "Database Design & SQL", Period = "2024 - Present", Org = "Projects & Practice", Description = "Designed relational databases and wrote complex SQL queries. Worked on data integrity, relationships, and optimizing queries for real-world systems.", DisplayOrder = 3 },
                new() { Title = "System Building & Architecture", Period = "2025 - Present", Org = "Hands-on Projects", Description = "Developed full systems like Gym and Banking applications using layered architecture (DAL, Business, DTO). Focused on structuring code, handling business rules, and building scalable solutions.", DisplayOrder = 4 },
                new() { Title = "Frontend Integration (Angular)", Period = "2025 - Present", Org = "Project Integration", Description = "Used Angular to build user interfaces connected to backend systems. Focused on integrating APIs and creating functional dashboards rather than purely UI design.", DisplayOrder = 5 }
            };
            _context.JourneyItems.AddRange(journey);
            await _context.SaveChangesAsync();
        }
        
        // Seed Social Links (only if missing)
        if (!await _context.SocialLinks.AnyAsync())
        {
            var socials = new List<SocialLink>
            {
                new() { Label = "GitHub", Href = "https://github.com/abdallah670", Icon = "github" },
                new() { Label = "LinkedIn", Href = "https://linkedin.com/in/abdullah-mohammed-334475294", Icon = "linkedin" },
                new() { Label = "Instagram", Href = "https://instagram.com/meno221104", Icon = "instagram" }
            };
            _context.SocialLinks.AddRange(socials);
            await _context.SaveChangesAsync();
        }
        
        // Seed Contact (only if missing)
        if (!await _context.Contacts.AnyAsync())
        {
            var contact = new Contact
            {
                Email = "meno.mo.dev@gmail.com",
                WhatsApp = "+201205450824",
                Phone = "+201205450824",
                Location = "Cairo, Egypt"
            };
            _context.Contacts.Add(contact);
            await _context.SaveChangesAsync();
        }

        // Seed Messages (only if missing)
        if (!await _context.Messages.AnyAsync())
        {
            var messages = new List<Message>
            {
                new()
                {
                    Name = "Hatem",
                    Email = "hnbg14006@gmail.com",
                    Subject = "Collaboration Inquiry",
                    Content = "Hello Abdullah,\n\nI came across your portfolio and I'm impressed by your work. I'd like to discuss a potential collaboration on a .NET project.\n\nPlease let me know if you're interested.\n\nBest regards,\nHatem",
                    IsRead = false,
                    IsReplied = false,
                    CreatedAt = DateTime.UtcNow.AddDays(-3)
                },
                new()
                {
                    Name = "Binfo",
                    Email = "binfof123@gmail.com",
                    Subject = "Project Collaboration",
                    Content = "Hi,\n\nI'm looking for a skilled .NET developer to join our team. Your experience with ASP.NET Core and Angular seems like a great match.\n\nAre you available for a quick chat?\n\nThanks,\nBinfo",
                    IsRead = true,
                    IsReplied = true,
                    CreatedAt = DateTime.UtcNow.AddDays(-5),
                    ReadAt = DateTime.UtcNow.AddDays(-5),
                    RepliedAt = DateTime.UtcNow.AddDays(-4)
                },
                new()
                {
                    Name = "Student",
                    Email = "20231104@stud.fci-cu.edu.eg",
                    Subject = "Question about Clean Architecture",
                    Content = "Dear Abdullah,\n\nI'm a CS student at FCI and I've been studying your portfolio projects. Could you explain more about how you structured your Clean Architecture implementation?\n\nI'm currently working on my graduation project and your approach would be very helpful.\n\nThank you for your time.\n\nSincerely,\nFCI Student",
                    IsRead = false,
                    IsReplied = false,
                    CreatedAt = DateTime.UtcNow.AddDays(-1)
                },
                new()
                {
                    Name = "Ahmed",
                    Email = "ahmed@example.com",
                    Subject = "Job Opportunity",
                    Content = "Hello,\n\nWe have an opening for a Senior .NET Developer at our company. Based on your portfolio, you might be a good fit.\n\nWould you be interested in learning more?\n\nBest,\nAhmed (HR Manager)",
                    IsRead = true,
                    IsReplied = false,
                    CreatedAt = DateTime.UtcNow.AddDays(-7),
                    ReadAt = DateTime.UtcNow.AddDays(-7)
                },
                new()
                {
                    Name = "Sarah",
                    Email = "sarah.m@techcorp.com",
                    Subject = "Freelance Project",
                    Content = "Hi Abdullah,\n\nWe need help building a REST API for our new product. Can you share your availability and rates?\n\nRegards,\nSarah",
                    IsRead = false,
                    IsReplied = false,
                    CreatedAt = DateTime.UtcNow.AddHours(-2)
                },
                new()
                {
                    Name = "Mohamed",
                    Email = "mohamed.dev@outlook.com",
                    Subject = "Mentorship Request",
                    Content = "Hello,\n\nI'm a junior developer looking for a mentor. Your projects inspired me to learn .NET. Would you consider mentoring me?\n\nThanks,\nMohamed",
                    IsRead = false,
                    IsReplied = false,
                    CreatedAt = DateTime.UtcNow.AddDays(-10)
                },
                new()
                {
                    Name = "Ali",
                    Email = "ali.k@gmail.com",
                    Subject = "Question about SignalR",
                    Content = "Hi,\n\nHow did you implement real-time chat in your Labor Marketplace? I'm trying to learn SignalR.\n\nThanks,\nAli",
                    IsRead = true,
                    IsReplied = false,
                    CreatedAt = DateTime.UtcNow.AddDays(-12),
                    ReadAt = DateTime.UtcNow.AddDays(-12)
                },
                new()
                {
                    Name = "Fatma",
                    Email = "fatma.ahmed@company.com",
                    Subject = "Partnership Opportunity",
                    Content = "Dear Abdullah,\n\nWe're looking for a development partner for our startup. Interested in discussing?\n\nBest,\nFatma",
                    IsRead = false,
                    IsReplied = false,
                    CreatedAt = DateTime.UtcNow.AddDays(-15)
                },
                new()
                {
                    Name = "Omar",
                    Email = "omar.n@email.com",
                    Subject = "Code Review Request",
                    Content = "Hello,\n\nCould you review my GitHub repository? I'm building a similar gym management system.\n\nThanks,\nOmar",
                    IsRead = true,
                    IsReplied = true,
                    CreatedAt = DateTime.UtcNow.AddDays(-20),
                    ReadAt = DateTime.UtcNow.AddDays(-20),
                    RepliedAt = DateTime.UtcNow.AddDays(-19)
                },
                new()
                {
                    Name = "Youssef",
                    Email = "youssef.s@business.com",
                    Subject = "Consulting Inquiry",
                    Content = "Hi,\n\nWe need consulting on our architecture. What's your hourly rate?\n\nRegards,\nYoussef",
                    IsRead = false,
                    IsReplied = false,
                    CreatedAt = DateTime.UtcNow.AddDays(-25)
                },
                new()
                {
                    Name = "Nour",
                    Email = "nour.h@startup.io",
                    Subject = "Job Interview",
                    Content = "Hello,\n\nWe'd like to invite you for an interview. Are you available next week?\n\nBest,\nNour (HR)",
                    IsRead = true,
                    IsReplied = false,
                    CreatedAt = DateTime.UtcNow.AddDays(-30),
                    ReadAt = DateTime.UtcNow.AddDays(-30)
                },
                new()
                {
                    Name = "Karim",
                    Email = "karim.m@agency.com",
                    Subject = "Contract Work",
                    Content = "Hi,\n\nWe have a 3-month contract for a .NET developer. Interested?\n\nThanks,\nKarim",
                    IsRead = false,
                    IsReplied = false,
                    CreatedAt = DateTime.UtcNow.AddDays(-35)
                },
                new()
                {
                    Name = "Layla",
                    Email = "layla.a@design.co",
                    Subject = "UI/UX Collaboration",
                    Content = "Hello,\n\nI'm a designer looking for a developer partner. Want to build something together?\n\nLayla",
                    IsRead = true,
                    IsReplied = false,
                    CreatedAt = DateTime.UtcNow.AddDays(-40),
                    ReadAt = DateTime.UtcNow.AddDays(-40)
                },
                new()
                {
                    Name = "Tarek",
                    Email = "tarek.w@corp.net",
                    Subject = "Full-time Position",
                    Content = "Dear Developer,\n\nWe have a full-time position available. Your skills match our requirements.\n\nBest,\nTarek",
                    IsRead = false,
                    IsReplied = false,
                    CreatedAt = DateTime.UtcNow.AddDays(-45)
                },
                new()
                {
                    Name = "Dina",
                    Email = "dina.f@edu.org",
                    Subject = "Academic Project Help",
                    Content = "Hi,\n\nCan you help me with my final year project? It's about e-commerce.\n\nDina",
                    IsRead = false,
                    IsReplied = false,
                    CreatedAt = DateTime.UtcNow.AddDays(-50)
                },
                new()
                {
                    Name = "Hossam",
                    Email = "hossam.a@tech.io",
                    Subject = "Open Source Contribution",
                    Content = "Hello,\n\nWould you accept contributions to your Gym MVC project?\n\nHossam",
                    IsRead = true,
                    IsReplied = true,
                    CreatedAt = DateTime.UtcNow.AddDays(-55),
                    ReadAt = DateTime.UtcNow.AddDays(-55),
                    RepliedAt = DateTime.UtcNow.AddDays(-54)
                },
                new()
                {
                    Name = "Rania",
                    Email = "rania.k@consulting.com",
                    Subject = "Technical Discussion",
                    Content = "Hi,\n\nCan we discuss CQRS pattern? I want to implement it in our project.\n\nRania",
                    IsRead = false,
                    IsReplied = false,
                    CreatedAt = DateTime.UtcNow.AddDays(-60)
                },
                new()
                {
                    Name = "Waleed",
                    Email = "waleed.m@enterprise.com",
                    Subject = "Enterprise Project",
                    Content = "Hello,\n\nWe're building a large-scale system. Looking for experienced developers.\n\nWaleed",
                    IsRead = false,
                    IsReplied = false,
                    CreatedAt = DateTime.UtcNow.AddDays(-65)
                },
                new()
                {
                    Name = "Mona",
                    Email = "mona.s@agency.net",
                    Subject = "Remote Work",
                    Content = "Hi,\n\nDo you work remotely? We have positions available.\n\nMona",
                    IsRead = true,
                    IsReplied = false,
                    CreatedAt = DateTime.UtcNow.AddDays(-70),
                    ReadAt = DateTime.UtcNow.AddDays(-70)
                },
                new()
                {
                    Name = "Sayed",
                    Email = "sayed.h@startup.com",
                    Subject = "MVP Development",
                    Content = "Hello,\n\nWe need an MVP built quickly. What's your availability?\n\nSayed",
                    IsRead = false,
                    IsReplied = false,
                    CreatedAt = DateTime.UtcNow.AddDays(-75)
                },
                new()
                {
                    Name = "Amira",
                    Email = "amira.r@company.io",
                    Subject = "Career Advice",
                    Content = "Hi Abdullah,\n\nI'm transitioning to development. Any advice for a beginner?\n\nAmira",
                    IsRead = false,
                    IsReplied = false,
                    CreatedAt = DateTime.UtcNow.AddDays(-80)
                },
                new()
                {
                    Name = "Ibrahim",
                    Email = "ibrahim.k@business.com",
                    Subject = "Long-term Partnership",
                    Content = "Hello,\n\nWe're looking for a technical partner for ongoing projects.\n\nIbrahim",
                    IsRead = false,
                    IsReplied = false,
                    CreatedAt = DateTime.UtcNow.AddDays(-85)
                },
                new()
                {
                    Name = "Salma",
                    Email = "salma.m@edu.edu",
                    Subject = "Teaching Opportunity",
                    Content = "Hi,\n\nWe're looking for a guest lecturer on .NET. Are you interested?\n\nSalma",
                    IsRead = false,
                    IsReplied = false,
                    CreatedAt = DateTime.UtcNow.AddDays(-90)
                },
                new()
                {
                    Name = "Ziad",
                    Email = "ziad.a@techcorp.com",
                    Subject = "Tech Talk",
                    Content = "Hello,\n\nWould you like to give a tech talk at our company?\n\nZiad",
                    IsRead = false,
                    IsReplied = false,
                    CreatedAt = DateTime.UtcNow.AddDays(-95)
                }
            };
            _context.Messages.AddRange(messages);
            await _context.SaveChangesAsync();
        }
        
        // Seed CV placeholder setting (only if missing)
        if (!await _context.SystemSettings.AnyAsync(s => s.Key == "cv_url"))
        {
            var cvSetting = new SystemSetting
            {
                Key = "cv_url",
                Value = "https://res.cloudinary.com/dmyrxpvnj/raw/upload/cv/Abdullah_Mohammed_CV.pdf",
                Category = "files",
                DataType = "string"
            };
            _context.SystemSettings.Add(cvSetting);
            
            await _context.SaveChangesAsync();
        }
    }
}
