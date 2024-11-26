using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Windows.Storage;

namespace StarWars_Aismondo.Utils
{
    public static class JsonFileManager
    {
        private const string FilePersonaggi = "PersonaggiStarWars.json";
        private const string FilePianeti = "PianetiStarWars.json";


        public static async Task SavePersonaggiAsync(IEnumerable<string> dettagliPersonaggio)
        {
            var localFolder = ApplicationData.Current.LocalFolder;

            var file = await localFolder.CreateFileAsync(FilePersonaggi, CreationCollisionOption.OpenIfExists);
            var existingContent = await FileIO.ReadTextAsync(file);
            var personaggiSalvati = JsonConvert.DeserializeObject<List<List<string>>>(existingContent) ?? new List<List<string>>();

            personaggiSalvati.Add(new List<string>(dettagliPersonaggio));
            var jsonData = JsonConvert.SerializeObject(personaggiSalvati, Formatting.Indented);

            await FileIO.WriteTextAsync(file, jsonData);
        }
        
        public static async Task SavePianetiAsync(IEnumerable<string> dettagliPianeta)
        {
            var localFolder = ApplicationData.Current.LocalFolder;

            var file = await localFolder.CreateFileAsync(FilePianeti, CreationCollisionOption.OpenIfExists);
            var existingContent = await FileIO.ReadTextAsync(file);
            var pianetiSalvati = JsonConvert.DeserializeObject<List<List<string>>>(existingContent) ?? new List<List<string>>();

            pianetiSalvati.Add(new List<string>(dettagliPianeta));
            var jsonData = JsonConvert.SerializeObject(pianetiSalvati, Formatting.Indented);

            await FileIO.WriteTextAsync(file, jsonData);
        }
    }
}
