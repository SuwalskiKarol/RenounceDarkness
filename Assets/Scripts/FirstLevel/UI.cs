using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
public class UI : MonoBehaviour {
	public GameObject ammo;
	public List<GameObject> weapons = new List<GameObject>();
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		foreach (GameObject go in weapons) {
			if (go.GetComponent<newGun> ().isEquipment) {
				ammo.GetComponent<Text> ().text = go.GetComponent<newGun> ().magazineAmmo + "/" + go.GetComponent<newGun> ().currentAmmo;
			}
			}
	}
}
