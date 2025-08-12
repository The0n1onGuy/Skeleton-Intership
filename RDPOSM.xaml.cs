using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Json;
using System.Runtime.InteropServices.WindowsRuntime;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Documents;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using PROYECTWEVENTS.CustClasses;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.ViewManagement;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace PROYECTWEVENTS.Z_Proyect_Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class RDPOSM : Page
    {
        private RegistrardatosUsuario _userData;
        private Asistentebtnretro _backHandler;
        private string _leafletTemplate;
        private decimal? _currentLat;
        private decimal? _currentLon;
        private readonly GeoService _geo = new();
        
        private Dictionary<string, List<string>> _cacheStates = new();
        private Dictionary<(string, string), List<string>> _cacheCities = new();

        public record CountryDto(string Name);
        public record CountriesResp(List<CountryDto> Data);

        public record StateDto(string Name);
        public record StatesData(List<StateDto> States);
        public record StatesResp(StatesData Data);

        public record CitiesResp(List<string> Data);

        /* LISTA FIJA DE MEXICO (PRUEBAS)
        private List<LocationData> _locationData = new List<LocationData> {    new LocationData
    {
        Country = "Mexico",
        States = new List<StateData>
        {
            new StateData
            {
                State = "Aguascalientes",
                Municipalities = new List<string> { "Aguascalientes", "Asientos", "Calvillo", "Jesús María", "Pabellón de Arteaga" }
            },
            new StateData
            {
                State = "Baja California",
                Municipalities = new List<string> { "Mexicali", "Tijuana", "Ensenada", "Tecate", "Playas de Rosarito" }
            },
            new StateData
            {
                State = "Ciudad de México",
                Municipalities = new List<string> { "Álvaro Obregón", "Azcapotzalco", "Benito Juárez", "Coyoacán", "Cuauhtémoc" }
            },
            new StateData
            {
                State = "Jalisco",
                Municipalities = new List<string> { "Guadalajara", "Zapopan", "Tlaquepaque", "Tonalá", "Puerto Vallarta" }
            },
            new StateData
            {
                State = "Nuevo León",
                Municipalities = new List<string> { "Monterrey", "San Nicolás", "Guadalupe", "Apodaca", "Santa Catarina" }
            },
            new StateData
            {
                State = "Quintana Roo",
                Municipalities = new List<string> { "Benito Juárez", "Cozumel", "Solidaridad", "Tulum", "Felipe Carrillo Puerto" }
            },
            new StateData
            {
                State = "Yucatán",
                Municipalities = new List<string> { "Mérida", "Valladolid", "Tizimín", "Progreso", "Izamal" }
            }
        }
    } };*/

        public RDPOSM()
		{
			this.InitializeComponent();
            ApplySafeArea();
            _ = LoadCountriesAsync();
        }
        

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            _backHandler = new Asistentebtnretro(this);
            _backHandler.Attach();
            _userData = e.Parameter as RegistrardatosUsuario;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            _backHandler?.Detach();
        }

        private void ApplySafeArea()
        {
            var visibleBounds = ApplicationView.GetForCurrentView().VisibleBounds;

            double topInset = visibleBounds.Top;
            double bottomInset = Window.Current.Bounds.Bottom - visibleBounds.Bottom;

            SafeAreaGrid.Padding = new Thickness(0, topInset, 0, bottomInset);
        }


        private void CustomBacc_Click(object sender, RoutedEventArgs e)
        {
            Frame.GoBack();
        }

        private void NextPage_Click(object sender, RoutedEventArgs e)
        {
            _userData.Nombres = NombreTxt.Text;
            _userData.Apellidos = ApellidosTxt.Text;
            _userData.Pais = ComboPais.SelectedItem.ToString();
            _userData.Estado = ComboEstado.SelectedItem.ToString();
            _userData.Municipio = ComboMunicipio.SelectedItem.ToString();
            _userData.CodigoPostal = CodigoPTxt.Text;
            if (_currentLat.HasValue && _currentLon.HasValue)
            {
                _userData.Lat = _currentLat.Value;
                _userData.Lon = _currentLon.Value;
            }
            else
            {
                ContentDialog dlg = new()
                {
                    Title = "Ubicacion no definida",
                    Content = "Selecciona un punto en el mapa o escribe el codigo postal antes de continuar.",
                    CloseButtonText = "Aceptar"
                };
                _ = dlg.ShowAsync();
                return;
            }

            this.Frame.Navigate(typeof(Upload), _userData);

        }
       


        public class GeoService
        {
            private readonly HttpClient _http = new()
            {
                BaseAddress = new Uri("https://countriesnow.space/")
            };

            public async Task<IEnumerable<string>> GetCountriesAsync()
            {
                var resp = await _http.GetFromJsonAsync<CountriesResp>(
                             "api/v0.1/countries/positions");
                return resp?.Data.Select(c => c.Name) ?? Enumerable.Empty<string>();
            }

            public async Task<IEnumerable<string>> GetStatesAsync(string country)
            {
                var body = JsonContent.Create(new { country });
                var resp = await _http.PostAsync("api/v0.1/countries/states", body);
                var data = await resp.Content.ReadFromJsonAsync<StatesResp>();
                return data?.Data.States.Select(s => s.Name) ?? Enumerable.Empty<string>();
            }

            public async Task<IEnumerable<string>> GetCitiesAsync(
                                                  string country, string state)
            {
                var body = JsonContent.Create(new { country, state });
                var resp = await _http.PostAsync(
                                "api/v0.1/countries/state/cities", body);
                var data = await resp.Content.ReadFromJsonAsync<CitiesResp>();
                return data?.Data ?? Enumerable.Empty<string>();
            }
        }
        private async Task LoadHtmlTemplateAsync()
        {
            if (_leafletTemplate is null)
            {
                var file = await StorageFile
                    .GetFileFromApplicationUriAsync(
                        new Uri("ms-appx:///Assets/map.html"));
                _leafletTemplate = await FileIO.ReadTextAsync(file);
            }
        }
        private async Task LoadCountriesAsync()
        {
            ComboPais.ItemsSource = await _geo.GetCountriesAsync();
        }
        private async Task<(double lat, double lon)?> GeocodeAsync(string query)
        {
            using var client = new HttpClient();
            client.DefaultRequestHeaders.UserAgent.ParseAdd("UnoSampleApp/1.0");

            var url = $"https://nominatim.openstreetmap.org/search" +
                      $"?q={Uri.EscapeDataString(query)}&format=json&limit=1";

            var result = await client.GetFromJsonAsync<List<NominatimResp>>(url);
            if (result?.Count > 0 &&
                double.TryParse(result[0].lat, out var lat) &&
                double.TryParse(result[0].lon, out var lon))
                return (lat, lon);

            return null;
        }

        private record NominatimResp(string lat, string lon);

        private async Task ShowMapAsync(string place)
        {
            await LoadHtmlTemplateAsync();

            var coords = await GeocodeAsync(place);
            if (coords is null)
            {
                MapaWebView.NavigateToString("<html><body>No se encontró la ubicación.</body></html>");
                return;
            }
            _currentLat = (decimal)Math.Round(coords.Value.lat, 6);
            _currentLon = (decimal)Math.Round(coords.Value.lon, 6);

            var html = _leafletTemplate
                .Replace("{{LAT}}", coords.Value.lat.ToString("F6", System.Globalization.CultureInfo.InvariantCulture))
                .Replace("{{LON}}", coords.Value.lon.ToString("F6", System.Globalization.CultureInfo.InvariantCulture));

            MapaWebView.NavigateToString(html);
        }

        private async  void ComboEstado_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ComboPais.SelectedItem is not string pais ||
        ComboEstado.SelectedItem is not string estado) return;

            var key = (pais, estado);
            if (!_cacheCities.TryGetValue(key, out var ciudades))
            {
                ciudades = (await _geo.GetCitiesAsync(pais, estado)).ToList();
                _cacheCities[key] = ciudades;
            }
            ComboMunicipio.ItemsSource = ciudades;
        }

        private async void ComboPais_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboEstado.ItemsSource = null;
            ComboMunicipio.ItemsSource = null;

            if (ComboPais.SelectedItem is not string pais) return;

            if (!_cacheStates.TryGetValue(pais, out var estados))
            {
                estados = (await _geo.GetStatesAsync(pais)).ToList();
                _cacheStates[pais] = estados;                        // cache
            }
            ComboEstado.ItemsSource = estados;
        }

        private async void ComboMunicipio_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string codigoP = CodigoPTxt.Text;
            if (ComboPais.SelectedItem is string pais &&
        ComboEstado.SelectedItem is string estado &&
        ComboMunicipio.SelectedItem is string mun)
            {
                string consulta = $"{mun}, {estado}, {pais},{codigoP}";
                await ShowMapAsync(consulta);
            }
        }
        private async void CodigoPTxt_TextChanged(object sender, TextChangedEventArgs e)
        {
            string cp = CodigoPTxt.Text.Trim();

            // Espera al menos 4-5 dígitos para evitar peticiones inútiles
            if (cp.Length < 4) return;

            // Toma lo que ya haya elegido el usuario (si no hay, usa valores vacíos)
            string pais = ComboPais.SelectedItem as string ?? "";
            string estado = ComboEstado.SelectedItem as string ?? "";
            string municipio = ComboMunicipio.SelectedItem as string ?? "";

            /*  Orden de precisión:
                1) Código postal                 (siempre lo ponemos)
                2) Municipio / Estado / País (si existen)              */
            string consulta = string.Join(", ",
                              new[] { cp, municipio, estado, pais }
                              .Where(p => !string.IsNullOrWhiteSpace(p)));

            await ShowMapAsync(consulta);
        }

    }
}
