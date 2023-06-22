
using Microsoft.EntityFrameworkCore;
using SafeZone.Models;

namespace SafeZone.Data
{
    public class DataContext:DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<VerificationCode> VerificationCodes { get; set; }
        public DbSet<PasswordResetCode> PasswordResetCodes { get; set; }
        public DbSet<Officer> Officers { get; set; }
        public DbSet<Crime> Crimes { get; set; }
        public DbSet<CloseCase> CloseCases { get; set; }
        public DbSet<CheckoutOrderResponse> CheckoutOrderResponses { get; set; }
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
            
        }
    }
}
