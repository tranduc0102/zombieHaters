namespace EPPZ.Cloud.Plugin
{
	public interface ICloud
	{
		void _CloudDidChange(string message);

		void _OnCloudChange(string[] changedKeys, ChangeReason changeReason);
	}
}
