using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Linq;

namespace UserTablesPrimer.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        private Model1 db = new Model1();
        public string Name { get; set; }
        public string Surname { get; set; }
        public int Tokens { get; set; }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            userIdentity.AddClaim(new Claim("Name", this.Name));
            userIdentity.AddClaim(new Claim("Surname", this.Surname));
            userIdentity.AddClaim(new Claim("Tokens", this.Tokens.ToString()));
            var userId = userIdentity.GetUserId();
            var numOfNots = db.Notifications.Where(n => (n.UserId == userId) && (n.Read == 0)).Count();
            userIdentity.AddClaim(new Claim("NumOfNotifications", numOfNots.ToString()));
            return userIdentity;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }
}