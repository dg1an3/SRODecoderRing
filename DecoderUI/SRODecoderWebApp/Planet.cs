using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRODecoderWebApp
{
    public enum PlanetStructure
    {
        Rock, Gas
    }

    public class Planet
    {
        public string Name { get; set; }
        public int Radius { get; set; }
        public PlanetStructure Structure { get; set; }
        public bool Bright { get; set; }
        public string RotationPeriod { get; set; }
        public string OrbitalPeriod { get; set; }
        public string ImagePath { get; set; }


        public static ObservableCollection<Planet> GetListOfPlanets()
        {
            return new ObservableCollection<Planet>()
            {
                new Planet() { Name = "Mercury", Structure = PlanetStructure.Rock, Bright=true, Radius = 2400, RotationPeriod = "59 days", OrbitalPeriod = "3 months", ImagePath = "ms-appx:/Planets/Mercury.png" },
                new Planet() { Name = "Venus", Structure = PlanetStructure.Rock, Bright=true, Radius = 6100, RotationPeriod = "243 days", OrbitalPeriod = "7 months", ImagePath = "ms-appx:/Planets/Venus.png" },
                new Planet() { Name = "Earth", Structure = PlanetStructure.Rock, Bright=true, Radius = 6400, RotationPeriod = "1 day", OrbitalPeriod = "1 year", ImagePath = "ms-appx:/Planets/Earth.png" },
                new Planet() { Name = "Mars", Structure = PlanetStructure.Rock, Bright=true, Radius = 3400, RotationPeriod = "1 day, 37 min", OrbitalPeriod = "2 years", ImagePath = "ms-appx:/Planets/Mars.png" },
                new Planet() { Name = "Jupiter", Structure = PlanetStructure.Gas, Bright=true, Radius = 71500, RotationPeriod = "1 day, 10 hrs", OrbitalPeriod = "12 years", ImagePath = "ms-appx:/Planets/Jupiter.png" },
                new Planet() { Name = "Saturn", Structure = PlanetStructure.Gas, Bright=true, Radius = 60300, RotationPeriod = "1 day, 11 hrs", OrbitalPeriod = "30 years", ImagePath = "ms-appx:/Planets/Saturn.png" },
                new Planet() { Name = "Uranus", Structure = PlanetStructure.Gas, Bright=false, Radius = 25600, RotationPeriod = "1 day, 17 hrs", OrbitalPeriod = "84 years", ImagePath = "ms-appx:/Planets/Uranus.png" },
                new Planet() { Name = "Neptune", Structure = PlanetStructure.Gas, Bright=false, Radius = 24800, RotationPeriod = "1 day, 16 hrs", OrbitalPeriod = "165 years", ImagePath = "ms-appx:/Planets/Neptune.png" },
            };
        }
    }
}
