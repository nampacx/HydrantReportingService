namespace Reporter
{
    public partial class App : Application
    {
        public App()
        {
            Current.UserAppTheme = AppTheme.Light;
            InitializeComponent();

            MainPage = new AppShell();

        }
    }
}