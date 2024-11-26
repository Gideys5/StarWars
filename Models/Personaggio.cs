using System;
using System.Collections.Generic;


namespace StarWars_Aismondo.Models
{
    public class Personaggio 
    {
        public int Id { get; set; } 
        public string Name { get; set; }
        public string Height { get; set; }
        public string Mass { get; set; }
        public string Skin_Color { get; set; }
        public string Birth_Year { get; set; }
        public string Gender { get; set; }
        public string Homeworld { get; set; }
        public List<Personaggio> Results { get; set; }
        public List<string> Vehicles { get; set; } = new List<string>();
        public List<string> Starships { get; set; } = new List<string>();

    }

    
}

