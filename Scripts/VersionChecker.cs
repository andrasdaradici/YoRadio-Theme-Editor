using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System.Diagnostics;

public class VersionChecker : MonoBehaviour
{
    // The URL to the version file
    private const string versionUrl = "http://andrewstudios.go.ro/YoRadio/Builds/version.txt";

    // Boolean to indicate if an update is available
    public bool isUpdateAvailable = false;

    // The current version of your application
    // Ideally, you'd get this from your build settings or a configuration file
    private string currentVersion = "1.0.0"; // Replace with your app's current version

    public GameObject UpdateMessage;
    void Start()
    {
        currentVersion = Application.version;
        StartCoroutine(CheckVersion());
    }

    private IEnumerator CheckVersion()
    {
        UnityWebRequest request = UnityWebRequest.Get(versionUrl);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string latestVersion = request.downloadHandler.text.Trim();
            UnityEngine.Debug.Log($"Current Version: {currentVersion}");
            UnityEngine.Debug.Log($"Latest Version: {latestVersion}");

            if (currentVersion != latestVersion)
            {
                isUpdateAvailable = true;
                UpdateMessage.SetActive(true);
                UnityEngine.Debug.Log("Update is available!");
            }
            else
            {
                isUpdateAvailable = false;
                UnityEngine.Debug.Log("You are using the latest version.");
            }
        }
        else
        {
            UnityEngine.Debug.LogError($"Error fetching version info: {request.error}");
            isUpdateAvailable = false;
        }
    }
}
