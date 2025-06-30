using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CarControll : MonoBehaviour
{
	public static int countGasInHands;

	[SerializeField]
	private WheelCollider[] fwheels;

	[SerializeField]
	private WheelCollider[] rwheels;

	[SerializeField]
	private Transform roundCenter;

	[SerializeField]
	private ParticleSystem refuelingFx;

	[Space]
	[SerializeField]
	private ZombieDamageZone zombieDamageZone;

	[SerializeField]
	private Turret turret;

	[Space]
	[SerializeField]
	private int fuelCapacity = 3;

	[Space]
	[SerializeField]
	private float maxSpeed = 100f;

	[SerializeField]
	private float acceleration = 10f;

	[Space]
	[SerializeField]
	private float maxSteerAngle = 50f;

	[SerializeField]
	private float turnSpeed = 0.1f;

	[Space]
	[SerializeField]
	private float distanceToSurvivorsForMove = 10f;

	[Space]
	[SerializeField]
	private Transform targetPoint;

	[Space]
	[SerializeField]
	private Animator animatorFillCircle;

	[SerializeField]
	private Image imageFrame;

	[SerializeField]
	private Image imageFill;

	private float rotateAngle;

	private float speedMultiplier;

	private int fuel;

	private Rigidbody rigid;

	private CameraTarget cameraTarget;

	private bool canDrive;

	private float tmpWavePower = 15000f;

	private AudioSource audioSource;

	[HideInInspector]
	public bool nearSurvivors;

	private void Start()
	{
		countGasInHands = 0;
		speedMultiplier = GetComponent<Rigidbody>().mass / 1000f;
		audioSource = GetComponent<AudioSource>();
		cameraTarget = Object.FindObjectOfType<CameraTarget>();
		rigid = GetComponent<Rigidbody>();
		zombieDamageZone.gameObject.SetActive(false);
		turret.enabled = false;
		DataLoader.gui.InitCanistersUI(fuelCapacity);
		DataLoader.gui.RefreshFuelTankFill(fuelCapacity, fuel);
		imageFill.fillAmount = 0f;
		imageFrame.color = new Color(1f, 0.78f, 0.24f, 1f);
	}

	private void AddFuel(int count)
	{
		fuel += count;
		if (fuel >= fuelCapacity)
		{
			GameManager.instance.DeleteAllInactiveZombies();
			StartCoroutine(PrepareToGo());
			GetComponent<SphereCollider>().enabled = false;
			ArenaWavesManager.instance.StopArenaTimer(true);
			DataLoader.gui.carPointer.ShowArrow(true);
		}
		else
		{
			DataLoader.gui.carPointer.ShowArrow(false);
		}
		refuelingFx.Play();
	}

	private IEnumerator PrepareToGo()
	{
		rigid.constraints = (RigidbodyConstraints)84;
		yield return new WaitForSeconds(2f);
		animatorFillCircle.SetTrigger("CarStart");
		float time = 1f;
		RuntimeAnimatorController ac = animatorFillCircle.runtimeAnimatorController;
		for (int i = 0; i < ac.animationClips.Length; i++)
		{
			if (ac.animationClips[i].name == "CarStart")
			{
				time = ac.animationClips[i].length;
			}
		}
		yield return new WaitForSeconds(time);
		CanDrive();
	}

	private void CanDrive()
	{
		canDrive = true;
		zombieDamageZone.gameObject.SetActive(true);
		turret.enabled = true;
		targetPoint.GetComponent<CarTargetPoint>().SpawnZombies();
	}

	private void FixedUpdate()
	{
		if (canDrive)
		{
			if (!audioSource.isPlaying && !SoundManager.Instance.soundIsMuted)
			{
				audioSource.Play();
			}
			Vector3 from = targetPoint.position - roundCenter.position;
			rotateAngle = Vector3.Angle(from, roundCenter.forward);
			if (Vector3.Angle(from, roundCenter.right) > 90f)
			{
				rotateAngle *= -1f;
			}
			if (rotateAngle < 0f - maxSteerAngle)
			{
				rotateAngle = 0f - maxSteerAngle;
			}
			if (rotateAngle > maxSteerAngle)
			{
				rotateAngle = maxSteerAngle;
			}
			nearSurvivors = ((Vector3.Distance(base.transform.position, cameraTarget.transform.position) < distanceToSurvivorsForMove) ? true : false);
			if (!nearSurvivors && audioSource.isPlaying)
			{
				audioSource.Stop();
			}
			UpdateWheels();
		}
	}

	private void UpdateWheels()
	{
		WheelCollider[] array = fwheels;
		foreach (WheelCollider wheelCollider in array)
		{
			if (nearSurvivors)
			{
				wheelCollider.motorTorque = Mathf.Lerp(wheelCollider.motorTorque, maxSpeed * speedMultiplier, acceleration * speedMultiplier);
			}
			else
			{
				wheelCollider.motorTorque = 0f;
			}
			wheelCollider.steerAngle = Mathf.Lerp(wheelCollider.steerAngle, rotateAngle, turnSpeed);
			wheelCollider.transform.localEulerAngles = new Vector3(0f, wheelCollider.steerAngle, 0f);
		}
		WheelCollider[] array2 = rwheels;
		foreach (WheelCollider wheelCollider2 in array2)
		{
			if (nearSurvivors)
			{
				wheelCollider2.motorTorque = Mathf.Lerp(wheelCollider2.motorTorque, maxSpeed * speedMultiplier, acceleration * speedMultiplier);
			}
			else
			{
				wheelCollider2.motorTorque = 0f;
			}
		}
	}

	public void Stop()
	{
		rotateAngle = 0f;
		UpdateWheels();
		canDrive = false;
		cameraTarget.transform.parent = GameManager.instance.transform;
		cameraTarget.transform.parent = null;
		Invoke("StopTheCar", 4f);
	}

	private void StopTheCar()
	{
		if (audioSource.isPlaying)
		{
			audioSource.Stop();
		}
		WheelCollider[] array = fwheels;
		foreach (WheelCollider wheelCollider in array)
		{
			wheelCollider.motorTorque = 0f;
		}
		WheelCollider[] array2 = rwheels;
		foreach (WheelCollider wheelCollider2 in array2)
		{
			wheelCollider2.motorTorque = 0f;
		}
	}

	public void NewTarget(CarTargetPoint nextPoint, bool isLast = false)
	{
		targetPoint = nextPoint.transform;
		if (isLast)
		{
			maxSpeed *= 2f;
			cameraTarget.enabled = false;
			cameraTarget.transform.parent = roundCenter;
			cameraTarget.transform.localPosition = Vector3.zero;
			GameManager.instance.GoToFinishLine = true;
		}
		else
		{
			nextPoint.SpawnZombies();
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (countGasInHands > 0 && fuel < fuelCapacity && other.transform.tag == "Survivor")
		{
			AddFuel(countGasInHands);
			countGasInHands = 0;
			imageFrame.color = new Color(1f, 0.97f, 0.42f, 1f);
			StartCoroutine(FillCenter((float)fuel / (float)fuelCapacity));
			DataLoader.gui.RefreshCarCaravanProgress();
			DataLoader.gui.RefreshFuelTankFill(fuelCapacity, fuel);
		}
	}

	private IEnumerator FillCenter(float amount)
	{
		float fillSpeed = 1.5f;
		while (imageFill.fillAmount < amount)
		{
			imageFill.fillAmount = Mathf.Lerp(imageFill.fillAmount, amount, Time.deltaTime * fillSpeed);
			yield return null;
		}
	}
}
