using System;
using System.Collections.ObjectModel;
using StarWars_Aismondo.Models;
using System.Threading.Tasks;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

using StarWars_Aismondo.Utils;
using Newtonsoft.Json;
using System.Collections.Generic;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace StarWars_Aismondo.ViewPages
{
    public sealed partial class PlanetPage : Page
    {
        private readonly StarWarsApiPlanet _apiPlanet = new StarWarsApiPlanet();
        public ObservableCollection<Pianeta> Pianeti { get; set; } = new ObservableCollection<Pianeta>();
        public ObservableCollection<string> DettagliPianeta { get; set; } = new ObservableCollection<string>();

        public PlanetPage()
        {
            InitializeComponent();
            _ = LoadPianetiAsync();
        }

        private async Task LoadPianetiAsync()
        {
            Pianeti.Clear();
            try
            {
                var pianeti = await _apiPlanet.GetAllPianetiAsync();
                foreach (var pianeta in pianeti)
                {
                    Pianeti.Add(pianeta);
                }

                PianetiListView.ItemsSource = Pianeti;
            }
            catch (Exception ex)
            {
                await ShowErrorAsync($"Errore nella chiamata HTTP GET: {ex.Message}");
            }
        }

        private async void PlanetInfo_OnClick(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is Pianeta pianeta)
            {
                DettagliPianeta.Clear();
                try
                {
                    var dettagli = await _apiPlanet.GetPianetaDettagliAsync(pianeta.Id);
                    foreach (var dettaglio in dettagli)
                    {
                        DettagliPianeta.Add(dettaglio);
                    }
                    DettagliListView.ItemsSource = DettagliPianeta;
                }
                catch (Exception ex)
                {
                    await ShowErrorAsync($"Errore caricamento dettagli: {ex.Message}");
                }
            }
        }

        private async void SaveInJson_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                if (DettagliPianeta.Count == 0)
                {
                    await ShowErrorAsync("Nessun pianeta selezionato da salvare.");
                    return;
                }

                await JsonFileManager.SavePianetiAsync(DettagliPianeta);
                await ShowMessageAsync("Informazioni salvate con successo!");
            }
            catch (Exception ex)
            {
                await ShowErrorAsync($"Errore durante il salvataggio: {ex.Message}");
            }
        }

        private async void SearchByName_OnClick(object sender, RoutedEventArgs e)
        {
            string searchText = Text.Text.Trim().ToLower();

            DettagliPianeta.Clear();
            try
            {

                var dettagli = await _apiPlanet.GetPianetaByName(searchText);
                foreach (var dettaglio in dettagli)
                {
                    DettagliPianeta.Add(dettaglio);
                }
                DettagliListView.ItemsSource = DettagliPianeta;
            }
            catch (Exception ex)
            {
                await ShowErrorAsync($"Errore caricamento dettagli: {ex.Message}");
            }
        }


        private void NamePage_OnClick(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(CharacterPage));

        }

        private static async Task ShowErrorAsync(string message) => await new MessageDialog(message).ShowAsync();
        private static async Task ShowMessageAsync(string message) => await new MessageDialog(message).ShowAsync();
        
    }

}

