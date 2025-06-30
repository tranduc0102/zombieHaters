using UnityEngine;

public class TransformParentManager : MonoBehaviour
{
	public Transform upgradePanelCams;

	public Transform heroes;

	public Transform moneyBox;

	public Transform zombies;

	public Transform bullets;

	public Transform fx;

	public Transform bossList;

	public static TransformParentManager Instance { get; private set; }

	private void Awake()
	{
		Instance = this;
	}
}
