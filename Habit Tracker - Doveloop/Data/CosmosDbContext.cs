using Duende.IdentityServer.EntityFramework.Options;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Habit_Tracker___Doveloop.Data
{
    public class CosmosDbContext : CosmosIdentityDbContext<IdentityUser>
    {

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }

        public CosmosDbContext(DbContextOptions dbContextOptions, IOptions<OperationalStoreOptions> operationalStoreOptions) : base(dbContextOptions, operationalStoreOptions) { }
    }
}
