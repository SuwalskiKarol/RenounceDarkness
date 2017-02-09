using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Scripts.FirstLevel;
namespace Scripts.FirstLevel
{
    //the worst script ever xD
    
    public class newGun : MonoBehaviour
    {
        public Animator anim;
        ParticleSystem particle;
        public GameObject gunFire, cross;
        AudioSource sound;
        public RaycastHit hit;
        public Ray ray;
        ChangeWeapons skrypt;
        public List<GameObject> weapons = new List<GameObject>();
        public bool isEquipment, handGun, assaultRifle, canFire, coloring, reloading;
        public float fireTime = 0;
        public int maxAmmo, currentAmmo, magazineAmmo, maxMagazineAmmo, emptyAmmoSlots;
        public float reloadTime = 3f, crossWait = 1f;
        // Use this for initialization
        void Start()
        {
            anim = GetComponent<Animator>();
            particle = gunFire.GetComponent<ParticleSystem>();
            sound = GetComponent<AudioSource>();
            skrypt = cross.GetComponent<ChangeWeapons>();
            if (anim.GetBool("isChange"))
            {
                anim.Play("Change", 0, 1f);

            }
        }

        // Update is called once per frame
        void Update()
        {
            foreach (GameObject go in weapons)
            {
                if (isEquipment)
                {
                    Moving();

                    if (assaultRifle)
                    {
                        Reload();
                        CantShoot();
                        if (canFire)
                        {
                            fireTime -= Time.deltaTime;
                            if (fireTime <= 0)
                            {

                                if (Input.GetMouseButton(0))
                                {
                                    CrossColor();
                                    magazineAmmo -= 1;
                                    sound.Play();
                                    anim.SetBool("isShooting", true);
                                    particle.Play();
                                    fireTime = 0.5f;

                                }
                                else
                                {
                                    anim.SetBool("isShooting", false);

                                }
                            }
                        }
                        else
                            anim.SetBool("isShooting", false);
                    }

                    if (handGun)
                    {
                        Reload();
                        CantShoot();
                        //HandGunFire ();
                        if (canFire)
                        {
                            fireTime -= Time.deltaTime;
                            if (fireTime <= 0)
                            {

                                if (Input.GetMouseButtonDown(0))
                                {
                                    CrossColor();
                                    magazineAmmo -= 1;
                                    sound.Play();
                                    anim.SetBool("isShooting", true);
                                    particle.Play();

                                    fireTime = 2f;

                                }
                                else
                                {
                                    anim.SetBool("isShooting", false);

                                }
                            }
                        }
                        else
                            anim.SetBool("isShooting", false);
                    }
                }

                ChangeWeapon();
                if (reloading)
                {

                    reloadTime -= Time.deltaTime;
                    if (reloadTime <= 0)
                    {
                        anim.SetBool("isReloading", false);
                        reloadTime = 3f;
                        reloading = false;
                    }

                }

                if (coloring)
                {
                    crossWait -= Time.deltaTime;
                    skrypt.cross.color = Color.red;
                    hit.transform.GetComponent<Monster>().hitting = true;
                    if (crossWait <= 0)
                    {
                        skrypt.cross.color = Color.blue;
                        crossWait = 0.3f;
                        coloring = false;
                        hit.transform.GetComponent<Monster>().hitting = false;
                    }

                }
            }
        }

        void Moving()
        {
            if (Input.GetKey("w") || Input.GetKey("s") || Input.GetKey("a") || Input.GetKey("d"))
            {
                anim.SetBool("isWalking", true);
                if (Input.GetKey(KeyCode.LeftShift))
                {
                    anim.SetBool("isRuning", true);

                    anim.SetBool("isShooting", false);
                }
                else
                {

                    anim.SetBool("isRuning", false);
                }
            }
            else
            {

                anim.SetBool("isWalking", false);
                anim.SetBool("isRuning", false);

            }
        }

        void CantShoot()
        {
            if (magazineAmmo <= 0 || Input.GetKey(KeyCode.LeftShift) || /*anim.GetBool("isReloading")*/ reloading == true)
                canFire = false;
            else
                canFire = true;
        }
        public void CrossColor()
        {

            ray = Camera.main.ScreenPointToRay(Camera.main.WorldToScreenPoint(cross.transform.position));
            if (Physics.Raycast(ray, out hit, 100))
            {

                if (hit.transform.tag == "Enemy")
                {
                    coloring = true;

                }

            }
        }

        void ChangeWeapon()
        {
            if (anim.GetBool("isChange"))
            {
                isEquipment = false;
            }
            else
                isEquipment = true;

        }

        void Reload()
        {


            if (maxMagazineAmmo != magazineAmmo && currentAmmo > 0)
            {
                if (Input.GetKeyDown(KeyCode.R))
                {
                    GameObject reload = GameObject.Find("GunCamera");
                    reload.GetComponent<AudioSource>().Play();
                    reloading = true;
                    emptyAmmoSlots = maxMagazineAmmo - magazineAmmo;
                    currentAmmo -= emptyAmmoSlots;
                    magazineAmmo = maxMagazineAmmo;
                    anim.SetBool("isReloading", true);

                }
            }
            if (currentAmmo < 0)
                currentAmmo = 0;
        }

        IEnumerator Wait()
        {
            yield return new WaitForSeconds(1);
        }
    }
}
