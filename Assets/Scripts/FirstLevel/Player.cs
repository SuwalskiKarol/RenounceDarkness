using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Scripts.FirstLevel;
namespace Scripts.FirstLevel
{
    public class Player : MonoBehaviour
    {

        public bool getHit = false;
        public AudioClip[] hitSounds;
        public int HP;
        public int damageTaken;
        GameObject HPbar, hitSoundsObject;
        AudioSource audioo;
        GameControl getHitScript;
        public int rand;

        void Start()
        {

            hitSoundsObject = GameObject.Find("FirstPersonCharacter");
            HP = 100;
            damageTaken = 0;
            HPbar = GameObject.Find("HPbar");
            audioo = hitSoundsObject.GetComponent<AudioSource>();
        }

        void Update()
        {
            if (getHit)
            {
                audioo.clip = hitSounds[Random.Range(0, hitSounds.Length)];
                StartCoroutine(Wait());
                getHit = false;

            }
        }

        IEnumerator Wait()
        {
            audioo.Play();
            if (audioo.clip != null)
                yield return new WaitForSeconds(audioo.clip.length);
        }
        public void DamagePlayer()
        {
            HP -= 5;
            damageTaken += 5;
            HPbar.GetComponent<Image>().fillAmount -= 0.05f;
            getHit = true;
        }


        void OnTriggerEnter(Collider col)
        {
            Vector3 moveDirection = Vector3.zero;
            int climbspeed = 2;
            if (col.gameObject.name == "Ladder")
            {
                Debug.Log("drabinna");

            }
        }
    }
}


