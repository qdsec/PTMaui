using Microsoft.Maui.Handlers;
using UIKit;
using Microsoft.Maui.Controls;

namespace PeterTours.Platforms.iOS
{
    public static class CustomButtonHandler
    {
        public static void Register()
        {
#if IOS
            ButtonHandler.Mapper.AppendToMapping("CustomButtonHandler", (handler, view) =>
            {
                if (handler.PlatformView != null)
                {
                    handler.PlatformView.TitleLabel.LineBreakMode = UILineBreakMode.WordWrap;
                    handler.PlatformView.TitleLabel.Lines = 0; // Permite múltiples líneas
                }
            });
#endif
        }
    }
}
