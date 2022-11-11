using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.ApiAuthorization.IdentityServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Duende.IdentityServer.EntityFramework.Options;
using Duende.IdentityServer.EntityFramework.Entities;

namespace Habit_Tracker___Doveloop.Data
{
    public class CosmosIdentityDbContext<TUser> : ApiAuthorizationDbContext<TUser> where TUser : IdentityUser
    {

        public CosmosIdentityDbContext(DbContextOptions options, IOptions<OperationalStoreOptions> operationalStoreOptions) : base(options, operationalStoreOptions) { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            //identity models
            builder.Entity<TUser>(b =>
            {
                b.HasKey(u => u.Id);
                b.HasPartitionKey(_ => _.Id);
                b.Property(_ => _.ConcurrencyStamp).IsETagConcurrency();

                b.ToContainer("Users");
            });
            builder.Entity<IdentityUserRole<string>>(b =>
            {
                b.UseETagConcurrency().HasPartitionKey(_ => _.UserId);
                b.ToContainer("UserRoles");
            });
            builder.Entity<IdentityRole>(b =>
            {
                b.HasKey(_ => _.Id);
                b.HasPartitionKey(_ => _.Id);
                b.Property(_ => _.ConcurrencyStamp).IsETagConcurrency();
                b.ToContainer("Roles");
            });
            builder.Entity<IdentityRoleClaim<string>>(b =>
            {
                b.Property(_ => _.Id).HasConversion(_ => _.ToString(), _ => Convert.ToInt32(_));
                b.UseETagConcurrency().HasPartitionKey(_ => _.Id);
                b.ToContainer("Users");
            });
            builder.Entity<IdentityUserClaim<string>>(b => {
                b.Property(_ => _.Id).HasConversion(_ => _.ToString(), _ => Convert.ToInt32(_));
                b.UseETagConcurrency().HasPartitionKey(_ => _.Id);
                b.ToContainer("Users");
            });
            builder.Entity<IdentityUserLogin<string>>(b =>
            {
                b.UseETagConcurrency().HasPartitionKey(_ => _.ProviderKey);
                b.ToContainer("Logins");
            });
            builder.Entity<IdentityUserToken<string>>(b =>
            {
                b.UseETagConcurrency().HasPartitionKey(_ => _.UserId);
                b.ToContainer("Tokens");
            });
            builder.Entity<DeviceFlowCodes>(b =>
            {
                b.HasKey(_ => new { _.ClientId, _.SessionId, _.DeviceCode });
                b.UseETagConcurrency().HasPartitionKey(_ => _.SessionId);
                b.ToContainer("DeviceFlowCodes");
            });
            builder.Entity<PersistedGrant>(b =>
            {
                b.HasKey(_ => new { _.Type, _.ClientId, _.SessionId });
                b.UseETagConcurrency().HasPartitionKey(_ => _.Key);
                b.ToContainer("PersistedGrant");
            });
        }
    }
}
