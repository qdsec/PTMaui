namespace PeterTours
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new NavigationPage(new Views.LoginPage());
            //MainPage = new NavigationPage(new step5Cuentas());
            //MainPage = new NavigationPage(new step2ConfirmaPasajeroOtro());
            //CrossFirebasePushNotification.Current.OnTokenRefresh += (s, p) =>
            //{
            //    System.Diagnostics.Debug.WriteLine($"TOKEN : {p.Token}   FIN");
            //};
            //CrossFirebasePushNotification.Current.OnNotificationReceived += (s, p) =>
            //{

            //    System.Diagnostics.Debug.WriteLine("Received");
            //    foreach (var data in p.Data)
            //    {
            //        System.Diagnostics.Debug.WriteLine($"{data.Key} : {data.Value}");
            //    }

            //};
            //CrossFirebasePushNotification.Current.OnNotificationOpened += (s, p) =>
            //{
            //    System.Diagnostics.Debug.WriteLine("Opened");
            //    foreach (var data in p.Data)
            //    {
            //        System.Diagnostics.Debug.WriteLine($"{data.Key} : {data.Value}");
            //    }


            //};
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
