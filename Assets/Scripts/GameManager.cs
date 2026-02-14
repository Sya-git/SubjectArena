using System;
using SubjectArena.Player;
using UnityEngine;

namespace SubjectArena
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance;
        
        public PlayerController Player { get; set; }

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
            }
            else
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
        }
    }
}