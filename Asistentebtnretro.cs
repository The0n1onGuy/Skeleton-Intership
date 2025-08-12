using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Windows.UI.Core;

namespace PROYECTWEVENTS.CustClasses;
public class Asistentebtnretro
{
    private readonly Page _page;
    public bool IsEnabled { get; set; } = true;

    public Asistentebtnretro(Page page) => _page = page;

    public void Attach()
        => SystemNavigationManager.GetForCurrentView().BackRequested += OnBackRequested;

    public void Detach()
        => SystemNavigationManager.GetForCurrentView().BackRequested -= OnBackRequested;

    private void OnBackRequested(object sender, BackRequestedEventArgs e)
    {
        if (!IsEnabled) return;                    // ← clave

        var rootFrame = Window.Current.Content as Frame;
        if (rootFrame != null && rootFrame.CanGoBack)
        {
            e.Handled = true;
            rootFrame.GoBack();
        }
    }
}
