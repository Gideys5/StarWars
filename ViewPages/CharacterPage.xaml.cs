using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using StarWars_Aismondo.Models;
using StarWars_Aismondo.Utils;
using Newtonsoft.Json;
using System.Collections.Generic;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace StarWars_Aismondo.ViewPages
{
    public sealed partial class CharacterPage : Page
    {
        private readonly StarWarsApiClient _apiClient = new StarWarsApiClient();
        public ObservableCollection<Personaggio> Personaggi { get; set; } = new ObservableCollection<Personaggio>();
        public ObservableCollection<string> DettagliPersonaggio { get; set; } = new ObservableCollection<string>();



        public CharacterPage()
        {
            InitializeComponent();
            _ = LoadPersonaggiAsync();
        }

        private async Task LoadPersonaggiAsync()
        {
            Personaggi.Clear();
            try
            {
                var personaggi = await _apiClient.GetAllPersonaggiAsync();
                foreach (var personaggio in personaggi)
                {
                    Personaggi.Add(personaggio);
                }

                PersonaggiListView.ItemsSource = Personaggi;
            }
            catch (Exception ex)
            {
                await ShowErrorAsync($"Errore nella chiamata HTTP GET: {ex.Message}");
            }
        }


        private async void PlayerInfo_OnClick(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is Personaggio personaggio)
            {
                DettagliPersonaggio.Clear();
                try
                {
                    var dettagli = await _apiClient.GetPersonaggioDettagliAsync(personaggio.Id);
                    foreach (var dettaglio in dettagli)
                    {
                        DettagliPersonaggio.Add(dettaglio);
                    }
                    DettagliListView.ItemsSource = DettagliPersonaggio;
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
                if (DettagliPersonaggio.Count == 0)
                {
                    await ShowErrorAsync("Nessun personaggio selezionato da salvare.");
                    return;
                }

                await JsonFileManager.SavePersonaggiAsync(DettagliPersonaggio);
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

            DettagliPersonaggio.Clear();
            try
            {

               var dettagli = await _apiClient.GetPersonaggioByName(searchText);
                foreach (var dettaglio in dettagli)
                {
                    DettagliPersonaggio.Add(dettaglio);
                }
                DettagliListView.ItemsSource = DettagliPersonaggio;
            }
            catch (Exception ex)
            {
                await ShowErrorAsync($"Errore caricamento dettagli: {ex.Message}");
            }
        }

        private void PlanetPage_OnClick(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(PlanetPage));

        }

        

        private static async Task ShowErrorAsync(string message) => await new MessageDialog(message).ShowAsync();
        private static async Task ShowMessageAsync(string message) => await new MessageDialog(message).ShowAsync();

    }
}
