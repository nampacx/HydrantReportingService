using CommunityToolkit.Maui.Views;

namespace Reporter.Controls;

public partial class SimplePopup : Popup
{
	public SimplePopup(string text)
	{
        InitializeComponent();

        if (!string.IsNullOrEmpty(text))
            Label.Text = text;
    }
}