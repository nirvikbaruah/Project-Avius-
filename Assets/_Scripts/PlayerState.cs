using UnityEngine;
using System.Collections;

public class PlayerState : MonoBehaviour {

	CharacterState CurrentState;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}

enum CharacterState {
	Grounded,
	Walking,
	Sprinting,
	Jumping,
	Falling
}