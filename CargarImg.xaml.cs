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
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.UI.Xaml.Navigation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage.Pickers;
using Windows.Media.Capture;   
using Windows.ApplicationModel;
using Windows.UI.ViewManagement;
using PROYECTWEVENTS.CustClasses;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace PROYECTWEVENTS.Z_Proyect_Pages;
/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class CargarImg : Page
{
    StorageFile _lastPhoto;
    private Asistentebtnretro _backHandler;
    public CargarImg()
    {
        this.InitializeComponent();
        ApplySafeArea();
    }

    protected override async void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        _backHandler = new Asistentebtnretro(this);
        _backHandler.Attach();
        /*
        var file = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Assets/gmps.html"));
        var html = await FileIO.ReadTextAsync(file);
        MapaWebView.NavigateToString(html);
        */


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

    private async void TomarFoto_Click(object sender, RoutedEventArgs e)
    {
        _backHandler.IsEnabled = false;     
        try
        {
            var captureUI = new CameraCaptureUI();
            captureUI.PhotoSettings.Format = CameraCaptureUIPhotoFormat.Jpeg;
            captureUI.PhotoSettings.AllowCropping = false;
            captureUI.PhotoSettings.MaxResolution = CameraCaptureUIMaxPhotoResolution.HighestAvailable;

            var photo = await captureUI.CaptureFileAsync(CameraCaptureUIMode.Photo);

            if (photo != null)
            {
                await MuestraImagenAsync(photo);
                _lastPhoto = photo;                    // sigues en la MISMA pagina
            }
        }
        catch (Exception ex)
        {
            await new ContentDialog
            {
                Title = "Error al usar la cámara",
                Content = ex.Message,
                CloseButtonText = "OK",
                XamlRoot = this.XamlRoot
            }.ShowAsync();
        }
        finally
        {
            _backHandler.IsEnabled = true;             // ← re-habilita back
        }
    }

    private async void ElegirDeGaleria_Click(object sender, RoutedEventArgs e)
    {
        _backHandler.IsEnabled = false;

        try
        {
            var picker = new FileOpenPicker();
            var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
            WinRT.Interop.InitializeWithWindow.Initialize(picker, hwnd);

            picker.FileTypeFilter.Add(".jpg");
            picker.FileTypeFilter.Add(".jpeg");
            picker.FileTypeFilter.Add(".png");

            var file = await picker.PickSingleFileAsync();
            if (file != null)
            {
                await MuestraImagenAsync(file);
                _lastPhoto = file;
            }
        }
        finally
        {
            _backHandler.IsEnabled = true;
        }
    }

    private async System.Threading.Tasks.Task MuestraImagenAsync(StorageFile file)
    {
        using var stream = await file.OpenAsync(FileAccessMode.Read);
        var bmp = new BitmapImage();
        await bmp.SetSourceAsync(stream);
        CapturedImage.Source = bmp;
    }

    // -------- Obtener fichero listo para subir ----------
    public StorageFile GetPhotoFile() => _lastPhoto;

    private void SubirDoc_Click(object sender, RoutedEventArgs e)
    {
        
    }
}
