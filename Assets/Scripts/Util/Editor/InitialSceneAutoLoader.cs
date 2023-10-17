using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

[InitializeOnLoad]
public static class InitialSceneAutoLoader
{
    private static string _previousScenePath;

    static InitialSceneAutoLoader()
    {
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
    }

    private static void OnPlayModeStateChanged(PlayModeStateChange state)
    {
        switch (state)
        {
            case PlayModeStateChange.ExitingEditMode:
                EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
                break;

            case PlayModeStateChange.EnteredPlayMode
                when SceneManager.GetActiveScene().buildIndex == 0:
                return;

            case PlayModeStateChange.EnteredPlayMode:
                LoadInitialScene();
                break;

            case PlayModeStateChange.EnteredEditMode:
                LoadSceneFromWhichYouStarted();
                break;
        }
    }

    private static void LoadInitialScene()
    {
        _previousScenePath = SceneManager.GetActiveScene().path;

        try
        {
            SceneManager.LoadScene(0);
        }
        catch
        {
            Debug.LogError("Cannot load initial scene");
        }
    }

    private static void LoadSceneFromWhichYouStarted()
    {
        try
        {
            EditorSceneManager.OpenScene(_previousScenePath);
        }
        catch
        {
            Debug.LogError($"Cannot load scene: {_previousScenePath}");
        }
    }
}