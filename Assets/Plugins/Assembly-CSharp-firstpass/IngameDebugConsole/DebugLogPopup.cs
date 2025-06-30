using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace IngameDebugConsole
{
	public class DebugLogPopup : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IEventSystemHandler
	{
		private RectTransform popupTransform;

		private Vector2 halfSize;

		private Image backgroundImage;

		private CanvasGroup canvasGroup;

		[SerializeField]
		private DebugLogManager debugManager;

		[SerializeField]
		private Text newInfoCountText;

		[SerializeField]
		private Text newWarningCountText;

		[SerializeField]
		private Text newErrorCountText;

		private int newInfoCount;

		private int newWarningCount;

		private int newErrorCount;

		private Color normalColor;

		[SerializeField]
		private Color alertColorInfo;

		[SerializeField]
		private Color alertColorWarning;

		[SerializeField]
		private Color alertColorError;

		private bool isPopupBeingDragged;

		private IEnumerator moveToPosCoroutine;

		private void Awake()
		{
			popupTransform = (RectTransform)base.transform;
			backgroundImage = GetComponent<Image>();
			canvasGroup = GetComponent<CanvasGroup>();
			normalColor = backgroundImage.color;
		}

		private void Start()
		{
			halfSize = popupTransform.sizeDelta * 0.5f * popupTransform.root.localScale.x;
		}

		public void OnViewportDimensionsChanged()
		{
			halfSize = popupTransform.sizeDelta * 0.5f * popupTransform.root.localScale.x;
			OnEndDrag(null);
		}

		public void NewInfoLogArrived()
		{
			newInfoCount++;
			newInfoCountText.text = newInfoCount.ToString();
			if (newWarningCount == 0 && newErrorCount == 0)
			{
				backgroundImage.color = alertColorInfo;
			}
		}

		public void NewWarningLogArrived()
		{
			newWarningCount++;
			newWarningCountText.text = newWarningCount.ToString();
			if (newErrorCount == 0)
			{
				backgroundImage.color = alertColorWarning;
			}
		}

		public void NewErrorLogArrived()
		{
			newErrorCount++;
			newErrorCountText.text = newErrorCount.ToString();
			backgroundImage.color = alertColorError;
		}

		private void Reset()
		{
			newInfoCount = 0;
			newWarningCount = 0;
			newErrorCount = 0;
			newInfoCountText.text = "0";
			newWarningCountText.text = "0";
			newErrorCountText.text = "0";
			backgroundImage.color = normalColor;
		}

		private IEnumerator MoveToPosAnimation(Vector3 targetPos)
		{
			float modifier = 0f;
			Vector3 initialPos = popupTransform.position;
			while (modifier < 1f)
			{
				modifier += 4f * Time.unscaledDeltaTime;
				popupTransform.position = Vector3.Lerp(initialPos, targetPos, modifier);
				yield return null;
			}
		}

		public void OnPointerClick(PointerEventData data)
		{
			if (!isPopupBeingDragged)
			{
				debugManager.Show();
				Hide();
			}
		}

		public void Show()
		{
			canvasGroup.interactable = true;
			canvasGroup.blocksRaycasts = true;
			canvasGroup.alpha = 1f;
			Reset();
			OnViewportDimensionsChanged();
		}

		public void Hide()
		{
			canvasGroup.interactable = false;
			canvasGroup.blocksRaycasts = false;
			canvasGroup.alpha = 0f;
		}

		public void OnBeginDrag(PointerEventData data)
		{
			isPopupBeingDragged = true;
			if (moveToPosCoroutine != null)
			{
				StopCoroutine(moveToPosCoroutine);
				moveToPosCoroutine = null;
			}
		}

		public void OnDrag(PointerEventData data)
		{
			popupTransform.position = data.position;
		}

		public void OnEndDrag(PointerEventData data)
		{
			int width = Screen.width;
			int height = Screen.height;
			Vector3 position = popupTransform.position;
			float x = position.x;
			float num = Mathf.Abs(position.x - (float)width);
			float num2 = Mathf.Abs(position.y);
			float num3 = Mathf.Abs(position.y - (float)height);
			float num4 = Mathf.Min(x, num);
			float num5 = Mathf.Min(num2, num3);
			if (num4 < num5)
			{
				position = ((!(x < num)) ? new Vector3((float)width - halfSize.x, position.y, 0f) : new Vector3(halfSize.x, position.y, 0f));
				position.y = Mathf.Clamp(position.y, halfSize.y, (float)height - halfSize.y);
			}
			else
			{
				position = ((!(num2 < num3)) ? new Vector3(position.x, (float)height - halfSize.y, 0f) : new Vector3(position.x, halfSize.y, 0f));
				position.x = Mathf.Clamp(position.x, halfSize.x, (float)width - halfSize.x);
			}
			if (moveToPosCoroutine != null)
			{
				StopCoroutine(moveToPosCoroutine);
			}
			moveToPosCoroutine = MoveToPosAnimation(position);
			StartCoroutine(moveToPosCoroutine);
			isPopupBeingDragged = false;
		}
	}
}
