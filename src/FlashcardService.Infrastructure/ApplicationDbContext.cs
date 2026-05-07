using Microsoft.EntityFrameworkCore;

namespace FlashcardService.Infrastructure;

public sealed class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    
}