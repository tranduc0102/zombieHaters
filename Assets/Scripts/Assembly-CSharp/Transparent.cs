using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Transparent : MonoBehaviour
{
	[SerializeField]
	private Shader shader;

	[SerializeField]
	private float transp = 0.25f;

	private List<GameObject> objects = new List<GameObject>();

	private void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Obstacle")
		{
			objects.Add(other.gameObject);
			Renderer component = other.GetComponent<Renderer>();
			if (component != null)
			{
				component.material.shader = shader;
				StartCoroutine(SmoothRenderer(component, 1f));
			}
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.tag == "Obstacle")
		{
			objects.Remove(objects.First((GameObject obj) => obj == other.gameObject));
			Renderer component = other.GetComponent<Renderer>();
			if (component != null && !ContainsObject(other.gameObject))
			{
				StartCoroutine(SmoothRenderer(component, transp));
			}
		}
	}

	private bool ContainsObject(GameObject go)
	{
		for (int i = 0; i < objects.Count; i++)
		{
			if (objects[i] == go)
			{
				return true;
			}
		}
		return false;
	}

	private IEnumerator SmoothRenderer(Renderer rend, float value)
	{
		if (value == 1f)
		{
			while (value > transp)
			{
				value -= Time.deltaTime * 3f;
				Color clr = rend.material.color;
				clr.a = value;
				rend.material.color = clr;
				yield return null;
			}
		}
		else
		{
			while (value < 1f)
			{
				value += Time.deltaTime * 3f;
				rend.material.color = Color.white * value;
				yield return null;
			}
			rend.material.shader = Shader.Find("Unlit/Texture");
		}
	}
}
