using UnityEngine;
using System.Collections;

public class killParticle : MonoBehaviour {

	public float time;

	void Update () {
		Destroy (gameObject, time);
	}
}