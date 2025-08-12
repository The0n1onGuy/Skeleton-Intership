using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.Storage.Pickers;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using PROYECTWEVENTS.CustClasses;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace PROYECTWEVENTS.Z_Proyect_Pages
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class Upload : Page
	{
        private StorageFile selectedPdfFile;
        private StorageFile selectedImageFile;
        private StorageFile selectedImageFile2;
        private RegistrardatosUsuario datosUsuario;
        private RegistrardatosUsuario _userData;
        private Asistentebtnretro _backHandler;

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (e.Parameter is RegistrardatosUsuario usuario)
            {
                datosUsuario = usuario;
            }
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
        public Upload()
		{
			this.InitializeComponent();
            ApplySafeArea();
        }
        private void ApplySafeArea()
        {
            var visibleBounds = ApplicationView.GetForCurrentView().VisibleBounds;

            double topInset = visibleBounds.Top;
            double bottomInset = Window.Current.Bounds.Bottom - visibleBounds.Bottom;

            // Apply padding to the named Grid in XAML
            SafeAreaGrid.Padding = new Thickness(0, topInset, 0, bottomInset);
        }
        private void CustomBacc_Click(object sender, RoutedEventArgs e)
        {
            Frame.GoBack();
            PDFIcon.Visibility = Visibility.Collapsed;
        }
        private async void BrowsePDFButton_Click(object sender, RoutedEventArgs e)
        {
            var picker = new FileOpenPicker();
            var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(Microsoft.UI.Xaml.Window.Current);
            WinRT.Interop.InitializeWithWindow.Initialize(picker, hwnd);

            picker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
            picker.FileTypeFilter.Add(".pdf");

            var file = await picker.PickSingleFileAsync();
            if (file != null)
            {
                selectedPdfFile = file;
                SelectedPDFText.Text = $"Seleccionado: {file.Name}";
                PDFIcon.Visibility = Visibility.Visible;
            }
        }

        private async void BrowseIMGButton_Click(object sender, RoutedEventArgs e)
        {
            var picker = new FileOpenPicker();
            var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(Microsoft.UI.Xaml.Window.Current);
            WinRT.Interop.InitializeWithWindow.Initialize(picker, hwnd);

            picker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            picker.FileTypeFilter.Add(".jpg");
            picker.FileTypeFilter.Add(".jpeg");
            picker.FileTypeFilter.Add(".png");

            var file = await picker.PickSingleFileAsync();
            if (file != null)
            {
                selectedImageFile = file;
                SelectedIMGText.Text = $"Seleccionado: {file.Name}";
                IMGIcon.Visibility = Visibility.Visible;
            }
        }
        private async void BrowseIMGButton2_Click(object sender, RoutedEventArgs e)
        {
            var picker = new FileOpenPicker();
            var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(Microsoft.UI.Xaml.Window.Current);
            WinRT.Interop.InitializeWithWindow.Initialize(picker, hwnd);

            picker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            picker.FileTypeFilter.Add(".jpg");
            picker.FileTypeFilter.Add(".jpeg");
            picker.FileTypeFilter.Add(".png");

            var file = await picker.PickSingleFileAsync();
            if (file != null)
            {
                selectedImageFile2 = file;
                SelectedIMGText2.Text = $"Seleccionado: {file.Name}";
                IMGIcon2.Visibility = Visibility.Visible;
            }
        }
        private static async Task<byte[]> FileToBytesAsync(StorageFile file)
        {
            if (file is null) return Array.Empty<byte>();

            var buffer = await FileIO.ReadBufferAsync(file);
            var bytes = new byte[buffer.Length];
            DataReader.FromBuffer(buffer).ReadBytes(bytes);
            return bytes;
        }
        private async void PaginaS_Click(object sender, RoutedEventArgs e)
        {

            if (selectedPdfFile == null || selectedImageFile == null || selectedImageFile2 == null)
            {
                await new ContentDialog
                {
                    Title = "Faltan archivos",
                    Content = "Debes seleccionar el PDF y las dos imágenes (INE frente y selfie).",
                    CloseButtonText = "OK",
                    XamlRoot = this.XamlRoot
                }.ShowAsync();
                return;
            }
            _userData.CURP = await FileToBytesAsync(selectedPdfFile);
            _userData.IneFront = await FileToBytesAsync(selectedImageFile);
            _userData.IneBack = await FileToBytesAsync(selectedImageFile2);
            Frame.Navigate(typeof(CargarImg), _userData);
        }

    }
}
