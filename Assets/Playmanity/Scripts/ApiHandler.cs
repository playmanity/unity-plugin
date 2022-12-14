using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using UnityEditor;
using System;
using UnityEngine.SceneManagement;


public class ApiHandler : MonoBehaviour
{
    // public string ApiKey, Dada;
    public TMP_InputField EmailField, PasswordField;
    public TextMeshProUGUI LoginResult;

	private void Start()
	{
        LoginResult.text = "";
	}

	public void Login() 
    {
        /*
        if (EmailField.text.Length <= 3) 
        {
            print("Invalid email");
        }
        if (PasswordField.text.Length < 8)
        {
            print("Invalid password");
        }
        */
        LoginResult.text = "Logging in...";
        StartCoroutine(PostLogin());


    }

    [Serializable]
    class LoginForm 
    {
        public string username;
        public string password;
    }

    [Serializable]
    class LoginErrorMessage 
    {
        public string details;
        public string message;
    }

    [Serializable]
    class LoginSuccess 
    {
        public string value;
    }

    IEnumerator PostLogin() 
    {
        LoginForm form = new LoginForm();
        form.username = EmailField.text;
        form.password = PasswordField.text;
        string json = JsonUtility.ToJson(form);
        // Debug.Log(json);
        using (UnityWebRequest www = UnityWebRequest.Put("https://api.playmanity.com/sign-in", json))
        {
            www.method = "POST";
            www.SetRequestHeader("Content-Type", "application/json");
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                if (www.responseCode == 401)
                {
                    LoginErrorMessage ErrorMessage = JsonUtility.FromJson<LoginErrorMessage>(www.downloadHandler.text);   
                    LoginResult.text = ErrorMessage.message;
                }
                else
                {
                    LoginResult.text = "Login failed, unknown error";
                    Debug.LogError(www.error);
                
                }
            }
            else
            {
                LoginResult.text = "Logged in!";
                LoginSuccess Key = JsonUtility.FromJson<LoginSuccess>(www.downloadHandler.text);
                PlayerPrefs.SetString("PlaymanityAPIKey", Key.value);
                // 1 because this scene takes the 0 spot
                yield return new WaitForSeconds(0.5f);
                SceneManager.LoadScene(1);
            }
        }
    }

}


#if UNITY_EDITOR
public class PlaymanityMenu
{

    [MenuItem("Playmanity/Add ad handler")]
    static void AddAdHandler(MenuCommand menuCommand)
    {
        // Create a custom game
        GameObject go = Resources.Load<UnityEngine.Object>("Playmanity Ad Handler") as GameObject;
        GameObject newAdHandler = PrefabUtility.InstantiatePrefab(go) as GameObject;
        // Ensure it gets reparented if this was a context click (otherwise does nothing)
        // GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject);
        // Register the creation in the undo system
        // Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);
        Selection.activeObject = newAdHandler;
    }

    [MenuItem("Playmanity/Add login scene to build settings")]
    static void AddLoginScene(MenuCommand menuCommand) 
    {

        EditorBuildSettingsScene[] newScenes = new EditorBuildSettingsScene[EditorBuildSettings.scenes.Length + 1];
        string scenePath = AssetDatabase.GetAssetPath(Resources.Load<SceneAsset>("Playmanity Login"));
        EditorBuildSettingsScene newScene = new EditorBuildSettingsScene(scenePath, true);

        EditorBuildSettings.scenes.CopyTo(newScenes, 1);
        foreach (EditorBuildSettingsScene scene in newScenes) 
        {
            if (scene != null && scene.path == newScene.path) 
            {
                Debug.Log("Playmanity login scene is already in the build settings");
                return;
            }
        }
        newScenes[0] = newScene;
        EditorBuildSettings.scenes = newScenes;
        Debug.Log("Playmanity Login scene added");
    }
}

#endif

/*
[CustomEditor(typeof(ApiHandler))]
public class ApiHandlerEdito : Editor
{

    SerializedProperty apiKey;
    ApiStates ApiState;

    enum ApiStates 
    {
        NotSet,
        UnderValidation,
        Valid,
        Invalid
    }

    void OnEnable()
    {
        apiKey = serializedObject.FindProperty("ApiKey");
        ApiState = ApiStates.NotSet;
    }


    // Used to show a custom api key menu
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        /*
        if (ApiState == ApiStates.NotSet)
            EditorGUILayout.HelpBox("Input your api key", MessageType.Info);
        else if (ApiState == ApiStates.UnderValidation)
            EditorGUILayout.HelpBox("Validating...", MessageType.Info);
        else if (ApiState == ApiStates.Invalid)
            EditorGUILayout.HelpBox("Invalid Api key", MessageType.Error);
        else if (ApiState == ApiStates.Valid)
            EditorGUILayout.HelpBox("Api key validated", MessageType.None);
        *//*
        EditorGUILayout.PropertyField(apiKey);
        if (GUILayout.Button("Validate")) 
        {
            Array values = Enum.GetValues(typeof(ApiStates));
			System.Random random = new System.Random();
            ApiState = (ApiStates)values.GetValue(random.Next(values.Length));
        }
        serializedObject.ApplyModifiedProperties();
    }
}
*/