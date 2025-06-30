using System.Collections;
using System.Linq;
using UnityEngine;

public class BotPVPGroupController : BasePVPGroupController
{
	[SerializeField]
	private BotTarget botTarget;

	protected override void Start()
	{
		base.Start();
		StartCoroutine(GroupManager());
	}

	protected override void SetSquadTarget()
	{
		botTarget = Object.Instantiate(botTarget, base.transform);
		botTarget.SetController(this);
		for (int i = 0; i < squad.Count; i++)
		{
			squad[i].SetTarget(botTarget.transform);
			squad[i].groupController = this;
		}
	}

	public override void AddSurvivor()
	{
		base.AddSurvivor();
		squad.Last().SetTarget(botTarget.transform);
		ParticleSystem particleSystem = Object.Instantiate(PVPManager.Instance.birthOfEnemieSurvivorFx, squad.Last().transform);
	}

	private IEnumerator GroupManager()
	{
		bool needToResetTarget = false;
		while (true)
		{
			yield return new WaitForSeconds(1f);
			CheckGroupParts();
			while (groups.Count > 1)
			{
				needToResetTarget = true;
				int biggest = GetBiggestGroupIndex();
				int smallest = GetSmallestGroupIndex();
				for (int i = 0; i < groups.Count; i++)
				{
					for (int j = 0; j < groups[i].survivors.Count; j++)
					{
						groups[i].survivors[j].SetAgentDestination((i != biggest) ? groups[biggest].GetPartGroupCenter() : groups[smallest].GetPartGroupCenter());
					}
				}
				yield return new WaitForSeconds(1f);
				CheckGroupParts();
			}
			if (needToResetTarget)
			{
				botTarget.transform.position = GetGroupCenter();
				needToResetTarget = false;
			}
			for (int k = 0; k < groups.Count; k++)
			{
				for (int l = 0; l < groups[k].survivors.Count; l++)
				{
					groups[k].survivors[l].ResetAgent();
				}
			}
		}
	}

	private int GetBiggestGroupIndex()
	{
		int result = 0;
		int num = -1;
		for (int i = 0; i < groups.Count; i++)
		{
			if (groups[i].survivors.Count > num)
			{
				num = groups[i].survivors.Count;
				result = i;
			}
		}
		return result;
	}

	private int GetSmallestGroupIndex()
	{
		int result = 0;
		int num = squad.Count + 1;
		for (int i = 0; i < groups.Count; i++)
		{
			if (groups[i].survivors.Count < num)
			{
				num = groups[i].survivors.Count;
				result = i;
			}
		}
		return result;
	}
}
