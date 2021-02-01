using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.App.Models.Terminals
{
    public class Driver
    {
        public string DriverId { get; set; }

        public virtual ICollection<Vehicle> Vehicles { get; set; } = new List<Vehicle>();
    }

    public class Vehicle
    {
        public string VehicleId { get; set; }
    }

    public class Toda
    {
        public string TodaId { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }

        public virtual ICollection<Terminal> Terminals { get; set; } = new List<Terminal>();
    }

    public class Terminal
    {
        public string TerminalId { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }

        public double GeoX { get; set; }
        public double GeoY { get; set; }
    }
}
