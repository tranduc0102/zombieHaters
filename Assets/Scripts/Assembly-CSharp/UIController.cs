using System.Collections;
using IAP;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
	public ScrollContentControllers scrollControllers;

	public MultiplyImages multiplyImages;

	public static UIController instance { get; private set; }

	public void Awake()
	{
		instance = this;
	}

	private void Start()
	{
		scrollControllers.CreateAllCells();
	}

	public void ResizeScrollContent(RectTransform cellPrefab, ScrollRect scrollRect, int cellCount, float startBorder, float spaceBetweenCells)
	{
		if (scrollRect.horizontal)
		{
			float num = cellPrefab.rect.x + startBorder;
			for (int i = 0; i < cellCount; i++)
			{
				num += cellPrefab.rect.width * cellPrefab.localScale.x + spaceBetweenCells;
			}
			scrollRect.content.sizeDelta = new Vector2((cellPrefab.sizeDelta.x + spaceBetweenCells) * (float)cellCount - spaceBetweenCells + startBorder, scrollRect.content.sizeDelta.y);
		}
		else
		{
			float num2 = cellPrefab.rect.y - startBorder;
			for (int j = 0; j < cellCount; j++)
			{
				num2 -= cellPrefab.rect.height * cellPrefab.localScale.y + spaceBetweenCells;
			}
			scrollRect.content.sizeDelta = new Vector2(scrollRect.content.sizeDelta.x, (cellPrefab.sizeDelta.y + spaceBetweenCells) * (float)cellCount - spaceBetweenCells + startBorder);
		}
	}

	public T[] CreareScrollContent<T>(RectTransform cellPrefab, ScrollRect scrollRect, int cellCount, float startBorder, float spaceBetweenCells, bool horizontal)
	{
		scrollRect.horizontal = horizontal;
		scrollRect.vertical = !horizontal;
		T[] array = new T[cellCount];
		if (horizontal)
		{
			float num = cellPrefab.rect.x + startBorder;
			for (int i = 0; i < cellCount; i++)
			{
				RectTransform rectTransform = Object.Instantiate(cellPrefab, scrollRect.content);
				rectTransform.anchoredPosition = new Vector2(num, rectTransform.anchoredPosition.y);
				num += rectTransform.rect.width * rectTransform.localScale.x + spaceBetweenCells;
				base.gameObject.GetComponent<T>();
				array[i] = rectTransform.GetComponent<T>();
			}
			scrollRect.content.sizeDelta = new Vector2((cellPrefab.sizeDelta.x + spaceBetweenCells) * (float)cellCount - spaceBetweenCells + startBorder, scrollRect.content.sizeDelta.y);
		}
		else
		{
			float num2 = cellPrefab.rect.y - startBorder;
			for (int j = 0; j < cellCount; j++)
			{
				RectTransform rectTransform2 = Object.Instantiate(cellPrefab, scrollRect.content);
				rectTransform2.anchoredPosition = new Vector2(rectTransform2.anchoredPosition.x, num2);
				num2 -= rectTransform2.rect.height * rectTransform2.localScale.y + spaceBetweenCells;
				array[j] = rectTransform2.GetComponent<T>();
			}
			scrollRect.content.sizeDelta = new Vector2(scrollRect.content.sizeDelta.x, (cellPrefab.sizeDelta.y + spaceBetweenCells) * (float)cellCount - spaceBetweenCells + startBorder);
		}
		return array;
	}

	public IEnumerator Scale(Transform _transform)
	{
		Vector3 targetScale = new Vector3(1.05f, 1.05f, 1.05f);
		Vector3 targetScale2 = new Vector3(0.975f, 0.975f, 0.975f);
		float deltaspeed = 4f;
		while (_transform.localScale != targetScale)
		{
			_transform.localScale = Vector3.MoveTowards(_transform.localScale, targetScale, Time.deltaTime * deltaspeed);
			yield return null;
		}
		while (_transform.localScale != targetScale2)
		{
			_transform.localScale = Vector3.MoveTowards(_transform.localScale, targetScale2, Time.deltaTime * deltaspeed);
			yield return null;
		}
		while (_transform.localScale != Vector3.one)
		{
			_transform.localScale = Vector3.MoveTowards(_transform.localScale, Vector3.one, Time.deltaTime * deltaspeed);
			yield return null;
		}
	}

	public void StartGetPrice(Text textPrice, GameObject priceLoader, PurchaseInfo purchase, ref Coroutine cor)
	{
		if (cor != null)
		{
			StopCoroutine(cor);
		}
		cor = StartCoroutine(GetPrice(textPrice, priceLoader, purchase));
	}

	public IEnumerator GetPrice(Text textPrice, GameObject priceLoader, PurchaseInfo purchase)
	{
		textPrice.text = string.Empty;
		priceLoader.SetActive(true);
		if (purchase != null)
		{
			do
			{
				textPrice.text = InAppManager.Instance.GetPrice(purchase.purchaseName);
				yield return new WaitForSecondsRealtime(0.1f);
			}
			while (textPrice.text == string.Empty);
			priceLoader.SetActive(false);
		}
	}
}
