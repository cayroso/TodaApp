using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Constants
{
    public sealed class ApplicationRoles
    {
        private ApplicationRoles(string id, string name)
        {
            Id = id;
            Name = name;
        }

        public string Id { get; }
        public string Name { get; }

        public const string SystemRoleName = "System";
        public static ApplicationRoles System = new ApplicationRoles(SystemRoleName.ToLower(), SystemRoleName);

        public const string AdministratorRoleName = "Administrator";
        public static ApplicationRoles Administrator = new ApplicationRoles(AdministratorRoleName.ToLower(), AdministratorRoleName);

        public const string DriverRoleName = "Driver";
        public static ApplicationRoles Driver = new ApplicationRoles(DriverRoleName.ToLower(), DriverRoleName);

        public const string RiderRoleName = "Rider";
        public static ApplicationRoles Rider = new ApplicationRoles(RiderRoleName.ToLower(), RiderRoleName);

        public static List<ApplicationRoles> Items
        {
            get
            {
                return new List<ApplicationRoles>
                {
                    System,
                    Administrator,
                    Driver,
                    Rider
                };
            }
        }
    }
}
