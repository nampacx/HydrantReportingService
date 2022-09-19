namespace Reporter;

public partial class StartPage : ContentPage
{
	public StartPage()
	{
		InitializeComponent();
	}

	private async void Button_Clicked(object sender, EventArgs e)
	{
		await Shell.Current.GoToAsync("newreport");
        //await Navigation.PushAsync(new MainPage());
	}
}