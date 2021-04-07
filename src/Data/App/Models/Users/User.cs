

using Cayent.Core.Data.Users;
using Data.App.Models.Trips;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.App.Models.Users
{
    public class User : UserBase
    {
        public virtual ICollection<UserTask> UserTasks { get; set; } = new List<UserTask>();
    }

    public class UserConfiguration : UserConfiguration<User>
    {
        public override void Configure(EntityTypeBuilder<User> builder)
        {
            base.Configure(builder);
            this.ConfigureEntity(builder);
        }

        private void ConfigureEntity(EntityTypeBuilder<User> builder)
        {
            //builder.HasKey(e => e.UserId);
            //Registration of extension properties
            //builder.Property(t => t.FirstName).HasColumnName("FirstName");
            //builder.Property(t => t.LastName).HasColumnName("LastName");
            //builder.Property(t => t.Phone).HasColumnName("Phone");
            //builder.Property(t => t.Mobile).HasColumnName("Mobile");
        }
    }

}
