using UnityEngine.Events;
using UnityEngine.UI;

namespace WoodenSword.ZombieHaters.UI
{
	public static class UIExtensions
	{
		public static void SetNewOnClickLogic(this Button button, UnityAction callback)
		{
			button.onClick.RemoveAllListeners();
			button.onClick.AddListener(callback);
		}

		public static string AddHexColor(this string text, string hexColor)
		{
			return "<color=#" + hexColor + ">" + text + "</color>";
		}
	}
}
