using Foundation;
using UIKit;

namespace PeterTours
{
    [Register("AppDelegate")]
    public class AppDelegate : MauiUIApplicationDelegate
    {
        protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            //// Forzar modo claro
            //if (UIDevice.CurrentDevice.CheckSystemVersion(13, 0))
            //{
            //    Window.OverrideUserInterfaceStyle = UIUserInterfaceStyle.Light;
            //}

            return base.FinishedLaunching(app, options);
        }

    }
}
