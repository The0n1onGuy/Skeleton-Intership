using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.Foundation;
using Windows.Foundation.Collections;
using PROYECTWEVENTS.CustClasses;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace PROYECTWEVENTS.Z_Proyect_Pages;
/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class Regdatosperfil : Page
{
    private RegistrardatosUsuario _userData;
    private Asistentebtnretro _backHandler;
    private const string _browserKey = "AIzaSyCJTQgwYMrrNKC6rY5u1buGZ3PDvkvT04s";
    private List<LocationData> _locationData = new List<LocationData> {
    new LocationData
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
    } };
    private static string BuildEmbedHtml(string url) =>
        $"""
        <!DOCTYPE html><html><body style="margin:0;">
        <iframe src="{url}" style="border:0;width:100%;height:100%"></iframe>
        </body></html>
        """;

    private void Mostrar_Mapa(string placeOrCoords)
    {
        var url =
            $"https://www.google.com/maps/embed/v1/place?key={_browserKey}" +
            $"&q={Uri.EscapeDataString(placeOrCoords)}";

        MapaWebView.NavigateToString(BuildEmbedHtml(url));
    }
    private void Actualizar_Mapa()
    {
        // Sólo intentamos si hay al menos estado o municipio
        var partes = new List<string>();

        if (ComboMunicipio.SelectedItem is string mun) partes.Add(mun);
        if (ComboEstado.SelectedItem is string edo) partes.Add(edo);
        if (ComboPais.SelectedItem is string pais) partes.Add(pais);

        if (partes.Count > 0)
            Mostrar_Mapa(string.Join(", ", partes));
    }

    public Regdatosperfil()
    {
        this.InitializeComponent();
        ApplySafeArea();
        ComboPais.ItemsSource = _locationData.Select(ld => ld.Country).ToList();
        
    }    
    

    protected override async void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        _backHandler = new Asistentebtnretro(this);
        _backHandler.Attach();
        _userData = e.Parameter as RegistrardatosUsuario;
        /*
        var file = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Assets/gmps.html"));
        var html = await FileIO.ReadTextAsync(file);
        MapaWebView.NavigateToString(html);
        */
        Mostrar_Mapa("México");

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

        // Navigate to next page (document upload)
        this.Frame.Navigate(typeof(Upload), _userData);

    }

    private void ComboEstado_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var selectedCountry = ComboPais.SelectedItem as string;
        var selectedState = ComboEstado.SelectedItem as string;

        var countryData = _locationData.FirstOrDefault(c => c.Country == selectedCountry);
        var stateData = countryData?.States.FirstOrDefault(s => s.State == selectedState);

        if (stateData != null)
        {
            ComboMunicipio.ItemsSource = stateData.Municipalities;
            ComboMunicipio.SelectedIndex = -1;
        }
        Actualizar_Mapa();
    }
 
    private void ComboPais_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var selectedCountry = ComboPais.SelectedItem as string;
        var countryData = _locationData.FirstOrDefault(c => c.Country == selectedCountry);

        if (countryData != null)
        {
            ComboEstado.ItemsSource = countryData.States.Select(s => s.State).ToList();
            ComboEstado.SelectedIndex = -1;
            ComboMunicipio.ItemsSource = null;
        }
        Actualizar_Mapa();
    }

    private void ComboMunicipio_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        Actualizar_Mapa();
    }
}
