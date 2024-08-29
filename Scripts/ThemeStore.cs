using System.Collections.Generic;
using UnityEngine.Networking;
using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using System.Xml;
using TMPro;

[System.Serializable]
public class Theme
{
    public string Name;
    public string Creator;
    public string Description;
    public string ThumbnailURL;
    public string FileURL;
}

public class ThemeStore : MonoBehaviour
{
    public List<Theme> Themes = new List<Theme>();
    private string xmlUrl = "http://andrewstudios.go.ro/YoRadio/ThemeStore.xml";
    public int selectedIndex = 0;
    private int lastSelectedIndex = -1;

    public Image ThemeThumbnailDisplay;
    public TextMeshProUGUI ThemeNameDisplay;
    public TextMeshProUGUI ThemeCreatorDisplay;
    public TextMeshProUGUI ThemeDescriptionDisplay;

    public Scrollbar ViewportScrollbar;

    public GameObject ThemeTemplate;
    public Transform ThemeParent;

    int curIndex = 0;

    public Themer Themer;
    public GameObject ThemeStoreDisplay;

    void Start()
    {
        ViewportScrollbar.value = 1;
        StartCoroutine(LoadThemesFromWeb());
    }

    void Update()
    {
        if (selectedIndex != lastSelectedIndex)
        {
            lastSelectedIndex = selectedIndex;

            if (selectedIndex >= 0 && selectedIndex < Themes.Count)
            {
                StartCoroutine(LoadThumbnailImage(Themes[selectedIndex].ThumbnailURL));
            }
            else
            {
                Debug.LogWarning("Selected index is out of range.");
            }
        }
    }

    IEnumerator LoadThemesFromWeb()
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(xmlUrl))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Failed to download XML file: " + webRequest.error);
            }
            else
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(webRequest.downloadHandler.text);

                XmlNodeList themeList = xmlDoc.GetElementsByTagName("Theme");

                foreach (XmlNode themeInfo in themeList)
                {
                    Theme theme = new Theme();

                    theme.Name = themeInfo.SelectSingleNode("Name").InnerText;
                    theme.Creator = themeInfo.SelectSingleNode("Creator").InnerText;
                    theme.Description = themeInfo.SelectSingleNode("Description").InnerText;
                    theme.ThumbnailURL = themeInfo.SelectSingleNode("ThumbnailURL").InnerText;
                    theme.FileURL = themeInfo.SelectSingleNode("FileURL").InnerText;
                    Themes.Add(theme);

                    GameObject inst = Instantiate(ThemeTemplate, ThemeParent);
                    inst.SetActive(true);
                    inst.name = theme.Name;
                    inst.GetComponent<ThemeData>().ApplyData(theme.Name, theme.Creator, curIndex);

                    curIndex++;
                }

                if (selectedIndex >= 0 && selectedIndex < Themes.Count)
                {
                    StartCoroutine(LoadThumbnailImage(Themes[selectedIndex].ThumbnailURL));
                }
            }
        }
    }

    IEnumerator LoadThumbnailImage(string url)
    {
        using (UnityWebRequest webRequest = UnityWebRequestTexture.GetTexture(url))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Failed to download image: " + webRequest.error);
            }
            else
            {
                Texture2D texture = ((DownloadHandlerTexture)webRequest.downloadHandler).texture;
                Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                ThemeThumbnailDisplay.sprite = sprite;
                ThemeNameDisplay.text = Themes[selectedIndex].Name;
                ThemeCreatorDisplay.text = "Created by: " + Themes[selectedIndex].Creator;
                ThemeDescriptionDisplay.text = Themes[selectedIndex].Description;
            }
        }
    }

    public void ApplyTheme()
    {
        ThemeStoreDisplay.SetActive(false);
        Themer.ApplyTheme(Themes[selectedIndex].FileURL);
    }

    public void OpenBrowser()
    {
        Application.OpenURL("http://andrewstudios.go.ro/YoRadio/");
    }
}
