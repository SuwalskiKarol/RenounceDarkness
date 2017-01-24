using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using Scripts.TitleScreen;

namespace Scripts.TitleScreen
{
    /// <summary>
    /// Displays title 
    /// </summary>
    public class Title : MonoBehaviour
    {
        private GameObject title;
    
        void Start()
        {
            title = GameObject.Find("Title");
            Cursor.visible = false;
        }

        void Update()
        {
            if ((title.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("TitleShow") && 
                title.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f) || Input.GetMouseButton(0))
                    SceneManager.LoadScene("menu");
                
        }

    }
   
}
