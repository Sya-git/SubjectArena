using System;
using SubjectArena.Player;
using SubjectArena.UI.Inventory;
using Unity.Cinemachine;
using UnityEngine;

namespace SubjectArena
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private Transform playerSpawnPoint;
        [SerializeField] private PlayerController playerPrefab;
        [SerializeField] private CanvasInventoryManager canvasInventoryManager;
        [SerializeField] private CinemachineCamera cinemachineCamera;
        
        public static GameManager Instance;
        
        public PlayerController Player { get; private set; }

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

        private void Start()
        {
            Player = Instantiate(playerPrefab, playerSpawnPoint.position, playerSpawnPoint.rotation);
            canvasInventoryManager.Initialize(Player);
            cinemachineCamera.Target = new CameraTarget() { TrackingTarget = Player.transform };
        }
    }
}