using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

[InitializeOnLoad]
public static class InitialSceneAutoLoader
{
    private const string _previousScenePrefsKey = "Previous scene";

    static InitialSceneAutoLoader()
    {
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
    }

    private static void OnPlayModeStateChanged(PlayModeStateChange state)
    {
        switch (state)
        {
            case PlayModeStateChange.ExitingEditMode:
                LoadInitialScene();
                break;

            case PlayModeStateChange.EnteredEditMode:
                LoadSceneFromWhichYouStarted();
                break;
        }
    }

    private static void LoadInitialScene()
    {
        EditorPrefs.SetString(_previousScenePrefsKey, SceneManager.GetActiveScene().path);

        if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo() == false)
        {
            EditorApplication.isPlaying = false;
            return;
        }

        if (SceneManager.GetActiveScene().buildIndex == 0)
            return;


        try
        {
            EditorSceneManager.OpenScene("Assets/Scenes/BootstrapScene.unity", OpenSceneMode.Single);
        }
        catch
        {
            Debug.LogError("Cannot load initial scene");
        }
    }

    private static void LoadSceneFromWhichYouStarted()
    {
        string previousScenePath = EditorPrefs.GetString(_previousScenePrefsKey);

        try
        {
            EditorSceneManager.OpenScene(previousScenePath);
        }
        catch
        {
            Debug.LogError($"Cannot load scene: {previousScenePath}");
        }
    }
}