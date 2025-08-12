using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text.Json;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using PROYECTWEVENTS.CustClasses;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.ViewManagement;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace PROYECTWEVENTS.Z_Proyect_Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class PROFILEVIEW : Page
    {
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            SystemNavigationManager.GetForCurrentView().BackRequested += OnBackRequested;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)

        {
            base.OnNavigatedFrom(e);
            SystemNavigationManager.GetForCurrentView().BackRequested -= OnBackRequested;
        }

        private void OnBackRequested(object? sender, BackRequestedEventArgs e)

        {
            var rootFrame = (Microsoft.UI.Xaml.Window.Current.Content as Frame);

            if (rootFrame != null && rootFrame.CanGoBack)

            {
                e.Handled = true;

                rootFrame.GoBack();
            }

        }
        public PROFILEVIEW()
        {
            this.InitializeComponent();
            ApplySafeArea();
            CargarDatos();

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
        private async void ViewFile_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is string fileUrl)
            {
                var uri = new Uri(fileUrl);
                await Launcher.LaunchUriAsync(uri);
            }
        }


        private async void CargarDatos()
        {
            try
            {
                HttpClient client = new HttpClient();

                var values = new Dictionary<string, string>
        {
            { "token_operacion", "2" },
            { "id", "6" } // You can change this to any valid user ID
        };

                var content = new FormUrlEncodedContent(values);
                var response = await client.PostAsync("https://clouddatacancun.com/registrousersyarchs.php", content);

                var responseString = await response.Content.ReadAsStringAsync();
                var doc = JsonDocument.Parse(responseString);
                var root = doc.RootElement;

                if (root.GetProperty("status").GetString() == "success")
                {
                    var user = root.GetProperty("usuario");
                    NombreTxt.Text = user.GetProperty("nombres").GetString();
                    ApellidosTxt.Text = user.GetProperty("apellidos").GetString();
                    CorreoTxt.Text = user.GetProperty("correo").GetString();
                    TelefonoTxt.Text = user.GetProperty("telefono").GetString();
                    ContrasenaTxt.Password = user.GetProperty("contrasena").GetString();

                    var archivos = root.GetProperty("archivos");
                    ListaArch.Items.Clear();
                    

                    foreach (var file in archivos.EnumerateArray())
                    {
                        if (file.TryGetProperty("id", out var idProp))
                        {
                            ListaArch.Items.Add(new ArchivoItem
                            {
                                FileId = int.Parse(idProp.GetString()), // FIX HERE
                                Filename = file.GetProperty("archnombre").GetString()
                            });
                        }
                    }

                    System.Diagnostics.Debug.WriteLine($"Archivos encontrados: {ListaArch.Items.Count}");


                }
                else
                {
                    await new ContentDialog
                    {
                        Title = "Error",
                        Content = root.GetProperty("message").GetString(),
                        CloseButtonText = "OK",
                        XamlRoot = this.XamlRoot
                    }.ShowAsync();
                }
            }
            catch (Exception ex)
            {
                await new ContentDialog
                {
                    Title = "Error",
                    Content = ex.Message,
                    CloseButtonText = "OK",
                    XamlRoot = this.XamlRoot
                }.ShowAsync();
            }
        }
    }
    }
