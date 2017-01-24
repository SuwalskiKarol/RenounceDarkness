using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class PowerUp : MonoBehaviour {
	public List<GameObject> weapons = new List<GameObject>();
	public bool forHandGun;
	public bool forAssaultRifle;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	void OnTriggerEnter(Collider col){
		
		if (col.tag == "Player") {
			
			foreach(GameObject go in weapons){
				if (forHandGun) {
					if (go.GetComponent<newGun> ().HandGun) {
					
						go.GetComponent<newGun> ().currentAmmo += go.GetComponent<newGun> ().maxMagazineAmmo;
						Destroy (this.gameObject);
					}
				}

				if (forAssaultRifle) {
					if (go.GetComponent<newGun> ().AssaultRifle) {

						go.GetComponent<newGun> ().currentAmmo += go.GetComponent<newGun> ().maxMagazineAmmo;
						Destroy (this.gameObject);
					}
				}
			}
		}

	}
}
