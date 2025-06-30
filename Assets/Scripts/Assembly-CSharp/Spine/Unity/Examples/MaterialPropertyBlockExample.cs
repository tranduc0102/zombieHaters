using UnityEngine;

namespace Spine.Unity.Examples
{
	public class MaterialPropertyBlockExample : MonoBehaviour
	{
		public float timeInterval = 1f;

		public Gradient randomColors = new Gradient();

		public string colorPropertyName = "_FillColor";

		private MaterialPropertyBlock mpb;

		private float timeToNextColor;

		private void Start()
		{
			mpb = new MaterialPropertyBlock();
		}

		private void Update()
		{
			if (timeToNextColor <= 0f)
			{
				timeToNextColor = timeInterval;
				Color value = randomColors.Evaluate(Random.value);
				mpb.SetColor(colorPropertyName, value);
				GetComponent<MeshRenderer>().SetPropertyBlock(mpb);
			}
			timeToNextColor -= Time.deltaTime;
		}
	}
}
