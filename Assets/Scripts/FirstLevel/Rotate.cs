using UnityEngine;
using System.Collections;

public class Rotate : MonoBehaviour {
	public int speed;

	void Update () {
		transform.Rotate (new Vector3 (0, 1, 0) * Time.deltaTime * speed);
	}



}
