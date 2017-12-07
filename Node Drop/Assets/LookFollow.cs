using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using DAQRI;

public class LookFollow : MonoBehaviour {
	public float offset;
	private Vector3 displayPosition;
	private Vector3 displayForward;
	private Vector3 distanceToAnchor;
	private Vector3 offsetVector;
	private Vector3 offsetNoXZ;
	private Vector3 offsetNoY;
	private Vector3 offsetNoXY;
	private Vector3 newAnchor;
	private float offsetX;
	private float offsetY;
	private float offsetZ;
	private bool isDropped = true;
	private bool adjustY = false;
	private bool adjustZ = false;
	private bool toggleAnchor = false;
	Vector3 displayRelative;
	bool stop = false;
	bool up = false;
	bool down = false;
	private ControlState state = ControlState.Grab;
	public Material grabIcon;
	public Material liftIcon;
	public Material pullIcon;
	//Vector3 offsetPos;

	enum ControlState {
		Grab,
		Lift,
		Pull
	}

	public void LeftToggle () {
		//var stateArray = Enum.GetValues (typeof(ControlState));
		state--;
		if (state < ControlState.Grab)
			state = ControlState.Pull;
	}

	public void RightToggle () {
		state++;
		if (state > ControlState.Pull)
			state = ControlState.Grab;
	}

	void MoveX () {
		displayPosition = DisplayManager.Instance.transform.position;
		displayForward = DisplayManager.Instance.transform.forward;
		offsetVector = displayPosition + (displayForward.normalized * distanceToAnchor.magnitude);
		offsetNoY = new Vector3 (offsetVector.x, offsetY, offsetVector.z);
		transform.position = offsetNoY;
	}

	void MoveY () {
		displayForward = DisplayManager.Instance.transform.forward;
		displayPosition = DisplayManager.Instance.transform.position;
		offsetVector = displayPosition + (displayForward.normalized * distanceToAnchor.magnitude);
		offsetNoXZ = new Vector3 (offsetX, offsetVector.y + Vector3.Distance (transform.position, transform.GetChild(1).transform.position), offsetZ);
		transform.position = offsetNoXZ;
	}

	void MoveZ () {
		displayForward = DisplayManager.Instance.transform.forward;
		displayPosition = DisplayManager.Instance.transform.position;
		distanceToAnchor = transform.GetChild (1).transform.position - DisplayManager.Instance.transform.position;
		offsetVector = displayPosition + (displayForward.normalized * distanceToAnchor.magnitude);

//		Vector3 projected = Vector3.ProjectOnPlane (offsetVector, transform.GetChild(5).transform.up);
//		displayRelative = DisplayManager.Instance.transform.InverseTransformPoint (projected);
//		Vector3 displayRelativeNode = DisplayManager.Instance.transform.InverseTransformPoint (transform.position);
//
//		Vector3 twoRelatives = new Vector3 (displayRelativeNode.x, Vector3.Distance (transform.position, transform.GetChild (1).transform.position), displayRelative.z);
//		Vector3 backToWorld = DisplayManager.Instance.transform.TransformPoint (twoRelatives);
//
//		Vector3 victory = new Vector3 (backToWorld.x, offsetY, backToWorld.z);
//		transform.position = victory;

		Vector3 projected = Vector3.ProjectOnPlane (offsetVector, Vector3.up);
		projected = new Vector3 (projected.x, offsetY, projected.z);
		transform.position = projected;
	}

	void FixedUpdate () {
		//Debug.LogError(transform.position);
		//Debug.LogError(DisplayManager.Instance.transform.position);

		//print (DisplayManager.Instance.transform.forward);
		//print (transform.position - DisplayManager.Instance.transform.position);

		switch (state) {
		case ControlState.Grab:
			transform.GetChild (1).GetComponent<MeshRenderer> ().material = grabIcon;
			if (!isDropped) {
				MoveX ();
			}
			break;
		case ControlState.Lift:
			transform.GetChild (1).GetComponent<MeshRenderer> ().material = liftIcon;
			if (AdjustY) {
				MoveY ();
			}
			break;
		case ControlState.Pull:
			transform.GetChild (1).GetComponent<MeshRenderer> ().material = pullIcon;
			if (AdjustZ) {
				MoveZ ();
			}
			break;
		}
	}

	public bool ToggleAnchor {
		get {
			return toggleAnchor;
		} set {
			toggleAnchor = !toggleAnchor;

			if (toggleAnchor)
				transform.GetChild (11).gameObject.SetActive (false);
			else
				transform.GetChild (11).gameObject.SetActive (true);

			switch (state) {
			case ControlState.Grab:
				AdjustX = !AdjustX;
				break;
			case ControlState.Lift:
				AdjustY = !AdjustY;
				break;
			case ControlState.Pull:
				AdjustZ = !AdjustZ;
				break;
			}
		}
	}

	public bool AdjustX {
		get {
			return isDropped;
		} set {
			offsetY = transform.position.y;
			distanceToAnchor = transform.GetChild(1).transform.position - DisplayManager.Instance.transform.position;

			isDropped = !isDropped;
			if (!isDropped) {
				transform.GetChild (1).gameObject.SetActive (false);
				transform.GetChild (4).gameObject.SetActive (true);
				transform.GetChild (6).gameObject.SetActive (false);
			} else {
				transform.GetChild (1).gameObject.SetActive (true);
				transform.GetChild (4).gameObject.SetActive (false);
				transform.GetChild (6).gameObject.SetActive (false);
			}
		} 
	}

	public bool AdjustY {
		get {
			return adjustY;
		} set {
			//RaycastHit hit;
			//if (Physics.Raycast (DisplayManager.Instance.transform.position, DisplayManager.Instance.transform.forward, out hit))
				//contactPoint = hit.point;

			offsetX = transform.position.x;
			offsetZ = transform.position.z;
			distanceToAnchor = transform.GetChild(1).transform.position - DisplayManager.Instance.transform.position;

			adjustY = !adjustY;

			if (adjustY) {
				transform.GetChild (1).gameObject.SetActive (false);
				transform.GetChild (3).gameObject.SetActive (true);
			} else {
				transform.GetChild (1).gameObject.SetActive (true);
				transform.GetChild (3).gameObject.SetActive (false);
			}
		} 
	}

	public bool AdjustZ {
		get {
			return adjustZ;
		} set {
			displayRelative = DisplayManager.Instance.transform.InverseTransformPoint (transform.position);

			offsetX = transform.position.x;
			offsetY = transform.position.y;

			distanceToAnchor = transform.GetChild(1).transform.position - DisplayManager.Instance.transform.position;

			adjustZ = !adjustZ;

			if (adjustZ) {
				transform.GetChild (1).gameObject.SetActive (false);
				transform.GetChild (3).gameObject.SetActive (false);
				transform.GetChild (6).gameObject.SetActive (true);
			} else {
				transform.GetChild (1).gameObject.SetActive (true);
				transform.GetChild (3).gameObject.SetActive (false);
				transform.GetChild (6).gameObject.SetActive (false);
			}
		} 
	}

}
