using UnityEngine;
using UnityEngine.UI;

public class UIPVPPlayerRow : MonoBehaviour
{
	[SerializeField]
	private Text textName;

	[SerializeField]
	private Text textPower;

	[SerializeField]
	private Image imagePowerBar;

	private float maxBarLength = -1f;

	private float minBarLength = 115f;

	public void SetValues(string name, float power, Color color, float averagePowerValue)
	{
		if (maxBarLength == -1f)
		{
			maxBarLength = imagePowerBar.rectTransform.sizeDelta.x;
		}
		textName.text = name;
		textName.color = color;
		textPower.text = ((int)power).ToString();
		float num = (maxBarLength - minBarLength) / 2f;
		float num2 = num - minBarLength + power / averagePowerValue * (maxBarLength - minBarLength);
		num2 = ((num2 < minBarLength) ? minBarLength : ((!(num2 > maxBarLength)) ? num2 : maxBarLength));
		imagePowerBar.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, num2);
		imagePowerBar.color = color;
	}
}
