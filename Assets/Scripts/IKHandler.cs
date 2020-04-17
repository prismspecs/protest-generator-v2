using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IKHandler : MonoBehaviour {

	public Transform IKtarget;
	Animator anim;
	public float IKweight;

	// Use this for initialization
	void Start () {
		anim = GetComponent<Animator> ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	// everything having to do with IK must go here
	void OnAnimatorIK() {
		// position as well as rotation!
		anim.SetIKPositionWeight (AvatarIKGoal.RightHand, IKweight);
		anim.SetIKRotationWeight (AvatarIKGoal.RightHand, IKweight);

		anim.SetIKPosition (AvatarIKGoal.RightHand, IKtarget.position);
		anim.SetIKRotation (AvatarIKGoal.RightHand, IKtarget.rotation);
	}
}
