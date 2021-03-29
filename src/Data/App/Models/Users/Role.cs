using Cayent.Core.Data.Users;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.App.Models.Users
{
    public class Role : RoleBase
    {
        //public string RoleId { get; set; }
        //public string Name { get; set; }
        //public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    }

    public class RoleConfiguration : RoleConfiguration<Role>
    {
        public override void Configure(EntityTypeBuilder<Role> builder)
        {
            base.Configure(builder);
            this.ConfigureEntity(builder);
        }

        private void ConfigureEntity(EntityTypeBuilder<Role> builder)
        {
            //Registration of extension properties
            //builder.Property(t => t.FirstName).HasColumnName("FirstName");
            //builder.Property(t => t.LastName).HasColumnName("LastName");
            //builder.Property(t => t.Phone).HasColumnName("Phone");
            //builder.Property(t => t.Mobile).HasColumnName("Mobile");
        }
    }
}
