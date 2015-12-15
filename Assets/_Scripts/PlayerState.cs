﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(Health), typeof(PlayerMovement))]
public class PlayerState : MonoBehaviour {

	/*
		The character state class is used to handle the current state of the character between the Movement and Weapon Classes

		The state is calculated by using the classes parameters. The parameters are set in the other classes that the player uses;

		It is also has helper functions, e.g. to find out if the charater is grounded;
	*/

	[HideInInspector]
	public CharacterState CurrentState;

	[HideInInspector]
	public Vector2 currentSpeed;

	[HideInInspector]
	public float sprintSpeed;

	public float GroundCheckDistance = 2.4f;
	public LayerMask WhatIsGround;

	public bool IsGrounded() {
		RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, GroundCheckDistance, WhatIsGround);
		return hit;
	}

    public Image StaminaCircle;
    public Image HealthBar;

    private Health playerHealth;

	// Use this for initialization
	void Start () {
        playerHealth = GetComponent<Health>();
	}
	
	// Update is called once per frame
	void Update ()
    {
        
    }

	void OnDrawGizmos()
	{
		Debug.DrawRay(transform.position, Vector2.down * GroundCheckDistance, IsGrounded() ? Color.green : Color.red);
	}
}

public enum CharacterState {
	Grounded,
	Walking,
	Sprinting,
	Jumping,
	Falling,
	InAir
}