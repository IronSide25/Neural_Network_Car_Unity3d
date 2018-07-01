using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDController : MonoBehaviour {

	Text generation;
	Text population;
	Text engine;
	Text top;

	neuralController controller;

	// Use this for initialization
	void Start () {
		generation = GetComponent<Text> ();
		population = GameObject.Find ("population").GetComponent<Text> ();
		engine = GameObject.Find ("speed").GetComponent<Text> ();
		top = GameObject.Find ("top").GetComponent<Text>();

		controller = GameObject.Find ("Flyer").GetComponent<neuralController> ();
	}
	
	// Update is called once per frame
	void Update () {
		generation.text = "front " + controller.frontForce;
		population.text = "back " + controller.backForce;
		engine.text = "left " + controller.leftForce;
		top.text = "right " + controller.rightForce;

		
	}
}
