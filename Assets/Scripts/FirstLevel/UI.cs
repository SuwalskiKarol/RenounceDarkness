using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using Scripts.FirstLevel;
public class UI : MonoBehaviour {
	public GameObject ammo;
	public List<GameObject> weapons = new List<GameObject>();

	void Update () {
		foreach (GameObject go in weapons) {
			if (go.GetComponent<newGun> ().isEquipment) {
				ammo.GetComponent<Text> ().text = go.GetComponent<newGun> ().magazineAmmo + "/" + go.GetComponent<newGun> ().currentAmmo;
			}
			}
	}
}
