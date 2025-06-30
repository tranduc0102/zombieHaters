using UnityEngine;
using UnityEngine.UI;

namespace Spine.Unity.Examples
{
	public class EquipButtonExample : MonoBehaviour
	{
		public EquipAssetExample asset;

		public EquipSystemExample equipSystem;

		public Image inventoryImage;

		private void OnValidate()
		{
			MatchImage();
		}

		private void MatchImage()
		{
			if (inventoryImage != null)
			{
				inventoryImage.sprite = asset.sprite;
			}
		}

		private void Start()
		{
			MatchImage();
			Button component = GetComponent<Button>();
			component.onClick.AddListener(delegate
			{
				equipSystem.Equip(asset);
			});
		}
	}
}
