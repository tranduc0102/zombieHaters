using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class BotTarget : MonoBehaviour
{
	private PVPTargetTypes targetType;

	private BotPVPGroupController botController;

	private Transform currentTarget;

	private Vector3 optimalPosition;

	private NavMeshAgent agent;

	private LayerMask lootDropMask;

	private LayerMask obstacleMask;

	private Coroutine p_Move;

	private Coroutine l_Move;

	private Coroutine lb_Move;

	private float timeBeforeStartFight = 30f;

	public void SetController(BotPVPGroupController groupController)
	{
		botController = groupController;
	}

	private void Start()
	{
		agent = GetComponent<NavMeshAgent>();
		lootDropMask = 1 << LayerMask.NameToLayer("LootDrop");
		obstacleMask = (1 << LayerMask.NameToLayer("Obstacle")) | (1 << LayerMask.NameToLayer("Water"));
		StartCoroutine(FindBestTarget());
		targetType = PVPTargetTypes.Undefined;
		timeBeforeStartFight += Time.time;
	}

	private IEnumerator FindBestTarget()
	{
		yield return null;
		while (true)
		{
			GetBestTarget();
			yield return new WaitForSeconds(2.5f);
		}
	}

	private void ResetTarget()
	{
		StopCor(p_Move);
		StopCor(l_Move);
		StopCor(lb_Move);
		targetType = PVPTargetTypes.Undefined;
	}

	private void StopCor(Coroutine cor)
	{
		if (cor != null)
		{
			StopCoroutine(cor);
		}
	}

	private void GetBestTarget()
	{
		ResetTarget();
		if (FindClosestLoot(true))
		{
			targetType = PVPTargetTypes.Loot;
			return;
		}
		if (Time.time > timeBeforeStartFight)
		{
			if (PVPManager.Instance.GetStrongestGroupIndex() == botController.pvpPlayerIndex)
			{
				if (targetType != PVPTargetTypes.Player)
				{
					targetType = PVPTargetTypes.Player;
					p_Move = StartCoroutine(PlayerMoving(GetClosestPVPPlayer(PVPManager.pvpPlayers)));
				}
			}
			else if (PVPManager.pvpPlayers[botController.pvpPlayerIndex].survivors.Count > 10)
			{
				List<PVPPlayerInfo> weakGroups;
				if (PVPManager.Instance.GetWeakGroups(botController.pvpPlayerIndex, out weakGroups) || PVPManager.Instance.lootBoxes.Count == 0)
				{
					PVPPlayerInfo closestPVPPlayer = GetClosestPVPPlayer(weakGroups);
					if (PVPManager.Instance.lootBoxes.Count != 0 && closestPVPPlayer != null)
					{
						Transform closestTransform = GetClosestTransform(PVPManager.Instance.lootBoxes);
						if (CompareVectorsAndGetClosest(botController.GetGroupCenter(), closestPVPPlayer.GetGroupCenter(), closestTransform.position) == closestTransform.position)
						{
							if (targetType != PVPTargetTypes.LootBox)
							{
								targetType = PVPTargetTypes.LootBox;
								currentTarget = closestTransform;
								lb_Move = StartCoroutine(LootBoxMoving());
							}
						}
						else if (targetType != PVPTargetTypes.Player)
						{
							targetType = PVPTargetTypes.Player;
							p_Move = StartCoroutine(PlayerMoving(closestPVPPlayer));
						}
					}
					else if (PVPManager.Instance.lootBoxes.Count == 0)
					{
						if (targetType != PVPTargetTypes.Player)
						{
							targetType = PVPTargetTypes.Player;
							closestPVPPlayer = GetClosestPVPPlayer(PVPManager.pvpPlayers);
							p_Move = StartCoroutine(PlayerMoving(closestPVPPlayer));
						}
					}
					else if (targetType != PVPTargetTypes.LootBox)
					{
						targetType = PVPTargetTypes.LootBox;
						currentTarget = GetClosestTransform(PVPManager.Instance.lootBoxes);
						lb_Move = StartCoroutine(LootBoxMoving());
					}
				}
				else
				{
					PVPPlayerInfo closestPVPPlayer2 = GetClosestPVPPlayer(PVPManager.pvpPlayers);
					if (Vector3.Distance(botController.GetGroupCenter(), closestPVPPlayer2.GetGroupCenter()) < 25f)
					{
						targetType = PVPTargetTypes.Player;
						p_Move = StartCoroutine(PlayerMoving(closestPVPPlayer2));
					}
					else
					{
						FindClosestLoot(false);
					}
				}
			}
			else
			{
				PVPPlayerInfo closestPVPPlayer3 = GetClosestPVPPlayer(PVPManager.pvpPlayers);
				if (Vector3.Distance(botController.GetGroupCenter(), closestPVPPlayer3.GetGroupCenter()) < 25f)
				{
					targetType = PVPTargetTypes.Player;
					p_Move = StartCoroutine(PlayerMoving(closestPVPPlayer3));
				}
				else
				{
					FindClosestLoot(false);
				}
			}
		}
		if (targetType == PVPTargetTypes.Undefined && FindClosestLoot(false, 100f))
		{
			targetType = PVPTargetTypes.Loot;
		}
	}

	private Transform GetClosestTransform(List<Transform> list)
	{
		Transform result = null;
		float num = float.PositiveInfinity;
		Vector3 groupCenter = botController.GetGroupCenter();
		for (int i = 0; i < list.Count; i++)
		{
			float num2 = Vector3.Distance(groupCenter, list[i].position);
			if (num2 < num)
			{
				result = list[i];
				num = num2;
			}
		}
		return result;
	}

	private T GenericGetClosest<T>(List<T> list) where T : Component
	{
		T result = (T)null;
		float num = float.PositiveInfinity;
		Vector3 groupCenter = botController.GetGroupCenter();
		for (int i = 0; i < list.Count; i++)
		{
			T val = list[i];
			float num2 = Vector3.Distance(groupCenter, val.transform.position);
			if (num2 < num)
			{
				result = list[i];
				num = num2;
			}
		}
		return result;
	}

	private bool FindClosestLoot(bool playersLoot, float radius = 8f)
	{
		Vector3 groupCenter = botController.GetGroupCenter();
		Collider[] array = Physics.OverlapSphere(groupCenter, radius, lootDropMask.value);
		if (array.Length > 0)
		{
			if (playersLoot)
			{
				for (int i = 0; i < array.Length; i++)
				{
					if (array[i].tag == "Loot")
					{
						currentTarget = array[i].transform;
						l_Move = StartCoroutine(LootMoving(playersLoot));
						return true;
					}
				}
				return false;
			}
			float num = Vector3.Distance(groupCenter, array[0].transform.position);
			currentTarget = GenericGetClosest(array.ToList()).transform;
			l_Move = StartCoroutine(LootMoving(playersLoot));
			return true;
		}
		return false;
	}

	private IEnumerator LootMoving(bool playersLoot)
	{
		SetTargetPosition(currentTarget.transform.position);
		do
		{
			yield return new WaitForFixedUpdate();
		}
		while (currentTarget != null && currentTarget.gameObject.activeInHierarchy);
		targetType = PVPTargetTypes.Undefined;
		FindClosestLoot(playersLoot);
	}

	private PVPPlayerInfo GetClosestPVPPlayer(List<PVPPlayerInfo> list)
	{
		PVPPlayerInfo result = null;
		Vector3 groupCenter = botController.GetGroupCenter();
		float num = float.PositiveInfinity;
		for (int i = 0; i < list.Count; i++)
		{
			Vector3 groupCenter2;
			if (list[i] != PVPManager.pvpPlayers[botController.pvpPlayerIndex] && list[i].IsAlive() && list[i].TryToGetGroupCenter(out groupCenter2))
			{
				float num2 = Vector3.Distance(groupCenter, groupCenter2);
				if (num2 < num)
				{
					result = list[i];
					num = num2;
				}
			}
		}
		return result;
	}

	private Vector3 SetOptimalPositionInCircle(Vector3 position)
	{
		Vector3 vector = GetPositionInCircle() * 4.5f;
		return CompareVectorsAndGetClosest(botController.GetGroupCenter(), position + vector, position - vector);
	}

	private IEnumerator LootBoxMoving()
	{
		while (targetType == PVPTargetTypes.LootBox && currentTarget != null)
		{
			SetTargetPosition(SetOptimalPositionInCircle(currentTarget.transform.position));
			do
			{
				yield return new WaitForSeconds(0.5f);
			}
			while (Vector3.Distance(botController.GetGroupCenter(), optimalPosition) > 0.2f && targetType == PVPTargetTypes.LootBox);
		}
		targetType = PVPTargetTypes.Undefined;
		GetBestTarget();
	}

	private IEnumerator PlayerMoving(PVPPlayerInfo targetPlayer)
	{
		if (targetPlayer == null)
		{
			yield break;
		}
		currentTarget = targetPlayer.controller.transform;
		while (targetType == PVPTargetTypes.Player)
		{
			SetTargetPosition(SetOptimalPositionInCircle(targetPlayer.GetGroupCenter()));
			do
			{
				yield return new WaitForSeconds(0.3f);
			}
			while (Vector3.Distance(botController.GetGroupCenter(), optimalPosition) > 0.25f && targetType == PVPTargetTypes.Player);
		}
		targetType = PVPTargetTypes.Undefined;
		GetBestTarget();
	}

	private Vector3 GetPositionInCircle()
	{
		Vector2 insideUnitCircle = Random.insideUnitCircle;
		insideUnitCircle.Normalize();
		return new Vector3(insideUnitCircle.x, 0f, insideUnitCircle.y);
	}

	private Vector3 CompareVectorsAndGetClosest(Vector3 main, Vector3 first, Vector3 second)
	{
		if (Vector3.Distance(main, first) > Vector3.Distance(main, second))
		{
			return second;
		}
		return first;
	}

	private void SetTargetPosition(Vector3 position)
	{
		optimalPosition = position;
		Vector3 groupCenter = botController.GetGroupCenter();
		float num = Vector3.Distance(groupCenter, base.transform.position);
		if (num > 15f || (num > 7f && Physics.Linecast(groupCenter, base.transform.position, obstacleMask.value)))
		{
			agent.ResetPath();
			base.transform.position = groupCenter;
		}
		if (!IsVectorNaN(optimalPosition))
		{
			agent.SetDestination(optimalPosition);
		}
	}

	private bool IsVectorNaN(Vector3 vector)
	{
		if (float.IsNaN(vector.x) || float.IsNaN(vector.y) || float.IsNaN(vector.z))
		{
			return true;
		}
		return false;
	}
}
