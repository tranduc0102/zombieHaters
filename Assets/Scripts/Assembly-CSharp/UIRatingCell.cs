using System.Linq;
using CellController;
using UnityEngine;
using UnityEngine.UI;

public class UIRatingCell : UIScrollCell
{
	public RectSize rectTransformSize;

	public Text bonusText;

	public Text simpleRatingText;

	public RectSize simpleRatingTextSize;

	public RectSize simpleRatingTextPostition;

	public Text percentageText;

	public RectSize percentageTextSize;

	public RectSize percentagePostition;

	public Text ratingCountText;

	public RectSize ratingCountTextSize;

	public RectSize ratingCountTextPostition;

	public Image halfTransparent;

	public ImageColor transparentColors;

	public Image stripe;

	public RectSize stripeSize;

	public Image crown;

	public RectSize crownRectSize;

	public ParticleSystem psNext;

	public ParticleSystem psPrev;

	private float nextPosition;

	private float myPosition;

	private float prevPosition;

	private StagedArenaInfo arenaInfo;

	public override void SetContent(int index)
	{
		base.SetContent(index);
		SetPivot(new Vector2(0.5f, 0.5f));
		arenaInfo = ArenaManager.instance.GetStagedArenaInfoByIndex(base.cellIndex);
		percentageText.text = "+" + arenaInfo.stages[0].gemsReward;
	}

	public void OnEnable()
	{
		bonusText.text = LanguageManager.instance.GetLocalizedText(LanguageKeysEnum.Bonus) + " " + (base.cellIndex + 1);
	}

	public void SetSize(float percentage)
	{
		rectTransform.sizeDelta = AddPercentageToVector(percentage, rectTransformSize.minSize, rectTransformSize.maxSize);
		stripe.rectTransform.sizeDelta = AddPercentageToVector(percentage, stripeSize.minSize, stripeSize.maxSize);
		halfTransparent.color = AddPercentageToColor(percentage, transparentColors.startColor, transparentColors.targetColor);
		ratingCountText.rectTransform.sizeDelta = AddPercentageToVector(percentage, ratingCountTextSize.minSize, ratingCountTextSize.maxSize);
		ratingCountText.rectTransform.anchoredPosition = AddPercentageToVector(percentage, ratingCountTextPostition.minSize, ratingCountTextPostition.maxSize);
		ratingCountText.fontSize = (int)(40f + 25f * percentage);
		percentageText.rectTransform.sizeDelta = AddPercentageToVector(percentage, percentageTextSize.minSize, percentageTextSize.maxSize);
		percentageText.rectTransform.anchoredPosition = AddPercentageToVector(percentage, percentagePostition.minSize, percentagePostition.maxSize);
		simpleRatingText.rectTransform.sizeDelta = AddPercentageToVector(percentage, simpleRatingTextSize.minSize, simpleRatingTextSize.maxSize);
		simpleRatingText.rectTransform.anchoredPosition = AddPercentageToVector(percentage, simpleRatingTextPostition.minSize, simpleRatingTextPostition.maxSize);
		if (percentage > 0.55f)
		{
			base.transform.SetAsLastSibling();
		}
	}

	public void UpdateSize(float contentPositionX)
	{
		if (contentPositionX > prevPosition && contentPositionX < nextPosition)
		{
			if (contentPositionX < myPosition)
			{
				SetSize(StaticConstants.GetPercentageBetweenPoints(contentPositionX, prevPosition, myPosition));
			}
			else
			{
				SetSize(1f - StaticConstants.GetPercentageBetweenPoints(contentPositionX, myPosition, nextPosition));
			}
		}
		else
		{
			SetSize(0f);
		}
	}

	public void SetCellSize(float size, float startBorder)
	{
		myPosition = size * (float)(base.cellIndex - 1) + startBorder;
		prevPosition = size * (float)(base.cellIndex - 2) + startBorder;
		nextPosition = size * (float)base.cellIndex + startBorder;
	}

	private Vector2 AddPercentageToVector(float percentage, Vector2 min, Vector2 max)
	{
		return new Vector2(min.x + (max.x - min.x) * percentage, min.y + (max.y - min.y) * percentage);
	}

	private Color AddPercentageToColor(float percentage, Color startColor, Color targetColor)
	{
		return new Color(CalculateColor(percentage, startColor.r, targetColor.r), CalculateColor(percentage, startColor.g, targetColor.g), CalculateColor(percentage, startColor.b, targetColor.b), CalculateColor(percentage, startColor.a, targetColor.a));
	}

	private float CalculateColor(float percentage, float color1, float color2)
	{
		return color1 + (color2 - color1) * percentage;
	}

	public void UpdateAppearance(int selectedIndex)
	{
		if (selectedIndex >= base.cellIndex)
		{
			stripe.sprite = UIController.instance.multiplyImages.stripe.active;
			crown.gameObject.SetActive(true);
		}
		else
		{
			stripe.sprite = UIController.instance.multiplyImages.stripe.inactive;
			crown.gameObject.SetActive(false);
		}
		if (selectedIndex == base.cellIndex)
		{
			crown.gameObject.SetActive(true);
			crown.sprite = UIController.instance.multiplyImages.crown.active;
			crown.rectTransform.sizeDelta = crownRectSize.maxSize;
			ratingCountText.text = DataLoader.playerData.arenaRating.ToString();
			return;
		}
		crown.sprite = UIController.instance.multiplyImages.crown.inactive;
		crown.rectTransform.sizeDelta = crownRectSize.minSize;
		if (base.cellIndex == 0)
		{
			ratingCountText.text = "0";
		}
		else
		{
			ratingCountText.text = arenaInfo.stages.First().rating.ToString();
		}
	}

	public float GetCellCenterX()
	{
		return myPosition;
	}
}
