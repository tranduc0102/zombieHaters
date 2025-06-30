public class NativeDialog
{
	private string title;

	private string message;

	private string yesButton;

	private string noButton;

	public string urlString;

	public NativeDialog(string title, string message)
	{
		this.title = title;
		this.message = message;
		yesButton = "Yes";
		noButton = "No";
	}

	public NativeDialog(string title, string message, string yesButtonText, string noButtonText)
	{
		this.title = title;
		this.message = message;
		yesButton = yesButtonText;
		noButton = noButtonText;
	}

	public void SetUrlString(string urlString)
	{
		this.urlString = urlString;
	}

	public void init()
	{
		AndroidDialog androidDialog = AndroidDialog.Create(title, message, yesButton, noButton);
		androidDialog.urlString = urlString;
	}
}
