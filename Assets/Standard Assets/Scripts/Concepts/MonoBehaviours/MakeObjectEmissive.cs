using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class MakeObjectEmissive : MonoBehaviour
{
	public Renderer renderer;
	public Color color;

	void Start ()
	{
		renderer.material.SetColor("_EmissionColor", color);
		renderer.material.EnableKeyword ("_EMISSION");
	}
}