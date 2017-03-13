using UnityEngine;
using System.Collections;

public class GrapplingHook : MonoBehaviour {

	private bool inAir = false;
	private HingeJoint grabHinge;

	void OnCollisionEnter (Collision col) {
		grabHinge = gameObject.AddComponent <HingeJoint>();
		grabHinge.connectedBody = col.rigidbody;
		Debug.Log ("Ya");
			//This stops the hook once it collides with something, and creates a HingeJoint to the object it collided with.
	}
}
