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
public sealed partial class Regusuario : Page
{

    public Regusuario()
    {
        this.InitializeComponent();
        ApplySafeArea();
    }
    private Asistentebtnretro _backHandler;

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        _backHandler = new Asistentebtnretro(this);
        _backHandler.Attach();
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

        var contrasenia = ContrasenaTxt.Password;
        var confirmar = ConfirmarContrasenaTxt.Password;

        PasswordMismatchWarning.Visibility = Visibility.Collapsed;
        PasswordPolicyWarning.Visibility = Visibility.Collapsed;
        bool hasUpper = contrasenia.Any(char.IsUpper);
        bool hasDigit = contrasenia.Any(char.IsDigit);
        bool isValid = true;
        if (string.IsNullOrWhiteSpace(NombreTxt.Text) ||
        string.IsNullOrWhiteSpace(CorreoTxt.Text) ||
        string.IsNullOrWhiteSpace(TelefonoTxt.Text) ||
        string.IsNullOrWhiteSpace(contrasenia) ||
        string.IsNullOrWhiteSpace(confirmar))
        {
            _ = new ContentDialog
            {
                Title = "Campos vacíos",
                Content = "Por favor, completa todos los campos antes de continuar.",
                CloseButtonText = "OK",
                XamlRoot = this.XamlRoot
            }.ShowAsync();
            return;
        }
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
        if (!isValid)
        {
            return;
        }

        var userData = new RegistrardatosUsuario
        {
            Nombres = NombreTxt.Text,
            Correo = CorreoTxt.Text,
            Telefono = "0",
            Contrasena = ContrasenaTxt.Password
        };

        this.Frame.Navigate(typeof(RDPOSM), userData);
    }
}
