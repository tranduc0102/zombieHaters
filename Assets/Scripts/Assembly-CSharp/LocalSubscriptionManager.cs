using System;
using IAP;

[Serializable]
public class LocalSubscriptionManager
{
	public DateTime purchaseDate;

	public DateTime expiredDate;

	public DateTime lastClaimedDate;

	public LocalSubscriptionManager()
	{
		purchaseDate = DateTime.MinValue;
		expiredDate = DateTime.MinValue;
		lastClaimedDate = DateTime.MinValue;
	}

	public bool CanDailyClaim()
	{
		if (!InAppManager.Instance.IsSubscribed())
		{
			return false;
		}
		return (TimeManager.CurrentDateTime.Date - lastClaimedDate.Date).TotalDays > 1.0;
	}

	public bool IsSubscribed()
	{
		if (DateTime.Compare(TimeManager.CurrentDateTime, purchaseDate) < 0)
		{
			return false;
		}
		return DateTime.Compare(TimeManager.CurrentDateTime, expiredDate) <= 0;
	}

	public void SetClaimDate()
	{
		lastClaimedDate = TimeManager.CurrentDateTime;
	}

	public DateTime GetNextClaimDate()
	{
		return lastClaimedDate.Date.AddDays(1.0);
	}
}
