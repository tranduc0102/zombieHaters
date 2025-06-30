using System.Collections;
using UnityEngine;

public class FloatingText : MonoBehaviour
{
	[SerializeField]
	private TextMesh textMesh;

	[SerializeField]
	private SpriteRenderer coinSprite;

	[SerializeField]
	private AudioClip appearanceSound;

	private Color color;

	public void StartBaseAnimation(string text)
	{
		textMesh.text = text;
		StartCoroutine(Up());
	}

	public void StartBossAnimation(string text)
	{
		textMesh.text = text;
		StartCoroutine(BossUp());
	}

	private IEnumerator BossUp()
	{
		float speed = 3f;
		textMesh.transform.position = new Vector3(base.transform.position.x, base.transform.position.y + 2.3f, base.transform.position.z);
		textMesh.transform.Rotate(57f, 0f, 0f);
		Vector3 startScale = base.transform.localScale;
		base.transform.localScale = Vector3.zero;
		yield return new WaitForSeconds(0.45f);
		base.transform.localScale = startScale;
		float t = 1.2f;
		while (textMesh.color.a > 0f)
		{
			yield return null;
			t -= Time.deltaTime;
			base.transform.position = new Vector3(base.transform.position.x, base.transform.position.y + speed / 2f * Time.deltaTime, base.transform.position.z);
			if (t < 0f)
			{
				color = textMesh.color;
				color.a -= Time.deltaTime / 2f;
				textMesh.color = color;
				coinSprite.color = color;
			}
		}
		Object.Destroy(base.gameObject);
	}

	private IEnumerator Up()
	{
		float speed = 3f;
		SoundManager.Instance.PlaySound(appearanceSound);
		textMesh.transform.position = new Vector3(base.transform.position.x, base.transform.position.y + 1.5f, base.transform.position.z);
		textMesh.transform.Rotate(57f, 0f, 0f);
		while (textMesh.color.a > 0f)
		{
			yield return null;
			base.transform.position = new Vector3(base.transform.position.x, base.transform.position.y + speed * Time.deltaTime, base.transform.position.z);
			color = textMesh.color;
			color.a -= Time.deltaTime;
			textMesh.color = color;
			color = coinSprite.color;
			color.a -= Time.deltaTime;
			coinSprite.color = color;
		}
		Object.Destroy(base.gameObject);
	}
}
