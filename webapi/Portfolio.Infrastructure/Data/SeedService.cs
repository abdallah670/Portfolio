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
        if (await _context.Heroes.AnyAsync())
            return;
            
        var hero = new Hero
        {
            Name = "Abdullah Mohammed",
            HeadlineTop = "Hi, I'm",
            HeadlineMain = "Abdullah Mohammed",
            AvailabilityLabel = "Available for Opportunities",
            Subtitle = "Full-Stack .NET Developer",
            HeroIntro = "Backend-focused .NET developer specializing in building scalable systems using C#, SQL Server, and clean architecture. Passionate about system design, data handling, and writing maintainable, production-ready code.",
            CtaPrimaryLabel = "View My Work",
            CtaPrimaryHref = "/projects",
            CtaSecondaryLabel = "Get In Touch",
            CtaSecondaryHref = "/contact",
            ProfileImage = "uploads/profile-image/Meno.png"
        };
        _context.Heroes.Add(hero);
        
        var stats = new List<HeroStats>
        {
            new() { Label = "Projects", Value = "5+", DisplayOrder = 1 },
            new() { Label = "Backend Focus", Value = "100%", DisplayOrder = 2 },
            new() { Label = "SQL Expertise", Value = "Advanced", DisplayOrder = 3 }
        };
        _context.HeroStats.AddRange(stats);
        
        var about = new About
        {
            Kicker = "Get to Know Me",
            Title = "About Me",
            Subtitle = "A backend developer focused on building reliable and scalable systems",
            FunFact = "I enjoy turning complex system requirements into clean, structured code."
        };
        _context.Abouts.Add(about);
        
        var aboutCards = new List<AboutCard>
        {
            new() { Title = ".NET Developer", Subtitle = "C#, ADO.NET, Windows Services", DisplayOrder = 1 },
            new() { Title = "Database Specialist", Subtitle = "SQL Server, Query Optimization", DisplayOrder = 2 },
            new() { Title = "System Design", Subtitle = "Layered Architecture, Clean Code", DisplayOrder = 3 },
            new() { Title = "Problem Solver", Subtitle = "Real-world system implementation", DisplayOrder = 4 }
        };
        _context.AboutCards.AddRange(aboutCards);
        
        var achievements = new List<Achievement>
        {
            new() { Text = "Built Online coaching system with transaction handling", DisplayOrder = 1 },
            new() { Text = "Implemented layered architecture (DAL, Business, DTO)", DisplayOrder = 2 },
            new() { Text = "Designed SQL databases and Entity Framework models", DisplayOrder = 3 },
            new() { Text = "Hands-on with real system logic and constraints", DisplayOrder = 4 }
        };
        _context.Achievements.AddRange(achievements);
        
        var values = new List<Value>
        {
            new() { Title = "Clean Architecture", Description = "Focus on separation of concerns and maintainable system design.", DisplayOrder = 1 },
            new() { Title = "Data Integrity", Description = "Strong emphasis on correct data handling and database design.", DisplayOrder = 2 },
            new() { Title = "Scalability", Description = "Building systems that can grow without breaking.", DisplayOrder = 3 }
        };
        _context.Values.AddRange(values);
        
        var skillCategories = new List<SkillCategory>
        {
            new() { Title = "Backend Development", Color = "emerald", DisplayOrder = 1 },
            new() { Title = "Database", Color = "cyan", DisplayOrder = 2 },
            new() { Title = "Software Engineering", Color = "purple", DisplayOrder = 3 },
            new() { Title = "Frontend Development", Color = "blue", DisplayOrder = 4 }
        };
        _context.SkillCategories.AddRange(skillCategories);
        await _context.SaveChangesAsync();
        
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
        
        var projects = new List<Project>
        {
            new() { Title = "MenoPro - Gym Management System", Year = "2025", Category = "Web Application", Description = "Premium gym management with member/trainer portals, workout plans, diet tracking, and Chart.js analytics. Features Glassmorphism UI, Stripe payments, and Gemini AI integration.", Stack = "[\"ASP.NET Core MVC\",\"SQL Server\",\"Entity Framework\",\"Chart.js\",\"Stripe\",\"Gemini AI\"]", Image = "uploads/projects/gymmvc.png", LiveUrl = "https://www.linkedin.com/posts/abdullah-mohammed-334475294_aspnetcore-csharp-webdevelopment-activity-7424228685093994496-wSwl?utm_source=share&utm_medium=member_desktop&rcm=ACoAAEdCipgBcPb4fee5CeW-8yGc1BXjUpbeHs0", GithubUrl = "https://github.com/abdallah670/GymMVC", Status = "Production", Color = "emerald", IsFeatured = true, DisplayOrder = 1 },
            new() { Title = "Labor Marketplace System", Year = "2026", Category = "Full-Stack Platform", Description = "Platform connecting workers with job posters. Features multi-role auth, real-time chat with SignalR, Stripe payments, Hangfire jobs, and geographic search with SQL Server spatial queries.", Stack = "[\"ASP.NET Core MVC\",\".NET 9\",\"SignalR\",\"Stripe Connect\",\"Hangfire\",\"NetTopologySuite\"]", Image = "uploads/projects/labormvc.png", LiveUrl = "https://www.linkedin.com/posts/abdullah-mohammed-334475294_dotnet-architecture-systemdesign-activity-7444313186763358208-gBDD?utm_source=share&utm_medium=member_desktop&rcm=ACoAAEdCipgBcPb4fee5CeW-8yGc1BXjUpbeHs0", GithubUrl = "https://github.com/abdallah670/LaborMVC", Status = "Production", Color = "cyan", IsFeatured = true, DisplayOrder = 2 },
            new() { Title = "Outfit Planner", Year = "2026", Category = "Web Application", Description = "Intelligent wardrobe management system that generates outfit suggestions by analyzing clothes against real-time weather, occasions, and personal style preferences. Built with Clean Architecture and CQRS.", Stack = "[\"ASP.NET Core 9\",\"Angular 17+\",\"NgRx\",\"SQL Server\",\"Clean Architecture\",\"CQRS\"]", Image = "uploads/projects/outfitplanner.jpg", GithubUrl = "https://github.com/abdallah670/Outfit-Planner",LiveUrl="", Color = "pink", IsFeatured = false, DisplayOrder = 1 }
        };
        _context.Projects.AddRange(projects);
        
        var journey = new List<JourneyItem>
        {
            new() { Title = "Computer Science Student", Period = "2023 - Present", Org = "Faculty of Computers and Information (FCI)", Description = "Built a strong foundation in programming, data structures, and problem solving. Focused on understanding how systems work rather than just writing code.", DisplayOrder = 1 },
            new() { Title = "Backend Development with .NET", Period = "2024 - Present", Org = "Self Learning & Projects", Description = "Specialized in backend development using C# and .NET. Built multiple systems focusing on clean architecture, layered design, and maintainable code.", DisplayOrder = 2 },
            new() { Title = "Database Design & SQL", Period = "2024 - Present", Org = "Projects & Practice", Description = "Designed relational databases and wrote complex SQL queries. Worked on data integrity, relationships, and optimizing queries for real-world systems.", DisplayOrder = 3 },
            new() { Title = "System Building & Architecture", Period = "2025 - Present", Org = "Hands-on Projects", Description = "Developed full systems like Gym and Banking applications using layered architecture (DAL, Business, DTO). Focused on structuring code, handling business rules, and building scalable solutions.", DisplayOrder = 4 },
            new() { Title = "Frontend Integration (Angular)", Period = "2025 - Present", Org = "Project Integration", Description = "Used Angular to build user interfaces connected to backend systems. Focused on integrating APIs and creating functional dashboards rather than purely UI design.", DisplayOrder = 5 }
        };
        _context.JourneyItems.AddRange(journey);
        
        var socials = new List<SocialLink>
        {
            new() { Label = "GitHub", Href = "https://github.com/abdallah670", Icon = "github" },
            new() { Label = "LinkedIn", Href = "https://linkedin.com/in/abdullah-mohammed-334475294", Icon = "linkedin" },
            new() { Label = "Instagram", Href = "https://instagram.com/meno221104", Icon = "instagram" }
        };
        _context.SocialLinks.AddRange(socials);
        
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
}
