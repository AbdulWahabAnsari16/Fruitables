using Microsoft.EntityFrameworkCore;

namespace Fruitables.Models
{
    public class MainDbContextFile : DbContext
    {
        public MainDbContextFile(DbContextOptions options) : base(options)
        {
        }

        public DbSet<contact> contacts { get; set; }
        public DbSet<category> categories { get; set; }
        public DbSet<product> products { get; set; }
        public DbSet<adminLogin> adminLogins { get; set; }
        public DbSet<addtocart> addtocarts { get; set; }
        public DbSet<wishlist> wishlists { get; set; }
        public DbSet<user> users { get; set; }
        public DbSet<coupon> coupons { get; set; }
        public DbSet<verificationCode> vCodes { get; set; }
    }
}
