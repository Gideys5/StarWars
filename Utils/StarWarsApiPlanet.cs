using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using StarWars_Aismondo.Models;
using HttpClient = System.Net.Http.HttpClient;

namespace StarWars_Aismondo.Utils
{
    public class StarWarsApiPlanet
    {
        private readonly HttpClient _httpClient = new HttpClient();
        private List<string> dettagli = new List<string>();

        public async Task<List<Pianeta>> GetAllPianetiAsync()
        {
            var response = await _httpClient.GetAsync("https://swapi.dev/api/planets/");
            response.EnsureSuccessStatusCode();

            var jsonData = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<Pianeta>(jsonData);

            int id = 1;
            var pianeti = new List<Pianeta>();
            foreach (var pianeta in result.Results)
            {
                pianeta.Id = id++;
                pianeti.Add(pianeta);
            }

            return pianeti;
        }

        public async Task<List<string>> GetPianetaDettagliAsync(int id)
        {
            dettagli.Clear();
            var response = await _httpClient.GetAsync($"https://swapi.dev/api/planets/{id}");
            response.EnsureSuccessStatusCode();

            var jsonData = await response.Content.ReadAsStringAsync();
            var pianeta = JsonConvert.DeserializeObject<Pianeta>(jsonData);



            await GetDettagliPianeta(dettagli, pianeta);

            return dettagli;
        }




        public async Task<List<string>> GetPianetaByName(string nome)
        {
            dettagli.Clear();
            string searchUrl = $"https://swapi.dev/api/planets/?search={nome}";
            var response = await _httpClient.GetAsync(searchUrl);
            response.EnsureSuccessStatusCode();

            var jsonData = await response.Content.ReadAsStringAsync();
            var searchResult = JsonConvert.DeserializeObject<SearchResultPianeta>(jsonData);
            var pianeta = searchResult.ResultsPianeta.FirstOrDefault();

            if (pianeta == null)
            {
                dettagli.Add("Nessun pianeta trovato");
                return dettagli;
            }

            await GetDettagliPianeta(dettagli, pianeta);
            return dettagli;
        }


        private async Task<List<string>> GetDettagliPianeta(List<string> dettagli, Pianeta pianeta)
        {
            dettagli.Add($"Nome: {pianeta.Name}");
            dettagli.Add($"Gravità: {pianeta.Gravity}");
            dettagli.Add($"Terreno: {pianeta.Terrain}");
            dettagli.Add($"Superficie d'acqua: {pianeta.Surface_Water}");
            dettagli.Add($"Popolazione: {pianeta.Population}");

            if (pianeta.Residents.Count == 0)
            {
                dettagli.Add("Nessun abitante degno di nota per questo pianeta");
            }
            else
            {
                dettagli.Add($"ABITANTI DI {(pianeta.Name).ToUpper()} :");
                foreach (var residentUrl in pianeta.Residents)
                {
                    var residentResponse = await _httpClient.GetAsync(residentUrl);
                    residentResponse.EnsureSuccessStatusCode();

                    if (residentResponse.IsSuccessStatusCode)
                    {
                        var residentData = await residentResponse.Content.ReadAsStringAsync();
                        var resident = JsonConvert.DeserializeObject<Personaggio>(residentData);
                        dettagli.Add($"- {resident.Name}");
                    }

                }
            }

            return dettagli;
        }
    }
    public class SearchResultPianeta
    {
        [JsonProperty("results")]
        public List<Pianeta> ResultsPianeta { get; set; }


    }
}

