﻿using UnityEngine;
using System.Collections;

public class Lobby : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void onStartButtonClick() {
		Application.LoadLevel ("Island");
	}
}
