using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
namespace Scripts.MainMenu
{
    /// <summary>
    /// Main menu control
    /// </summary>
    public class MainMenu : MonoBehaviour
    {
        [Header("Music Game Object")]
        public GameObject music;
        private GameObject tekstExit, tekstNewGame, tekstInstructions, controlsAndObjectives;
        private AudioSource audioo;

        void Start()
        {
            Cursor.lockState = CursorLockMode.None;
            audioo = gameObject.GetComponent<AudioSource>();
            Cursor.visible = true;
            tekstExit = GameObject.Find("ExitText");
            tekstNewGame = GameObject.Find("NewGameText");
            tekstInstructions = GameObject.Find("InstructionsText");
            controlsAndObjectives = GameObject.Find("controls&objectives");
            music = GameObject.Find("Music");

            if (music == null)
            {
                audioo.enabled = true;
            }
        }

        void Update()
        {
            if (tekstExit.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("ExitTextClick"))
            {
                tekstExit.GetComponent<Animator>().SetBool("isExit", false);
                Application.Quit();
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#endif
            }
            if (tekstNewGame.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("NewGameClick"))
            {
                Debug.Log("play");
                tekstNewGame.GetComponent<Animator>().SetBool("isNewGame", false);
                SceneManager.LoadScene("1");
            }
            if (tekstInstructions.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("InstructionsClick"))
            {
                tekstInstructions.GetComponent<Animator>().SetBool("isInstructions", false);
                controlsAndObjectives.GetComponent<Animator>().SetBool("ShowMe", true);
                if (controlsAndObjectives.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f && controlsAndObjectives.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("controls&objectivesStart"))
                {
                    controlsAndObjectives.GetComponent<Animator>().SetBool("ShowMe", false);
                }
            }
        }
        /// <summary>
        /// Quit button control
        /// </summary>
        public void QuitButton()
        {
            tekstExit.GetComponent<Animator>().SetBool("isExit", true);
        }
        /// <summary>
        /// New game button control
        /// </summary>
        public void NewGameButton()
        {
            Debug.Log("play");
            tekstNewGame.GetComponent<Animator>().SetBool("isNewGame", true);
        }
        /// <summary>
        /// Instruction button control
        /// </summary>
        public void InstructionsButton()
        {
            tekstInstructions.GetComponent<Animator>().SetBool("isInstructions", true);
        }
    }
}
