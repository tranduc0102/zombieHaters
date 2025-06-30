using UnityEngine;

public class CarPointer : BasePointer
{
	private CarControll car;

	private new void Start()
	{
		base.Start();
		pointerImage.gameObject.SetActive(false);
		distanceText.enabled = false;
		base.enabled = false;
	}

	public void StartPointer()
	{
		base.enabled = true;
		car = Object.FindObjectOfType<CarControll>();
		targets.Add(car.transform.position);
		showArrow = false;
	}

	private new void FixedUpdate()
	{
		if (car == null || car.nearSurvivors)
		{
			targets.Clear();
		}
		else if (targets.Count > 0)
		{
			targets[0] = car.transform.position;
		}
		else
		{
			targets.Add(car.transform.position);
		}
		base.FixedUpdate();
	}

	public void ShowArrow(bool value)
	{
		showArrow = value;
	}
}
