using System;
using System.Collections.Generic;

namespace StarWars_Aismondo.Models
{
    public class Pianeta
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Gravity { get; set; }
        public string Terrain { get; set; }
        public string Surface_Water { get; set; }
        public string Population { get; set; }

        public List<Pianeta> Results { get; set; }
        public List<string> Residents { get; set; }
    }
}
