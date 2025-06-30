using System.Linq;
using UnityEngine;

public class PlayerPVPGroupController : BasePVPGroupController
{
	protected override void SetSquadTarget()
	{
		GameManager.instance.EnableCameraTarget(base.transform.position);
		for (int i = 0; i < squad.Count; i++)
		{
			squad[i].SetTarget();
			squad[i].groupController = this;
		}
	}

	public override void AddSurvivor()
	{
		base.AddSurvivor();
		squad.Last().SetTarget();
		ParticleSystem particleSystem = Object.Instantiate(PVPManager.Instance.birthOfOurSurvivorFx, squad.Last().transform);
	}
}
