using System.Threading.Tasks;
using TMPro;
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

        private void Start() => LoadGame();

        private async void LoadGame()
        {
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

        private void LoadNextScene(AsyncOperation operation)
        {
            Destroy(_uiParent);
            Destroy(gameObject);
            _mainCamera.backgroundColor = Color.white;
            operation.allowSceneActivation = true;
        }
    }
}