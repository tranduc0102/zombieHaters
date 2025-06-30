using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace IngameDebugConsole
{
	public class DebugLogItem : MonoBehaviour, IPointerClickHandler, IEventSystemHandler
	{
		[SerializeField]
		private RectTransform transformComponent;

		[SerializeField]
		private Image imageComponent;

		[SerializeField]
		private Text logText;

		[SerializeField]
		private Image logTypeImage;

		[SerializeField]
		private GameObject logCountParent;

		[SerializeField]
		private Text logCountText;

		private DebugLogEntry logEntry;

		private int entryIndex;

		private DebugLogRecycledListView manager;

		public RectTransform Transform
		{
			get
			{
				return transformComponent;
			}
		}

		public Image Image
		{
			get
			{
				return imageComponent;
			}
		}

		public int Index
		{
			get
			{
				return entryIndex;
			}
		}

		public void Initialize(DebugLogRecycledListView manager)
		{
			this.manager = manager;
		}

		public void SetContent(DebugLogEntry logEntry, int entryIndex, bool isExpanded)
		{
			this.logEntry = logEntry;
			this.entryIndex = entryIndex;
			Vector2 sizeDelta = transformComponent.sizeDelta;
			if (isExpanded)
			{
				logText.horizontalOverflow = HorizontalWrapMode.Wrap;
				sizeDelta.y = manager.SelectedItemHeight;
			}
			else
			{
				logText.horizontalOverflow = HorizontalWrapMode.Overflow;
				sizeDelta.y = manager.ItemHeight;
			}
			transformComponent.sizeDelta = sizeDelta;
			logText.text = ((!isExpanded) ? logEntry.logString : logEntry.ToString());
			logTypeImage.sprite = logEntry.logTypeSpriteRepresentation;
		}

		public void ShowCount()
		{
			logCountText.text = logEntry.count.ToString();
			logCountParent.SetActive(true);
		}

		public void HideCount()
		{
			logCountParent.SetActive(false);
		}

		public void OnPointerClick(PointerEventData eventData)
		{
			manager.OnLogItemClicked(this);
		}

		public float CalculateExpandedHeight(string content)
		{
			string text = logText.text;
			HorizontalWrapMode horizontalOverflow = logText.horizontalOverflow;
			logText.text = content;
			logText.horizontalOverflow = HorizontalWrapMode.Wrap;
			float preferredHeight = logText.preferredHeight;
			logText.text = text;
			logText.horizontalOverflow = horizontalOverflow;
			return Mathf.Max(manager.ItemHeight, preferredHeight);
		}

		public override string ToString()
		{
			return logEntry.ToString();
		}
	}
}
