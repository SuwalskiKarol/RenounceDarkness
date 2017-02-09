using UnityEngine;
using System.Collections;
using UnityStandardAssets.Characters.FirstPerson;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections.Generic;
namespace Scripts.FirstLevel
{
    public class GameControl : MonoBehaviour
    {
        private string screenResolution;
        public int rand, resX, resY, score, dieCount;
        
        public bool canAudio = false, boom, canfireball = true;
        public AudioClip[] sounds;
        public float time = 2f;
        public GameObject[] audioOptions, graphicOptions, monsters, weapons, spawnPoints, monstersinScene, sparks;
        GameObject menu, options, lightt, playerr, gameovermenu, scoregameobject, daycycle, music, monsterCount;
        private Animator anim, animOptions, animGameOver;
        private AudioSource audioo;
        List<string> graphicgenerics = new List<string>();
        List<string> qualitysettings = new List<string>() { "High", "Medium", "Low" };
        List<string> antyaliasing = new List<string>() { "8x AA", "4x AA", "2x AA", "No AA" };
        List<string> refreshrate = new List<string>() { "120Hz", "60Hz" };

        void Start()
        {
            Cursor.lockState = CursorLockMode.None;
            monsterCount = GameObject.Find("DieCount");
            music = GameObject.Find("Music");
            Destroy(music);
            daycycle = GameObject.Find("day/night");
            StartCoroutine(Spawn());

            spawnPoints = GameObject.FindGameObjectsWithTag("SpawnPoint");
            scoregameobject = GameObject.Find("Scoretext");
            playerr = GameObject.Find("FPSController");
            Cursor.visible = false;
            screenResolution = Screen.currentResolution.width + "x" + Screen.currentResolution.height;
            graphicgenerics.Add(screenResolution);
            graphicgenerics.Add("1024x768");
            graphicgenerics.Add("800x600");
            graphicgenerics.Add("640x480");
            lightt = GameObject.Find("lighting");
            audioo = lightt.GetComponent<AudioSource>();
            menu = GameObject.Find("Menu");
            anim = menu.GetComponent<Animator>();
            options = GameObject.Find("Options");
            animOptions = options.GetComponent<Animator>();
            gameovermenu = GameObject.Find("GameOver");
            animGameOver = gameovermenu.GetComponent<Animator>();
            graphicOptions[1].GetComponent<Dropdown>().AddOptions(graphicgenerics);
            graphicOptions[3].GetComponent<Dropdown>().AddOptions(qualitysettings);
            graphicOptions[4].GetComponent<Dropdown>().AddOptions(antyaliasing);
            graphicOptions[7].GetComponent<Dropdown>().AddOptions(refreshrate);
        }


        void Update()
        {
            Cursor.lockState = CursorLockMode.None;
            foreach (GameObject go in monstersinScene)
            {
                if (go == null)
                {
                    dieCount += 1;
                }
            }
            sparks = GameObject.FindGameObjectsWithTag("Spark");
            monsterCount.GetComponent<Text>().text = "Monsters killed: " + dieCount;
            foreach (GameObject go in sparks)
            {
                if (!go.GetComponent<ParticleSystem>().IsAlive())
                {
                    Destroy(go);
                }
            }

            if (/*daycycle.transform.rotation.x >= 150*/ RenderSettings.ambientIntensity >= 1.5)
            {
                animGameOver.SetBool("isExitMenu", true);
                GameObject over = GameObject.Find("GameoverText");
                over.GetComponent<Text>().text = "YOU WIN";

                daycycle.GetComponent<AutoIntensity>().enabled = false;
                Cursor.visible = true;

                GameObject player = GameObject.Find("FPSController");
                player.GetComponent<FirstPersonController>().enabled = false;
                foreach (GameObject go in weapons)
                {
                    go.GetComponent<newGun>().enabled = false;
                    go.GetComponent<Animator>().SetBool("isReloading", true);
                    go.GetComponent<Animator>().SetBool("isDying", true);

                }
                foreach (GameObject go in monstersinScene)
                {
                    canfireball = false;
                    if (go.GetComponent<UnityEngine.AI.NavMeshAgent>() != null)
                        go.GetComponent<UnityEngine.AI.NavMeshAgent>().speed = 0f;
                }

            }
            monstersinScene = GameObject.FindGameObjectsWithTag("Enemy");

            //	scoregameobject.GetComponent<Text> ().text =""+ score;
            if (graphicOptions[2].GetComponent<Toggle>().isOn == true)
            {

                Screen.fullScreen = true;
            }
            else
            {

                Screen.fullScreen = false;

            }
            if (graphicOptions[1].GetComponent<Dropdown>().value == 0)
            {
                if (graphicOptions[2].GetComponent<Toggle>().isOn == true)
                {
                    Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, true);
                    resX = Screen.currentResolution.width;
                    resY = Screen.currentResolution.height;
                }
                else
                {
                    Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, false);
                    resX = Screen.currentResolution.width;
                    resY = Screen.currentResolution.height;
                }


            }
            if (graphicOptions[1].GetComponent<Dropdown>().value == 1)
            {
                if (graphicOptions[2].GetComponent<Toggle>().isOn == true)
                {
                    Screen.SetResolution(1024, 768, true);
                    resX = 1024;
                    resY = 768;
                }
                else
                    Screen.SetResolution(1024, 768, false);
                {
                    resX = 1024;
                    resY = 768;
                }
            }
            if (graphicOptions[1].GetComponent<Dropdown>().value == 2)
            {
                if (graphicOptions[2].GetComponent<Toggle>().isOn == true)
                {
                    Screen.SetResolution(800, 600, true);
                    resX = 800;
                    resY = 600;
                }
                else
                {
                    Screen.SetResolution(800, 600, false);
                    resX = 800;
                    resY = 600;
                }
            }
            if (graphicOptions[1].GetComponent<Dropdown>().value == 3)
            {
                if (graphicOptions[2].GetComponent<Toggle>().isOn == true)
                {
                    Screen.SetResolution(640, 480, true);
                    resX = 640;
                    resY = 480;
                }
                else
                {
                    Screen.SetResolution(640, 480, false);
                    resX = 640;
                    resY = 480;
                }
            }


            if (graphicOptions[3].GetComponent<Dropdown>().value == 0)
            {
                QualitySettings.SetQualityLevel(5);

            }
            if (graphicOptions[3].GetComponent<Dropdown>().value == 1)
            {
                QualitySettings.SetQualityLevel(2);

            }
            if (graphicOptions[3].GetComponent<Dropdown>().value == 2)
            {
                QualitySettings.SetQualityLevel(0);

            }



            if (graphicOptions[4].GetComponent<Dropdown>().value == 0)
            {
                QualitySettings.antiAliasing = 8;
            }
            if (graphicOptions[4].GetComponent<Dropdown>().value == 1)
            {
                QualitySettings.antiAliasing = 4;

            }
            if (graphicOptions[4].GetComponent<Dropdown>().value == 2)
            {
                QualitySettings.antiAliasing = 2;

            }
            if (graphicOptions[4].GetComponent<Dropdown>().value == 3)
            {
                QualitySettings.antiAliasing = 0;

            }

            if (graphicOptions[5].GetComponent<Toggle>().isOn == true)
            {

                QualitySettings.anisotropicFiltering = AnisotropicFiltering.ForceEnable;
            }
            else
            {

                QualitySettings.anisotropicFiltering = AnisotropicFiltering.Disable;

            }

            if (graphicOptions[6].GetComponent<Toggle>().isOn == true)
            {

                QualitySettings.vSyncCount = 1;
            }
            else
            {

                QualitySettings.vSyncCount = 0;

            }

            if (graphicOptions[7].GetComponent<Dropdown>().value == 1)
            {
                if (graphicOptions[2].GetComponent<Toggle>().isOn == true)
                    Screen.SetResolution(resX, resY, true, 120);
                else
                    Screen.SetResolution(resX, resY, false, 120);
            }
            if (graphicOptions[7].GetComponent<Dropdown>().value == 2)
            {
                if (graphicOptions[2].GetComponent<Toggle>().isOn == true)
                    Screen.SetResolution(resX, resY, true, 60);
                else
                    Screen.SetResolution(resX, resY, false, 60);

            }

            if (graphicOptions[8].GetComponent<Toggle>().isOn == true)
            {

                QualitySettings.maxQueuedFrames = 3;
            }
            else
            {

                QualitySettings.maxQueuedFrames = 0;

            }


            rand = Random.Range(0, 50);
            if (boom)
            {
                if (rand == 5)
                {

                    lightt.GetComponent<Light>().enabled = true;
                    audioo.clip = sounds[Random.Range(0, sounds.Length)];


                    canAudio = true;
                }
                else
                {
                    lightt.GetComponent<Light>().enabled = false;

                }
            }
            if (!audioo.isPlaying)
            {
                audioo.enabled = false;
                boom = true;


            }
            else
            {
                boom = false;
                time = 2;
            }

            if (canAudio)
            {
                time -= Time.deltaTime;
                if (time <= 0)
                {
                    audioo.enabled = true;

                    canAudio = false;

                }
            }

            //if(time <0)
            //	time = 2;

            if (Input.GetKey(KeyCode.Escape) && playerr.GetComponent<Player>().HP > 0)
            {

                daycycle.GetComponent<AutoIntensity>().enabled = false;
                Cursor.visible = true;
                anim.SetBool("isPaused", true);
                GameObject player = GameObject.Find("FPSController");
                player.GetComponent<FirstPersonController>().enabled = false;
                foreach (GameObject go in weapons)
                {
                    go.GetComponent<newGun>().enabled = false;
                }
                foreach (GameObject go in monstersinScene)
                {
                    canfireball = false;
                    if (go.GetComponent<UnityEngine.AI.NavMeshAgent>() != null)
                        go.GetComponent<UnityEngine.AI.NavMeshAgent>().speed = 0f;
                }
            }


            if (playerr.GetComponent<Player>().HP <= 0)
            {

                GameObject player = GameObject.Find("FPSController");
                player.GetComponent<FirstPersonController>().enabled = false;
                GameObject player2 = GameObject.Find("FirstPersonCharacter");
                player2.GetComponent<Animator>().enabled = true;
                foreach (GameObject go in weapons)
                {
                    go.GetComponent<newGun>().enabled = false;
                    go.GetComponent<Animator>().SetBool("isReloading", true);
                    go.GetComponent<Animator>().SetBool("isDying", true);
                }
                foreach (GameObject go in monstersinScene)
                {
                    canfireball = false;
                    if (go.GetComponent<UnityEngine.AI.NavMeshAgent>() != null)
                        go.GetComponent<UnityEngine.AI.NavMeshAgent>().speed = 0f;
                }
                if (player2.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("PlayerDie"))
                {
                    Debug.Log("DIEEEEEEe");

                    Cursor.visible = true;
                    animGameOver.SetBool("isExitMenu", true);


                }
            }
        }
        public void ContinueButton()
        {
            GameObject daycycle = GameObject.Find("day/night");
            daycycle.GetComponent<AutoIntensity>().enabled = true;
            Cursor.visible = false;
            anim.SetBool("isPaused", false);
            GameObject player = GameObject.Find("FPSController");
            player.GetComponent<FirstPersonController>().enabled = true;
            foreach (GameObject go in weapons)
            {
                go.GetComponent<newGun>().enabled = true;
            }
            foreach (GameObject go in monstersinScene)
            {
                canfireball = true;
                if (go.GetComponent<UnityEngine.AI.NavMeshAgent>() != null)
                    go.GetComponent<UnityEngine.AI.NavMeshAgent>().speed = 3.5f;
            }
        }

        public void OptionsButton()
        {
            anim.SetBool("isPaused", false);
            animOptions.SetBool("isOptions", true);

        }

        public void AudioButton()
        {
            GameObject music = GameObject.Find("Music");
            foreach (GameObject go in graphicOptions)
            {
                go.SetActive(false);
            }
            foreach (GameObject go in audioOptions)
            {
                go.SetActive(true);
            }

        }

        public void ExitButton()
        {
            SceneManager.LoadScene("menu");
        }

        public void OptionsBackButton()
        {
            animOptions.SetBool("isOptions", false);
            anim.SetBool("isPaused", true);

        }

        public void GraphicsButton()
        {
            foreach (GameObject go in audioOptions)
            {
                go.SetActive(false);
            }

            foreach (GameObject go in graphicOptions)
            {
                go.SetActive(true);
            }



            //Debug.Log (Screen.currentResolution);
        }

        public void NewGameButton()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        //Screen.SetResolution (Screen.currentResolution.width, Screen.currentResolution.height, true);



        private IEnumerator Spawn()
        {
            while (true)
            {
                yield return new WaitForSeconds(15);
                Instantiate(monsters[Random.Range(0, monsters.Length)], spawnPoints[Random.Range(0, spawnPoints.Length)].transform.position, Quaternion.identity);

            }
        }
    }
}
