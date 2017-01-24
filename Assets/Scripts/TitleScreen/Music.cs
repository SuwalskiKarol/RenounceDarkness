using UnityEngine;
using System.Collections;

namespace Scripts.TitleScreen
{
    /// <summary>
    /// Music still play after scene change
    /// </summary>
    public class Music : MonoBehaviour
    {
        void Awake()
        {
            DontDestroyOnLoad(this.gameObject);
        }
    }
}


