using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class BasePVPGroupController : MonoBehaviour
{
	[SerializeField]
	private GameObject topUI;

	[SerializeField]
	private Text textLevel;

	[SerializeField]
	private Transform survivorsParent;

	[HideInInspector]
	public int pvpPlayerIndex;

	private LayerMask obstacle;

	[HideInInspector]
	public List<PVPSoldierSurvivor> squad = new List<PVPSoldierSurvivor>();

	[HideInInspector]
	public int level;

	public float lastHitTime;

	private int baseLevel = 3;

	private float maxLootCount;

	public List<GroupPartInfo> groups = new List<GroupPartInfo>();

	protected virtual void Start()
	{
		obstacle = (1 << LayerMask.NameToLayer("Obstacle")) | (1 << LayerMask.NameToLayer("Car")) | (1 << LayerMask.NameToLayer("LootBox"));
	}

	public virtual void SpawnSquad(Vector3 startPointPos, int placeInList)
	{
		level = 0;
		pvpPlayerIndex = placeInList;
		for (int i = 0; i < baseLevel; i++)
		{
			float num = Random.Range(-0.5f, 0.5f);
			float num2 = Random.Range(-0.5f, 0.5f);
			level++;
			squad.Add(Object.Instantiate(PVPManager.Instance.GetSoldierByLevel(pvpPlayerIndex), new Vector3(startPointPos.x + num, startPointPos.y, startPointPos.z + num2), Quaternion.identity, survivorsParent));
		}
		PVPManager.pvpPlayers[pvpPlayerIndex].survivors = GetObjectList();
		PVPManager.pvpPlayers[pvpPlayerIndex].controller = this;
		PVPManager.pvpPlayers[pvpPlayerIndex].groupLootCount = PVPManager.Instance.levels[level - 1];
		SetSquadTarget();
		Invoke("UpdateLootTextAtStart", 0.5f);
		if (pvpPlayerIndex == 0)
		{
			PVPManager.pvpPlayers[0].UpdateBar();
		}
	}

	protected virtual void SetSquadTarget()
	{
	}

	private List<SurvivorHuman> GetObjectList()
	{
		List<SurvivorHuman> list = new List<SurvivorHuman>();
		for (int i = 0; i < squad.Count; i++)
		{
			list.Add(squad[i]);
		}
		return list;
	}

	public void RemoveSurvivor(PVPSoldierSurvivor survivor)
	{
		if (squad.Count == 1)
		{
			Die(survivor.transform.position);
		}
		squad.Remove(survivor);
		PVPManager.pvpPlayers[pvpPlayerIndex].survivors.Remove(survivor);
		PVPManager.pvpPlayers[pvpPlayerIndex].DecreaseLevel();
	}

	public Vector3 GetGroupCenter()
	{
		return PVPManager.pvpPlayers[pvpPlayerIndex].GetGroupCenter();
	}

	public void CheckGroupParts()
	{
		groups.Clear();
		for (int i = 0; i < squad.Count; i++)
		{
			GroupPartInfo groupPartInfo = new GroupPartInfo();
			groupPartInfo.indexes.Add(i);
			groupPartInfo.survivors.Add(squad[i]);
			for (int j = 0; j < squad.Count; j++)
			{
				if (i != j)
				{
					RaycastHit hitInfo;
					if (!Physics.Linecast(squad[i].transform.position + AddCenterPos(), squad[j].transform.position + AddCenterPos(), out hitInfo, obstacle))
					{
						groupPartInfo.indexes.Add(j);
						groupPartInfo.survivors.Add(squad[j]);
					}
					else if (Vector3.Distance(squad[i].transform.position, squad[j].transform.position) < 3f)
					{
						groupPartInfo.indexes.Add(j);
						groupPartInfo.survivors.Add(squad[j]);
					}
				}
			}
			if (!GroupExists(groupPartInfo))
			{
				groups.Add(groupPartInfo);
			}
		}
	}

	private bool GroupExists(GroupPartInfo group)
	{
		for (int i = 0; i < groups.Count; i++)
		{
			if (CompareGroups(group.indexes, groups[i].indexes))
			{
				return true;
			}
		}
		return false;
	}

	private bool CompareGroups<T>(List<T> listA, List<T> listB)
	{
		return listA.Intersect(listB).ToList().Count >= listA.Count / 2;
	}

	private Vector3 AddCenterPos()
	{
		return new Vector3(0f, 0.9f, 0f);
	}

	private void LateUpdate()
	{
		Vector3 groupCenter;
		if (PVPManager.pvpPlayers[pvpPlayerIndex].TryToGetGroupCenter(out groupCenter))
		{
			topUI.transform.position = new Vector3(groupCenter.x, 3.5f, groupCenter.z);
		}
	}

	private void UpdateLootTextAtStart()
	{
		UpdateLootText(PVPManager.pvpPlayers[pvpPlayerIndex].groupLootCount, PVPManager.pvpPlayers[pvpPlayerIndex].color);
	}

	public void UpdateLootText(float count, Color color = default(Color))
	{
		if (textLevel != null)
		{
			textLevel.text = level.ToString();
			if (color != default(Color))
			{
				textLevel.color = color;
			}
		}
		if (count > maxLootCount)
		{
			maxLootCount = count;
		}
	}

	public PVPPlayerInfo GetPvpInfo()
	{
		return PVPManager.pvpPlayers[pvpPlayerIndex];
	}

	public virtual void AddSurvivor()
	{
		Vector3 groupCenter = GetGroupCenter();
		float num = Random.Range(-0.5f, 0.5f);
		float num2 = Random.Range(-0.5f, 0.5f);
		squad.Add(Object.Instantiate(PVPManager.Instance.GetSoldierByLevel(pvpPlayerIndex), new Vector3(groupCenter.x + num, groupCenter.y, groupCenter.z + num2), Quaternion.identity, survivorsParent));
		PVPManager.pvpPlayers[pvpPlayerIndex].survivors.Add(squad.Last());
		squad.Last().groupController = this;
	}

	public void Die(Vector3 position)
	{
		int num = (int)(maxLootCount * 1.3f);
		for (int i = 0; i < num; i++)
		{
			LootObject lootObject = ObjectPooler.Instance.GetLootObject();
			lootObject.MoveToPosition(position, position + GetPositionInCircle(), 1f);
			lootObject.tag = "Loot";
			lootObject.isPooled = true;
		}
		GetPvpInfo().place = PVPManager.Instance.lastPlace;
		PVPManager.Instance.lastPlace--;
		int num2 = 0;
		for (int j = 0; j < PVPManager.pvpPlayers.Count; j++)
		{
			if (PVPManager.pvpPlayers[j].IsAlive())
			{
				num2++;
			}
		}
		if (pvpPlayerIndex == 0 || num2 <= 2)
		{
			PVPManager.Instance.PvpGameOver();
		}
		Object.Destroy(base.gameObject);
	}

	private Vector3 GetPositionInCircle()
	{
		Vector3 result = Random.onUnitSphere * 3.5f;
		result.y = 0.4f;
		return result;
	}
}
