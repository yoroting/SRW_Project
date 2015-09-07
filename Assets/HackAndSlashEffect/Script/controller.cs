using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class controller : MonoBehaviour {

	public GameObject[] myParticles;
	public Text nameParticle;
	public Text numParticle;
	public int currentParticle;
	int maxParticle;
	int countFOV;
	float speedZoom;
	float targetFOV;

	void Start () {
		currentParticle = 0;
		maxParticle = 49;
		countFOV = 0;
		speedZoom = 12f;
		UpdateName (myParticles [currentParticle].name);
		GetComponent<AudioSource>().Play();
	}

	void Update () {
		if (Input.GetKeyDown ("left")) {
			currentParticle = (Mathf.Clamp(currentParticle - 1, 0, maxParticle));
			Instantiate(myParticles[currentParticle]);
			UpdateName (myParticles[currentParticle].name);
		}
		if (Input.GetKeyDown ("right")) {
			currentParticle = (Mathf.Clamp(currentParticle + 1, 0, maxParticle));
			Instantiate(myParticles[currentParticle]);
			UpdateName (myParticles[currentParticle].name);
		}
		if (Input.GetKeyDown(KeyCode.RightControl))  {
			Instantiate(myParticles[currentParticle]);
			UpdateName (myParticles[currentParticle].name);
		}
		if (Input.GetKeyDown ("up")) {
			countFOV = (Mathf.Clamp(countFOV + 1, -1, 1));
		}
		if (Input.GetKeyDown ("down")) {
			countFOV = (Mathf.Clamp(countFOV - 1, -1, 1));
		}

		switch (countFOV) {
		case -1:
			targetFOV = 30f;
			break;
		case 0:
			targetFOV = 50f;
			break;
		case 1:
			targetFOV = 70f;
			break;
		}
		Camera.main.fieldOfView = 
			Mathf.Lerp(Camera.main.fieldOfView, targetFOV, Time.deltaTime * speedZoom);
	}

	void UpdateName (string name) {
		nameParticle.text = name;
		numParticle.text = "Effect : " + (currentParticle+1) + " / " + (maxParticle+1);
	}

	public void GoShortcut (int i) {
		currentParticle = i;
		Instantiate(myParticles[currentParticle]);
		UpdateName (myParticles[currentParticle].name);
	}
}