using UnityEngine;

public class CameraTarget : MonoBehaviour
{
	public static CameraTarget instance;

	private void Awake()
	{
		instance = this;
	}

	private void Update()
	{
		if (GameManager.instance.currentGameMode == GameManager.GameModes.PVP)
		{
			Vector3 groupCenter;
			if ((bool)PVPManager.Instance && PVPManager.pvpPlayers.Count > 0 && PVPManager.pvpPlayers[0].TryToGetGroupCenter(out groupCenter))
			{
				base.transform.position = PVPManager.pvpPlayers[0].GetGroupCenter();
			}
		}
		else
		{
			if (GameManager.instance.survivors.Count <= 0)
			{
				return;
			}
			Vector3 vector = default(Vector3);
			foreach (SurvivorHuman survivor in GameManager.instance.survivors)
			{
				if (!(survivor == null))
				{
					vector += survivor.transform.position;
				}
			}
			base.transform.position = vector / GameManager.instance.survivors.Count;
		}
	}
}
