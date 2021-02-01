using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Identity.Models.Users
{
    public class IdentityWebUser : IdentityUser
    {
        public string TenantId { get; set; }
        public virtual Tenant Tenant { get; set; }
        //public virtual ICollection<Customer> Customers { get; set; } = new List<Customer>();

        public virtual UserInformation UserInformation { get; set; }

        public virtual ICollection<LoginAudit> LoginAudits { get; set; } = new List<LoginAudit>();
    }
}
