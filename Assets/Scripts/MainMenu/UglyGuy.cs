using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Scripts.MainMenu {
    /// <summary>
    /// Monster animation controller
    /// </summary>
    public class UglyGuy : MonoBehaviour {
        public int rand;
        private Animator anim;
        private List<int> numbers = new List<int>() { 1,2,3};
        private List<string> notices = new List<string>() { "notice", "notice2", "notice3" };
        public int Rand
        {
            get{ return rand = Random.Range(1, 1000);}
            set{ rand = value;}
        }

        public AudioClip[] clip;
        private AudioSource audioo;
      
        void Start() {
            anim = GetComponent<Animator>();
            audioo = GetComponent<AudioSource>();
        }

        
        void Update() {
            for(int i =0; i<2; i++)
                ChangeAnim(Rand,numbers[i], notices[i]);
        }
        /// <summary>
        /// Randomly change monster animation
        /// </summary>
        /// <param name="random"></param>
        /// <param name="number"></param>
        /// <param name="notice"></param>
        void ChangeAnim(int random, int number, string notice)
        {
            if (random == number)
            {
                if (audioo.isPlaying == false)
                {
                    anim.SetBool(notice, true);
                    audioo.clip = clip[Random.Range(0, clip.Length)];
                    audioo.Play();
                }
            }
            else
                anim.SetBool(notice, false);
        }
    }
}
