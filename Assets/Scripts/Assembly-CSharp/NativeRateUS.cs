public class NativeRateUS
{
	public string title;

	public string message;

	public string yes;

	public string later;

	public string no;

	public string appLink;

	public NativeRateUS(string title, string message)
	{
		this.title = title;
		this.message = message;
		yes = "Rate app";
		later = "Later";
		no = "No, thanks";
	}

	public NativeRateUS(string title, string message, string yes, string later, string no)
	{
		this.title = title;
		this.message = message;
		this.yes = yes;
		this.later = later;
		this.no = no;
	}

	public void SetAppLink(string _appLink)
	{
		appLink = _appLink;
	}

	public void InitRateUS()
	{
		AndroidRateUsPopUp androidRateUsPopUp = AndroidRateUsPopUp.Create(title, message, yes, later, no);
		androidRateUsPopUp.appLink = appLink;
	}
}
