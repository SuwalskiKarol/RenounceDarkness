using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Scripts.FirstLevel;
namespace Scripts.FirstLevel
{
    public class Gun : MonoBehaviour
    {
        public Animator anim;
        public GameObject gunFire;
        ParticleSystem particle;
        AudioSource sound;
        public float fireTime;
        public bool fire;
        public GameObject cross;
        ChangeWeapons skrypt;
        public float colorchange = 0.2f;
        public GameObject[] enemies;
        public bool redCross;
        public int maxAmmo;
        public int ammo;
        public float actualAmmo;
        public Animation change;
        public bool HandGun;
        public bool AssaultRifle;
        public bool isEquipment;
        public RaycastHit hit;
        public Ray ray;
        float reloadTime;
        public float bulletsTime = 5f;
        bool reload;
        public List<GameObject> weapons = new List<GameObject>();
        void Start()
        {
            anim = GetComponent<Animator>();
            particle = gunFire.GetComponent<ParticleSystem>();
            sound = GetComponent<AudioSource>();
            skrypt = cross.GetComponent<ChangeWeapons>();
            enemies = GameObject.FindGameObjectsWithTag("Enemy");
            //weapons.Add (GameObject.FindWithTag ("Gun"));
            change = GetComponent<Animation>();
            if (anim.GetBool("isChange"))
            {
                anim.Play("ChangeAssault", 0, 1f);
            }
            actualAmmo = ammo;
        }

        void Update()
        {
            ray = Camera.main.ScreenPointToRay(Camera.main.WorldToScreenPoint(cross.transform.position));

            foreach (GameObject go in weapons)
            {
                if (isEquipment)
                {
                    if (Input.GetKey("w") || Input.GetKey("s") || Input.GetKey("a") || Input.GetKey("d"))
                    {
                        anim.SetBool("isWalking", true);
                        if (Input.GetKey(KeyCode.LeftShift))
                        {
                            fireTime = 1f;
                            fire = false;
                            anim.SetBool("isRuning", true);
                            skrypt.cross.color = Color.blue;
                        }
                        else
                        {

                            anim.SetBool("isRuning", false);
                        }
                    }
                    else
                    {
                        //fireTime = 0f;
                        anim.SetBool("isRuning", false);
                        anim.SetBool("isWalking", false);
                    }

                    if (Input.GetMouseButtonDown(0))
                    {

                        colorchange = 1f;
                        fire = true;
                        if (fireTime <= 0)
                        {
                            redCross = true;
                            if (HandGun)
                                fireTime = 1.3f;
                            if (AssaultRifle)
                            {

                                fireTime = 0.4f;
                            }
                            sound.Play();
                            anim.SetBool("isShooting", true);
                            particle.Play();
                        }
                        else
                            redCross = false;

                    }
                    else
                    {
                        if (HandGun)
                        {
                            anim.SetBool("isShooting", false);
                            skrypt.cross.color = Color.blue;
                        }
                    }

                    if (AssaultRifle)
                    {
                        //redCross = true;
                        if (Input.GetMouseButton(0))
                        {
                            colorchange = 0.2f;
                            fire = true;
                            if (fireTime <= 0)
                            {

                                fireTime = 0.4f;

                                sound.Play();
                                anim.SetBool("isShooting", true);
                                particle.Play();
                            }

                        }
                        else
                        {
                            anim.SetBool("isShooting", false);
                            //redCross = false;
                            skrypt.cross.color = Color.blue;

                        }
                    }


                    if (fire)
                    {
                        if (/*anim.GetCurrentAnimatorStateInfo(0).IsName("FireAssault")*/anim.GetBool("isShooting"))
                        {
                            Reload();
                        }
                        fireTime -= Time.deltaTime;
                        colorchange -= Time.deltaTime;
                        if (Physics.Raycast(ray, out hit, 100))
                        {

                            if (hit.transform.tag == "Enemy")
                            {

                                if (redCross)
                                {
                                    if (colorchange >= 0)
                                    {
                                        skrypt.cross.color = Color.red;

                                    }
                                    else
                                    {
                                        skrypt.cross.color = Color.blue;
                                    }

                                    //Debug.Log ("lololol");
                                }
                            }
                            else
                                //colorchange = 0.2f;
                                skrypt.cross.color = Color.blue;

                        }

                        if (AssaultRifle)
                        {

                        }
                    } //else
                      //colorchange = 0.2f;
                      //fireTime = 0;




                }
                if (anim.GetBool("isChange"))
                {
                    isEquipment = false;

                }
                else
                    isEquipment = true;

                if (actualAmmo <= 0 || maxAmmo <= 0)
                {
                    actualAmmo = 0;
                    fire = false;
                    fireTime = 1f;
                    anim.SetBool("isShooting", false);
                    skrypt.cross.color = Color.blue;
                }

                if (maxAmmo > 0)
                {
                    if (Input.GetKeyDown(KeyCode.R))
                    {

                        reload = true;
                    }
                    if (reload)
                    {
                        fireTime = 1f;
                        bulletsTime -= Time.deltaTime;
                        anim.SetBool("isReloading", true);
                        if (bulletsTime <= 0)
                        {
                            anim.SetBool("isReloading", false);
                            bulletsTime = 5f;
                            actualAmmo = ammo;
                            maxAmmo -= ammo;
                            reload = false;
                        }
                    }
                }

            }
        }

        void Reload()
        {
            if (AssaultRifle)
            {
                reloadTime -= Time.deltaTime;
                if (reloadTime <= 0)
                {
                    reloadTime = 0.4f;
                    actualAmmo -= 1;
                }
            }

            if (HandGun)
            {
                actualAmmo -= 0.5f;
            }
        }
    }
}
