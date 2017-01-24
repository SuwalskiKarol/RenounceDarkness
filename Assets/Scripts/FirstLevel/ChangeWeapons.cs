using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Scripts.FirstLevel{
    /// <summary>
    /// Change weapon
    /// </summary>
    public class ChangeWeapons : MonoBehaviour {
        [Header ("Crosshair gameobject")]
        public SpriteRenderer cross;

        [Header("Camera gameobject")]
        public Transform camra;

        [Header("Weapons list")]
        public List<GameObject> weapons = new List<GameObject>();

        private int currentWeapon; 
        private bool change;

        void Start() {
            cross.transform.position = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height / 2, 1));

        }

        void Update() {
            ChangeWeapon();
        }

        void ChangeWeapon()
        {
            for (int i = 1; i <= weapons.Count; i++)
            {

                if (Input.GetKeyDown("" + i))
                {
                    currentWeapon = i - 1;
                    change = true;
                }
            }
            if (change)
            {
                for (int b = 0; b < weapons.Count; b++)
                {
                    if (b == currentWeapon)
                    {
                        weapons[b].GetComponent<newGun>().anim.SetBool("isChange", false);

                    }
                    else
                        weapons[b].GetComponent<newGun>().anim.SetBool("isChange", true);
                }
            }

        }
    }
}
