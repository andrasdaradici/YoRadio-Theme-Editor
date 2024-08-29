using System.Collections;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Updater : MonoBehaviour
{
    [Header("Configuration")]
    public string downloadUrl = "http://andrewstudios.go.ro/YoRadio/Builds/themeeditor.zip";
    public string tempZipFileName = "themeeditor.zip";
    public string tempDir = "TempUpdate"; // Directory used temporarily if needed

    [Header("UI References")]
    public TextMeshProUGUI progressText;
    public Slider progressBar;

    private void Start()
    {
        if (progressText == null)
        {
            UnityEngine.Debug.LogError("Progress TextMeshProUGUI is not assigned.");
            return;
        }

        if (progressBar != null)
        {
            progressBar.value = 0;
            progressBar.maxValue = 100;
        }

        StartCoroutine(UpdateGame());
    }

    private IEnumerator UpdateGame()
    {
        string exeDir = Path.GetDirectoryName(Application.dataPath.Replace("Updater_Data", ""));
        string tempFilePath = Path.Combine(Application.persistentDataPath, tempZipFileName);

        // Download the update
        using (UnityWebRequest webRequest = UnityWebRequest.Get(downloadUrl))
        {
            webRequest.SendWebRequest();

            while (!webRequest.isDone)
            {
                float progress = webRequest.downloadProgress * 100;
                UpdateProgress("Downloading: " + Mathf.RoundToInt(progress) + "%", progress);
                yield return null;
            }

            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                File.WriteAllBytes(tempFilePath, webRequest.downloadHandler.data);
                UpdateProgress("Download complete.", 100);
                UnityEngine.Debug.Log("Download complete.");
            }
            else
            {
                UpdateProgress("Download failed: " + webRequest.error, 0);
                UnityEngine.Debug.LogError("Download failed: " + webRequest.error);
                yield break;
            }
        }

        // Extract the zip file directly to the updater directory
        UpdateProgress("Extracting...", 0);
        yield return StartCoroutine(UnzipFile(tempFilePath, exeDir));
        UpdateProgress("Extraction complete.", 100);
        UnityEngine.Debug.Log("Extraction complete.");

        // Launch the updated application
        string gameExecutablePath = Path.Combine(exeDir, "YoRadio! Theme Editor.exe");

        try
        {
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = gameExecutablePath,
                UseShellExecute = true,
                WorkingDirectory = exeDir
            };

            Process.Start(startInfo);
            UnityEngine.Debug.Log("Game launched successfully.");
        }
        catch (System.Exception ex)
        {
            UnityEngine.Debug.LogError("Failed to launch the game: " + ex.Message);
        }
        finally
        {
            // Quit the updater
            Application.Quit();

            // Clean up
            if (File.Exists(tempFilePath))
            {
                File.Delete(tempFilePath);
            }
        }
    }

    private IEnumerator UnzipFile(string zipPath, string extractPath)
    {
        try
        {
            // Ensure the destination directory exists
            if (!Directory.Exists(extractPath))
            {
                Directory.CreateDirectory(extractPath);
            }

            // Extract files directly to the updater directory
            ZipFile.ExtractToDirectory(zipPath, extractPath, overwriteFiles: true);
        }
        catch (System.Exception ex)
        {
            UnityEngine.Debug.LogError("Unzipping failed: " + ex.Message);
            throw;
        }

        // Simulate extraction progress (optional)
        for (int i = 0; i <= 100; i += 10)
        {
            UpdateProgress("Extracting: " + i + "%", i);
            yield return new WaitForSeconds(0.1f); // Simulate time delay
        }
    }

    private void UpdateProgress(string message, float progress)
    {
        if (progressText != null)
        {
            progressText.text = message;
        }

        if (progressBar != null)
        {
            progressBar.value = progress;
        }
    }
}
