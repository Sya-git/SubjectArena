using System;
using System.IO;
using SubjectArena.Combat.UI;
using SubjectArena.Items.Inventory.UI;
using SubjectArena.Player;
using Unity.Cinemachine;
using UnityEngine;

namespace SubjectArena.Managers
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private SpawnerManager spawnerManager;
        [SerializeField] private Transform playerSpawnPoint;
        [SerializeField] private PlayerController playerPrefab;
        [SerializeField] private CanvasInventoryManager canvasInventoryManager;
        [SerializeField] private CanvasHealthBar playerHealthBar;
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
            Player.Health.OnDeath += OnPlayerDeath;
            canvasInventoryManager.Initialize(Player);
            playerHealthBar.Initialize(Player.Health);
            cinemachineCamera.Target = new CameraTarget() { TrackingTarget = Player.transform };
            LoadSaveDataFromDisk();
        }

        private void OnPlayerDeath()
        {
            spawnerManager.StopSpawn();
        }

        private const string SaveFileName = "save";
        private static string SaveFilePath => Path.Combine(Application.persistentDataPath, SaveFileName);
        private void SaveDataToDisk()
        {
            try
            {
                var saveData = Player.GetSaveData();
                if (!string.IsNullOrEmpty(saveData))
                {
                    File.WriteAllText(SaveFilePath, saveData);
                }
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        private void LoadSaveDataFromDisk()
        {
            try
            {
                if (!File.Exists(SaveFilePath))
                {
                    return;
                }
                
                var saveData = File.ReadAllText(SaveFilePath);
                if (!string.IsNullOrEmpty(saveData))
                {
                    Player.LoadSaveData(saveData);
                }
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }
        
        private void OnApplicationQuit()
        {
            SaveDataToDisk();
        }
    }
}