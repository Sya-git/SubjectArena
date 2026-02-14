using System;
using System.IO;
using SubjectArena.Player;
using SubjectArena.UI.Inventory;
using Unity.Cinemachine;
using Unity.VisualScripting;
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
            LoadSaveDataFromDisk();
        }

        private const string _saveFileName = "save";
        private static string SaveFilePath => Path.Combine(Application.persistentDataPath, _saveFileName);
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