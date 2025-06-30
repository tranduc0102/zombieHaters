using UnityEngine;

public class UINewHeroOpened : MonoBehaviour
{
	public void OnDisable()
	{
		UIController.instance.scrollControllers.survivorsController.AnimateLastheroOpened();
	}
}
