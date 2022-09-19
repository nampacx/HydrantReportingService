namespace Reporter
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute("start", typeof(StartPage));
            Routing.RegisterRoute("newreport", typeof(MainPage));
        }
    }
}