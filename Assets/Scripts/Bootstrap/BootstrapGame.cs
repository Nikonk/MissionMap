using System.Collections.Generic;
using System.Threading.Tasks;
using MissionMap.Core;
using MissionMap.Hero;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace MissionMap.Bootstrap
{
    public class BootstrapGame : MonoBehaviour
    {
        private const string _completionText = "Loading is complete\nClick on screen to continue";

        [SerializeField] private GameObject _uiParent;
        [SerializeField] private TMP_Text _loadingText;
        [SerializeField] private Button _switchSceneButton;

        [SerializeField] private Camera _mainCamera;

        private DataHandler _dataHandler;

        private void Start()
        {
            _dataHandler = DataHandler.Instance;
            LoadGame();
        }

        private async void LoadGame()
        {
            LoadMission();

            AsyncOperation loadSceneOperation = SceneManager.LoadSceneAsync(1, LoadSceneMode.Additive);
            loadSceneOperation.allowSceneActivation = false;

            while (loadSceneOperation.isDone == false)
            {
                if (loadSceneOperation.progress >= 0.9f)
                {
                    _loadingText.text = _completionText;
                    _switchSceneButton.interactable = true;
                    _switchSceneButton.onClick.AddListener(() => LoadNextScene(loadSceneOperation));
                }

                await Task.Yield();
            }
        }

        private void LoadMission()
        {
            MissionMapNodeData missionMapNodeData1 = new MissionMapNodeData(
                "1",
                "Fly",
                "Start fly",
                "You fly",
                true,
                new List<List<HeroType>>()
                {
                    new List<HeroType> { HeroType.Gull }
                },
                new List<List<HeroType>>()
                {
                    new List<HeroType> { HeroType.Gull }
                },
                new List<HeroType>() { HeroType.Crow },
                new Dictionary<HeroType, int>() { { HeroType.Gull, 110 } },
                null);

            MissionMapNodeData missionMapNodeData2 = new MissionMapNodeData(
                "2",
                "Fly2",
                "Start fly2",
                "You fly2",
                false,
                new List<List<HeroType>>()
                {
                    new List<HeroType> { HeroType.Gull }
                },
                new List<List<HeroType>>()
                {
                    new List<HeroType> { HeroType.Gull }
                },
                new List<HeroType>() { HeroType.Owl },
                new Dictionary<HeroType, int>() { { HeroType.Myself, 10 }, { HeroType.Owl, 5 } },
                new List<string>() {"3.1"});

            MissionMapNodeData missionMapNodeData3_1 = new MissionMapNodeData(
                "3.1",
                "Walk",
                "Start walk",
                "You walk",
                false,
                new List<List<HeroType>>()
                {
                    new() { HeroType.Gull }
                },
                new List<List<HeroType>>()
                {
                    new() { HeroType.Gull }
                },
                new List<HeroType>() { HeroType.Crow },
                new Dictionary<HeroType, int>() { { HeroType.Myself, 10 } },
                new List<string>() {"1"});
            MissionMapNodeData missionMapNodeData3_2 = new MissionMapNodeData(
                "3.2",
                "Fly",
                "Start fly",
                "You fly",
                false,
                new List<List<HeroType>>()
                {
                    new List<HeroType> { HeroType.Jackdaw }
                },
                new List<List<HeroType>>()
                {
                    new List<HeroType> { HeroType.Gull }
                },
                new List<HeroType>() { HeroType.Crow },
                new Dictionary<HeroType, int>() { { HeroType.Myself, 10 } },
                new List<string>() {"1"});

            _dataHandler.Initialize(new List<List<MissionMapNodeData>>
                {
                    new() { missionMapNodeData1 },
                    new() { missionMapNodeData3_1, missionMapNodeData3_2 },
                    new() { missionMapNodeData2 }
                });
        }

        private void LoadNextScene(AsyncOperation operation)
        {
            Destroy(_uiParent);
            Destroy(gameObject);
            _mainCamera.backgroundColor = Color.white;
            operation.allowSceneActivation = true;
        }
    }
}