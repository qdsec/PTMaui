using Microsoft.Extensions.Logging;

#if IOS
using UIKit;
#endif

namespace PeterTours
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiMaps()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if IOS
            // ======== NAVBAR iOS (BACK BLANCO + FONDO FIJO) ========
            var appearance = new UINavigationBarAppearance();
            appearance.ConfigureWithOpaqueBackground();
            appearance.BackgroundColor = UIColor.FromRGB(252, 148, 12); // #fc940c
            appearance.TitleTextAttributes = new UIStringAttributes
            {
                ForegroundColor = UIColor.White
            };
            appearance.LargeTitleTextAttributes = new UIStringAttributes
            {
                ForegroundColor = UIColor.White
            };

            UINavigationBar.Appearance.StandardAppearance = appearance;
            UINavigationBar.Appearance.ScrollEdgeAppearance = appearance;
            UINavigationBar.Appearance.CompactAppearance = appearance;
            UINavigationBar.Appearance.TintColor = UIColor.White; // Flecha Back
#endif

#if DEBUG
            builder.Logging.AddDebug();
#endif

#if IOS
            PeterTours.Platforms.iOS.CustomButtonHandler.Register();
#endif

            return builder.Build();
        }
    }
}
