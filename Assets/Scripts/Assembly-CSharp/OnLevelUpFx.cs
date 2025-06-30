using UnityEngine;

public class OnLevelUpFx : MonoBehaviour
{
	[SerializeField]
	private ParticleSystem fx;

	public void Play()
	{
		DataLoader.gui.OnAnimationCompleted("MainMenu", "Menu", fx.Play);
	}
}
