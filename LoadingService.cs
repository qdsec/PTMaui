using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PeterTours
{
    public static class LoadingService
    {
        private static ContentPage _loadingOverlay;

        public static void Show(string message = "Cargando...")
        {
            if (_loadingOverlay != null)
                return;

            _loadingOverlay = new ContentPage
            {
                BackgroundColor = Color.FromRgba(0, 0, 0, 0.5),
                Content = new Grid
                {
                    VerticalOptions = LayoutOptions.Center,
                    HorizontalOptions = LayoutOptions.Center,
                    Children =
                    {
                        new VerticalStackLayout
                        {
                            Spacing = 15,
                            VerticalOptions = LayoutOptions.Center,
                            HorizontalOptions = LayoutOptions.Center,
                            Children =
                            {
                                new ActivityIndicator
                                {
                                    IsRunning = true,
                                    Color = Colors.White,
                                    WidthRequest = 50,
                                    HeightRequest = 50
                                },
                                new Label
                                {
                                    Text = message,
                                    TextColor = Colors.White,
                                    FontSize = 16,
                                    HorizontalOptions = LayoutOptions.Center
                                }
                            }
                        }
                    }
                }
            };

            MainThread.BeginInvokeOnMainThread(async () =>
            {
                await Application.Current.MainPage.Navigation.PushModalAsync(_loadingOverlay, false);
            });
        }

        public static void Hide()
        {
            if (_loadingOverlay == null)
                return;

            MainThread.BeginInvokeOnMainThread(async () =>
            {
                await Application.Current.MainPage.Navigation.PopModalAsync(false);
                _loadingOverlay = null;
            });
        }
    }
    //public class LoadingService
    //{
    //    private static ContentPage _loadingOverlay;

    //    public static async Task ShowAsync(string message = "Cargando...")
    //    {
    //        if (_loadingOverlay != null)
    //            return; // ya se está mostrando

    //        _loadingOverlay = new ContentPage
    //        {
    //            BackgroundColor = Color.FromRgba(0, 0, 0, 0.5),
    //            Content = new Grid
    //            {
    //                VerticalOptions = LayoutOptions.Center,
    //                HorizontalOptions = LayoutOptions.Center,
    //                Children =
    //                {
    //                    new VerticalStackLayout
    //                    {
    //                        Spacing = 15,
    //                        VerticalOptions = LayoutOptions.Center,
    //                        HorizontalOptions = LayoutOptions.Center,
    //                        Children =
    //                        {
    //                            new ActivityIndicator
    //                            {
    //                                IsRunning = true,
    //                                Color = Colors.White,
    //                                WidthRequest = 50,
    //                                HeightRequest = 50
    //                            },
    //                            new Label
    //                            {
    //                                Text = message,
    //                                TextColor = Colors.White,
    //                                FontSize = 16,
    //                                HorizontalOptions = LayoutOptions.Center
    //                            }
    //                        }
    //                    }
    //                }
    //            }
    //        };

    //        // Mostrar modal bloqueante
    //        await MainThread.InvokeOnMainThreadAsync(async () =>
    //        {
    //            await Application.Current.MainPage.Navigation.PushModalAsync(_loadingOverlay, false);
    //        });
    //    }

    //    public static async Task HideAsync()
    //    {
    //        if (_loadingOverlay == null)
    //            return;

    //        await MainThread.InvokeOnMainThreadAsync(async () =>
    //        {
    //            await Application.Current.MainPage.Navigation.PopModalAsync(false);
    //            _loadingOverlay = null;
    //        });
    //    }
    //}
}
