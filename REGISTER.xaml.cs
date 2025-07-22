using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Headers;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Text.Json;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using MySqlConnector;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using PROYECTWEVENTS.CustClasses;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace PROYECTWEVENTS.Z_Proyect_Pages
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class REGISTER : Page
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
        public REGISTER()
		{
			this.InitializeComponent();
            ApplySafeArea();
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
            var contrasenia = ContrasenaTxt.Text;
            var confirmar = ConfirmarContrasenaTxt.Text;

            PasswordMismatchWarning.Visibility = Visibility.Collapsed;
            PasswordPolicyWarning.Visibility = Visibility.Collapsed;
            bool hasUpper = contrasenia.Any(char.IsUpper);
            bool hasDigit = contrasenia.Any(char.IsDigit);
            bool isValid = true;

            if (contrasenia != confirmar)
            {
                PasswordMismatchWarning.Visibility = Visibility.Visible;
                isValid = false;

            }
            if (!(hasUpper && hasDigit))
            {
                PasswordPolicyWarning.Visibility = Visibility.Visible;
                isValid = false;
            }
            if (!isValid) {
                return;
            }
                
            var userData = new RegistrardatosUsuario
            {
                Nombres = NombreTxt.Text,
                Apellidos = ApellidosTxt.Text,
                Correo = CorreoTxt.Text,
                Telefono = TelefonoTxt.Text,
                Contrasena = ContrasenaTxt.Text
            };

            this.Frame.Navigate(typeof(Upload), userData);
        }
    }
}
