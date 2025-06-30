using System.Collections;
using UnityEngine;

public class HurtFlashEffect : MonoBehaviour
{
	private const int DefaultFlashCount = 3;

	public int flashCount = 3;

	public Color flashColor = Color.white;

	[Range(1f / 120f, 1f / 15f)]
	public float interval = 1f / 60f;

	public string fillPhaseProperty = "_FillPhase";

	public string fillColorProperty = "_FillColor";

	private MaterialPropertyBlock mpb;

	private MeshRenderer meshRenderer;

	public void Flash()
	{
		if (mpb == null)
		{
			mpb = new MaterialPropertyBlock();
		}
		if (meshRenderer == null)
		{
			meshRenderer = GetComponent<MeshRenderer>();
		}
		meshRenderer.GetPropertyBlock(mpb);
		StartCoroutine(FlashRoutine());
	}

	private IEnumerator FlashRoutine()
	{
		if (flashCount < 0)
		{
			flashCount = 3;
		}
		int fillPhase = Shader.PropertyToID(fillPhaseProperty);
		int fillColor = Shader.PropertyToID(fillColorProperty);
		WaitForSeconds wait = new WaitForSeconds(interval);
		for (int i = 0; i < flashCount; i++)
		{
			mpb.SetColor(fillColor, flashColor);
			mpb.SetFloat(fillPhase, 1f);
			meshRenderer.SetPropertyBlock(mpb);
			yield return wait;
			mpb.SetFloat(fillPhase, 0f);
			meshRenderer.SetPropertyBlock(mpb);
			yield return wait;
		}
		yield return null;
	}
}
