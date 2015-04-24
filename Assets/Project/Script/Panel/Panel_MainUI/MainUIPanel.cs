using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class MainUIPanel : BasicPanel {

	public GameObject StartButton;
	public GameObject LoadButton;
	public GameObject GalleryButton;
	public GameObject SetUpButton;

	void Awake(){
		UIEventListener.Get(StartButton).onClick += OnStartButtonClick;
		UIEventListener.Get(LoadButton).onClick += OnLoadButtonClick;
		UIEventListener.Get(GalleryButton).onClick += OnGalleryButtonClick;
		UIEventListener.Get(SetUpButton).onClick += OnSetUpButtonClick;
	}

	// Use this for initialization
	void Start () {

	}

	void OnStartButtonClick(GameObject go)
	{
		// When start button clicked do :
		Debug.Log("Start");
	}

	void OnLoadButtonClick(GameObject go)
	{
		// When load button clicked do :
		Debug.Log("Load");
	}

	void OnGalleryButtonClick(GameObject go)
	{
		// When gallery button clicked do :
		Debug.Log("Gallery");
	}

	void OnSetUpButtonClick(GameObject go)
	{
		// When setup button clicked do :
		Debug.Log("SetUp");
	}

}
