using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using StarWars_Aismondo.Models;
using HttpClient = System.Net.Http.HttpClient;


namespace StarWars_Aismondo.Utils
{
    public class StarWarsApiClient
    {
        private readonly HttpClient _httpClient = new HttpClient();
        private List<string> dettagli = new List<string>();

        public async Task<List<Personaggio>> GetAllPersonaggiAsync()
        {
            var response = await _httpClient.GetAsync("https://swapi.dev/api/people/");
            response.EnsureSuccessStatusCode();

            var jsonData = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<Personaggio>(jsonData);

            int id = 1;
            var personaggi = new List<Personaggio>();
            foreach (var personaggio in result.Results)
            {
                personaggio.Id = id++;
                personaggi.Add(personaggio);
            }

            return personaggi;
        }
       

        public async Task<List<string>> GetPersonaggioDettagliAsync(int id)
        {
            dettagli.Clear();
            var response = await _httpClient.GetAsync($"https://swapi.dev/api/people/{id}");
            response.EnsureSuccessStatusCode();

            var jsonData = await response.Content.ReadAsStringAsync();
            var personaggio = JsonConvert.DeserializeObject<Personaggio>(jsonData);


            await GetDettagli(dettagli, personaggio);
            return dettagli;
        }



        public async Task<List<string>> GetPersonaggioByName(string nome)
        {
            dettagli.Clear();
            string searchUrl = $"https://swapi.dev/api/people/?search={nome}";
            var response = await _httpClient.GetAsync(searchUrl);
            response.EnsureSuccessStatusCode();

            var jsonData = await response.Content.ReadAsStringAsync();
            var searchResult = JsonConvert.DeserializeObject<SearchResultPersonaggio>(jsonData);
            var personaggio = searchResult.ResultsPersonaggio.FirstOrDefault();

            if (personaggio == null)
            {
                dettagli.Add("Nessun personaggio trovato");
                return dettagli;
            }

            await GetDettagli(dettagli, personaggio);
            return dettagli;
        }

        private async Task GetDettagli(List<string> dettagli, Personaggio personaggio)
        {
            dettagli.Add($"Nome: {personaggio.Name}");
            dettagli.Add($"Altezza: {personaggio.Height}");
            dettagli.Add($"Massa: {personaggio.Mass}");
            dettagli.Add($"Colore della pelle: {personaggio.Skin_Color}");
            dettagli.Add($"Anno di nascita: {personaggio.Birth_Year}");
            dettagli.Add($"Genere: {personaggio.Gender}");


            var pianeta = await GetEntityByUrlAsync<Pianeta>(personaggio.Homeworld);
            dettagli.Add($"Pianeta: {pianeta.Name}");

            if (personaggio.Vehicles.Count == 0)
            {
                dettagli.Add("VEICOLI: Il personaggio non possiede veicoli");
            }
            else
            {
                foreach (var vehicleUrl in personaggio.Vehicles)
                {
                    var veicolo = await GetEntityByUrlAsync<Veicolo>(vehicleUrl);
                    dettagli.Add($"VEICOLO {(veicolo.Name).ToUpper()}");
                    dettagli.Add(
                        $"Nome: {veicolo.Name} - " +
                        $"Modello: {veicolo.Model} - " +
                        $"Classe: {veicolo.Vehicle_Class} - " +
                        $"Massima Velocità: {veicolo.Max_Atmosphering_Speed}");
                }
            }

            if (personaggio.Starships.Count == 0)
            {
                dettagli.Add("ASTRONAVI: Il personaggio non possiede astronavi");
            }
            else
            {
                foreach (var starshipUrl in personaggio.Starships)
                {
                    var astronave = await GetEntityByUrlAsync<Astronave>(starshipUrl);
                    dettagli.Add($"ASTRONAVE {(astronave.Name).ToUpper()} ");
                    dettagli.Add(
                        $"Nome: {astronave.Name} - " +
                        $"Modello: {astronave.Model}  - " +
                        $"Produttore: {astronave.Manufacturer} - " +
                        $"Classe: {astronave.Starship_Class}  - " +
                        $"Massima Velicità: {astronave.Max_Atmosphering_Speed}");
                }
            }
        }
        private async Task<T> GetEntityByUrlAsync<T>(string url)
        {
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            var jsonData = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(jsonData);
        }
    }
    public class SearchResultPersonaggio
    {
        [JsonProperty("results")]
        public List<Personaggio> ResultsPersonaggio { get; set; }


    }

}



