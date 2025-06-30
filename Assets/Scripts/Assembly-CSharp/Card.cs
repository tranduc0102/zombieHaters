using UnityEngine;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
	[SerializeField]
	public Image imageBackSide;

	[SerializeField]
	public Image imageFrontSide;

	[SerializeField]
	public Image imageVideo;

	[SerializeField]
	public ParticleSystem glowFx;

	[HideInInspector]
	public GameObject content;

	private UICardsPresent uICardsPresent;

	private Button button;

	public void IsVisible(bool value)
	{
		imageBackSide.enabled = !value;
		imageFrontSide.enabled = value;
		content.SetActive(value);
		if (value)
		{
			imageVideo.enabled = false;
			if (button == null)
			{
				button = GetComponent<Button>();
			}
			button.interactable = false;
		}
	}

	public void Interactable(bool value)
	{
		if (button == null)
		{
			button = GetComponent<Button>();
		}
		button.interactable = value;
	}
}
