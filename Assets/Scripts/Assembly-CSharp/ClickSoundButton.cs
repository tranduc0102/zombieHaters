using UnityEngine;
using UnityEngine.UI;

public class ClickSoundButton : MonoBehaviour
{
	private void Start()
	{
		GetComponent<Button>().onClick.AddListener(delegate
		{
			SoundManager.Instance.PlayClickSound();
		});
	}
}
