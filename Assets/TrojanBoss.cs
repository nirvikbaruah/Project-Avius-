﻿using UnityEngine;
using System.Collections;

public class TrojanBoss : MonoBehaviour {

	public GameObject player;
	public Animator playerAnim;
	private Vector3 distance;
	public Animator anim;
	public PlayerController health;
	private float clock = 3f;
	private Vector3 target;
	public float healthLevel;

	// Use this for initialization
	void Start () {
	
	}

	void Update(){

	}
	
	// Update is called once per frame
	/*void Update () {
		clock -= Time.deltaTime;
		if (clock <= 0f) {
			if (Random.value < 0.2f){
				anim.SetBool("Charge", true);
				target = new Vector3(player.transform.position.x, transform.position.y, 0f);
				transform.position = Vector3.MoveTowards(transform.position, target, 30f * Time.deltaTime);
			}
			clock = 3f;
		}
		distance = transform.position - player.transform.position;
		anim.SetFloat ("Distance", distance.x);

	}*/

}
