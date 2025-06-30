using UnityEngine;

namespace EPPZ.Cloud.Scenes.Helpers
{
	[ExecuteInEditMode]
	public class AnchorConstraint : MonoBehaviour
	{
		private RectTransform _parentRectTransform;

		private RectTransform _rectTransform;

		private void OnEnable()
		{
			_rectTransform = GetComponent<RectTransform>();
			_parentRectTransform = base.transform.parent.GetComponent<RectTransform>();
		}

		private void Update()
		{
			if (base.enabled)
			{
				float num = _parentRectTransform.rect.width / _parentRectTransform.rect.height;
				_rectTransform.anchorMin = new Vector2(_rectTransform.anchorMin.x, _rectTransform.anchorMin.x * num);
				_rectTransform.anchorMax = new Vector2(_rectTransform.anchorMax.x, 1f - (1f - _rectTransform.anchorMax.x) * num);
				RectTransform rectTransform = _rectTransform;
				Vector2 zero = Vector2.zero;
				_rectTransform.offsetMax = zero;
				rectTransform.offsetMin = zero;
			}
		}
	}
}
