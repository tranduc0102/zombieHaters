using UnityEngine;

public class ClosedWall : MonoBehaviour
{
	[SerializeField]
	private TextMesh levelText;

	[SerializeField]
	private Transform canvasRect;

	[SerializeField]
	private int levelRequired;

	[SerializeField]
	private bool rotateTextRuntime;

	private MeshRenderer meshRenderer;

	private void Start()
	{
		Check(DataLoader.Instance.GetCurrentPlayerLevel());
		if (levelText != null)
		{
			meshRenderer = levelText.GetComponent<MeshRenderer>();
		}
		UpdateText();
		if (canvasRect != null && rotateTextRuntime)
		{
			canvasRect.rotation = Quaternion.identity;
			canvasRect.Rotate(new Vector3(75f, 0f, 0f));
		}
	}

	public void Check(int currentLevel)
	{
		if (currentLevel >= levelRequired)
		{
			Object.Destroy(base.gameObject);
		}
	}

	public void UpdateText()
	{
		if (levelText != null)
		{
			levelText.text = LanguageManager.instance.GetLocalizedText(LanguageKeysEnum.Reach_x_Level_to_Unlock).Replace("x", levelRequired.ToString());
			levelText.font = LanguageManager.instance.currentLanguage.font;
			meshRenderer.material = LanguageManager.instance.currentLanguage.font.material;
		}
	}
}
