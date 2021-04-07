

using Cayent.Core.Data.Users;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.App.Models.Users
{
    public class UserRole: UserRoleBase
    {
        //public string UserId { get; set; }
        //public virtual User User { get; set; }

        //public string RoleId { get; set; }
        //public virtual Role Role { get; set; }

    }

    public class UserRoleConfiguration : UserRoleConfiguration<UserRole>
    {
        public override void Configure(EntityTypeBuilder<UserRole> builder)
        {
            base.Configure(builder);
            this.ConfigureEntity(builder);
        }

        private void ConfigureEntity(EntityTypeBuilder<UserRole> builder)
        {
            //builder.HasKey(e => new { e.UserId, e.RoleId });
            //Registration of extension properties
            //builder.Property(t => t.FirstName).HasColumnName("FirstName");
            //builder.Property(t => t.LastName).HasColumnName("LastName");
            //builder.Property(t => t.Phone).HasColumnName("Phone");
            //builder.Property(t => t.Mobile).HasColumnName("Mobile");
        }
    }
}
