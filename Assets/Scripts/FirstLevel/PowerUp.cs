using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Scripts.FirstLevel;
namespace Scripts.FirstLevel
{ 
public class PowerUp : MonoBehaviour {
	public List<GameObject> weapons = new List<GameObject>();
	public bool forHandGun;
	public bool forAssaultRifle;

        void OnTriggerEnter(Collider col)
        {

            if (col.tag == "Player")
            {

                foreach (GameObject go in weapons)
                {
                    if (forHandGun)
                    {
                        if (go.GetComponent<newGun>().handGun)
                        {

                            go.GetComponent<newGun>().currentAmmo += go.GetComponent<newGun>().maxMagazineAmmo;
                            Destroy(this.gameObject);
                        }
                    }

                    if (forAssaultRifle)
                    {
                        if (go.GetComponent<newGun>().assaultRifle)
                        {

                            go.GetComponent<newGun>().currentAmmo += go.GetComponent<newGun>().maxMagazineAmmo;
                            Destroy(this.gameObject);
                        }
                    }
                }
            }
        }
	}
}
