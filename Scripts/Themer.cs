using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using System.IO;
using System;
using TMPro;
using System.Runtime.InteropServices.ComTypes;
using System.Linq;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

[Serializable]
public class ThemeColor
{
    public string ColorName;
    public Vector3 ColorRGB255;
    public Vector3 ColorRGB1;
    public List<Image> ImageToColor;
    public List<TextMeshProUGUI> TextToColor;
}

public class Themer : MonoBehaviour
{
    public DynamicResolution DynamicResolution;
    public GameObject ExportWait;
    public Camera ExportCamera;

    public List<ThemeColor> ThemeColors;

    public int ChosenColor;
    public TextMeshProUGUI ChosenColorText;
    public TextMeshProUGUI FilePath;
    public List<Slider> ColorSliders;
    public List<TMP_InputField> ColorInputs;
    public Image PreviewColor;
    public bool Wait;

    public bool Copied;
    public Vector3 CopiedColor;

    public string readData;

    public string SaveLoadPath;
    public TMP_InputField PathInputField;

    public List<GameObject> SquareScreenAssets;
    public List<GameObject> RoundScreenAssets;

    void Start()
    {
        if(PlayerPrefs.GetInt("ScreenIndex") == 0)
        {
            foreach(var asset in RoundScreenAssets)
            {
                asset.SetActive(false);
            }
            foreach(var asset in SquareScreenAssets)
            {
                asset.SetActive(true);
            }
        }
        if (PlayerPrefs.GetInt("ScreenIndex") == 1)
        {
            foreach (var asset in RoundScreenAssets)
            {
                asset.SetActive(true);
            }
            foreach (var asset in SquareScreenAssets)
            {
                asset.SetActive(false);
            }
        }

        if (PlayerPrefs.GetString("Path") == "")
        {
            SaveLoadPath = Application.dataPath.Replace("YoRadio! Theme Editor_Data", "");
            PlayerPrefs.SetString("Path", SaveLoadPath);
        }
        SaveLoadPath = PlayerPrefs.GetString("Path");
        PathInputField.text = SaveLoadPath;

        CheckForFile();

        for (int i = 0; i < ThemeColors.Count; i++)
        {
            float R255 = ThemeColors[i].ColorRGB255.x;
            float G255 = ThemeColors[i].ColorRGB255.y;
            float B255 = ThemeColors[i].ColorRGB255.z;

            ThemeColors[i].ColorRGB1 = ConvertRGB(R255, G255, B255);
            
            float R = ThemeColors[i].ColorRGB1.x;
            float G = ThemeColors[i].ColorRGB1.y;
            float B = ThemeColors[i].ColorRGB1.z;

            Color currentColor = new Color(R, G, B);

            if (ThemeColors[i].ImageToColor.Count != 0)
            {
                for (int img = 0; img < ThemeColors[i].ImageToColor.Count; img++)
                {
                    Image image = ThemeColors[i].ImageToColor[img];
                    if(image != null)
                    {
                        ThemeColors[i].ImageToColor[img].color = currentColor;
                    }
                }
            }
            
            if (ThemeColors[i].TextToColor.Count != 0)
            {
                for (int txt = 0; txt < ThemeColors[i].TextToColor.Count; txt++)
                {
                    TextMeshProUGUI text = ThemeColors[i].TextToColor[txt];
                    if(text != null)
                    {
                        ThemeColors[i].TextToColor[txt].color = currentColor;
                    }
                }
            }
        }

        ChangeColor("COLOR_BACKGROUND");
    }

    public void UpdatePath()
    {
        PlayerPrefs.SetString("Path", PathInputField.text);
        SceneManager.LoadScene(0);
    }

    public void ChangeScreenType(int screenIndex)
    {
        PlayerPrefs.SetInt("ScreenIndex", screenIndex);
    }

    public void CheckForFile()
    {
        string filePath = Path.Combine(SaveLoadPath, "mytheme.h");
        if (File.Exists(filePath))
        {
            GetFileData(filePath);
        }
    }

    public string RemoveLines(string text, int numLines)
    {
        string[] lines = text.Split('\n');

        if (numLines >= lines.Length)
            return "";

        return string.Join("\n", lines[numLines..]);
    }

    string RemoveLinesAfter(string text, int lineNumber)
    {
        string[] lines = text.Split('\n');

        if (lineNumber >= lines.Length)
            return text;

        return string.Join("\n", lines[0..(lineNumber + 1)]);
    }

    string RemoveDefines(string text)
    {
        string[] lines = text.Split('\n');

        for (int i = 0; i < lines.Length; i++)
        {
            lines[i] = lines[i].Replace("#define ", "");
        }
        return string.Join("\n", lines);
    }

    string ClearText(string fileContent)
    {
        fileContent = RemoveLines(fileContent, 10);
        fileContent = RemoveLinesAfter(fileContent, 32);
        fileContent = RemoveDefines(fileContent);
        return fileContent;
    }

    void GetFileData(string filePath)
    {
        string fileContent = File.ReadAllText(filePath);

        fileContent = ClearText(fileContent);

        string[] lines = fileContent.Split("\n");

        Vector3 newVector = new Vector3(0,0,0);

        for (int i = 0; i < lines.Length; i++) 
        {
            if (lines[i] == "")
            {
                break;
            }

            string ColorName = "";
            foreach (char c in lines[i])
            {
                if (c == ' ')
                {
                    break;
                }
                ColorName += c;
            }

            int ColorIndex = GetIndexByLabel(ColorName);

            string TextToRemove = "";
            string Color = "";

            string TextWithoutName = lines[i].Replace(ThemeColors[ColorIndex].ColorName, "");

            foreach (char c in TextWithoutName)
            {
                if (char.IsDigit(c))
                {
                    Color = TextWithoutName.Replace(TextToRemove, "");
                    break;
                }

                TextToRemove += c;
            }

            string[] Parts = Color.Split(", ");
            
            newVector = new Vector3(float.Parse(Parts[0]), float.Parse(Parts[1]), float.Parse(Parts[2]));

            ThemeColors[ColorIndex].ColorRGB255 = newVector;
            ThemeColors[ColorIndex].ColorRGB1 = ConvertRGB(newVector.x, newVector.y, newVector.z);

            Vector3 ColorRGB1 = ThemeColors[ColorIndex].ColorRGB1;

            Color loadedColor = new Color(ColorRGB1.x, ColorRGB1.y, ColorRGB1.z);

            if(ColorName == "COLOR_BITRATE")
            {
                Debug.Log(ColorIndex);
                Debug.Log(ThemeColors[ColorIndex].ColorName);
                Debug.Log(ThemeColors[ColorIndex].ColorRGB1);
                Debug.Log(ThemeColors[ColorIndex].ColorRGB255);
                Debug.Log(loadedColor);
                Debug.Log(ThemeColors[ColorIndex].ImageToColor.Count);
                for(int z = 0; z < ThemeColors[ColorIndex].ImageToColor.Count; z++)
                {
                    Debug.Log(ThemeColors[ColorIndex].ImageToColor[z].name);
                }
            }

            if (ThemeColors[ChosenColor].ImageToColor.Count != 0)
            {
                for (int img = 0; img < ThemeColors[ChosenColor].ImageToColor.Count; img++)
                {
                    Image image = ThemeColors[ChosenColor].ImageToColor[img];
                    if (image != null)
                    {
                        ThemeColors[ChosenColor].ImageToColor[img].color = loadedColor;
                    }
                }
            }

            if (ThemeColors[ChosenColor].TextToColor.Count != 0)
            {
                for (int txt = 0; txt < ThemeColors[ChosenColor].TextToColor.Count; txt++)
                {
                    TextMeshProUGUI text = ThemeColors[ChosenColor].TextToColor[txt];
                    if (text != null)
                    {
                        ThemeColors[ChosenColor].TextToColor[txt].color = loadedColor;
                    }
                }
            }
        }
        FilePath.text = "Succesfully loaded the last theme.";
    }

    void Update()
    {
        ChosenColorText.text = ThemeColors[ChosenColor].ColorName;
        if (Input.GetKey(KeyCode.LeftControl))
        {
            if (Input.GetKeyDown(KeyCode.C))
            {
                CopyColor();
            }
            if (Input.GetKeyDown(KeyCode.V))
            {
                PasteColor();
            }
            if (Input.GetKeyDown(KeyCode.S))
            {
                SaveToFile();
            }
        }
    }

    public Vector3 ConvertRGB(float r, float g, float b)
    {
        r = Mathf.Clamp(r, 0f, 255f);
        g = Mathf.Clamp(g, 0f, 255f);
        b = Mathf.Clamp(b, 0f, 255f);

        float unityR = r / 255f;
        float unityG = g / 255f;
        float unityB = b / 255f;

        return new Vector3(unityR, unityG, unityB);
    }

    public void ChangeColor(string ColorName)
    {
        Wait = true;

        ChosenColor = GetIndexByLabel(ColorName);

        ColorSliders[0].value = ThemeColors[ChosenColor].ColorRGB255.x;
        ColorSliders[1].value = ThemeColors[ChosenColor].ColorRGB255.y;
        ColorSliders[2].value = ThemeColors[ChosenColor].ColorRGB255.z;

        ColorInputs[0].text = ThemeColors[ChosenColor].ColorRGB255.x.ToString();
        ColorInputs[1].text = ThemeColors[ChosenColor].ColorRGB255.y.ToString();
        ColorInputs[2].text = ThemeColors[ChosenColor].ColorRGB255.z.ToString();

        float R = ThemeColors[ChosenColor].ColorRGB1.x;
        float G = ThemeColors[ChosenColor].ColorRGB1.y;
        float B = ThemeColors[ChosenColor].ColorRGB1.z;

        Color preview = new Color(R, G, B);

        PreviewColor.color = preview;

        StartCoroutine(WaitABit());
    }

    IEnumerator WaitABit()
    {
        yield return new WaitForSeconds(0.1f);
        Wait = false;
    }

    public int GetIndexByLabel(string label)
    {
        int index = ThemeColors.FindIndex(color => color.ColorName.ToLower() == label.ToLower());
        return index;
    }

    Vector3 newColor255;
    Vector3 newColor1;

    public void OnSliderChange()
    {
        if (Wait == true) return;

        float r = ColorSliders[0].value;
        float g = ColorSliders[1].value;
        float b = ColorSliders[2].value;

        ColorInputs[0].text = ColorSliders[0].value.ToString();
        ColorInputs[1].text = ColorSliders[1].value.ToString();
        ColorInputs[2].text = ColorSliders[2].value.ToString();

        newColor255 = new Vector3(r, g, b);

        newColor1 = ConvertRGB(r, g, b);

        ThemeColors[ChosenColor].ColorRGB255 = newColor255;
        ThemeColors[ChosenColor].ColorRGB1 = newColor1;

        Color currentColor = new Color(newColor1.x, newColor1.y, newColor1.z);

        if (ThemeColors[ChosenColor].ImageToColor.Count != 0)
        {
            for (int img = 0; img < ThemeColors[ChosenColor].ImageToColor.Count; img++)
            {
                Image image = ThemeColors[ChosenColor].ImageToColor[img];
                if (image != null)
                {
                    ThemeColors[ChosenColor].ImageToColor[img].color = currentColor;
                }
            }
        }

        if (ThemeColors[ChosenColor].TextToColor.Count != 0)
        {
            for (int txt = 0; txt < ThemeColors[ChosenColor].TextToColor.Count; txt++)
            {
                TextMeshProUGUI text = ThemeColors[ChosenColor].TextToColor[txt];
                if (text != null)
                {
                    ThemeColors[ChosenColor].TextToColor[txt].color = currentColor;
                }
            }
        }

        PreviewColor.color = currentColor;
    }

    public void OnInputChange()
    {
        if (Wait == true) return;

        float r = int.Parse(ColorInputs[0].text);
        float g = int.Parse(ColorInputs[1].text);
        float b = int.Parse(ColorInputs[2].text);

        for (int i = 0; i < ColorInputs.Count; i++)
        {
            if (int.Parse(ColorInputs[i].text) < 0)
            {
                ColorInputs[i].text = "0";
            }
            if (int.Parse(ColorInputs[i].text) > 255)
            {
                ColorInputs[i].text = "255";
            }
        }

        ColorSliders[0].value = int.Parse(ColorInputs[0].text);
        ColorSliders[1].value = int.Parse(ColorInputs[1].text);
        ColorSliders[2].value = int.Parse(ColorInputs[2].text);

        newColor255 = new Vector3(r, g, b);

        newColor1 = ConvertRGB(r, g, b);

        ThemeColors[ChosenColor].ColorRGB255 = newColor255;
        ThemeColors[ChosenColor].ColorRGB1 = newColor1;

        Color currentColor = new Color(newColor1.x, newColor1.y, newColor1.z);

        if (ThemeColors[ChosenColor].ImageToColor.Count != 0)
        {
            for (int img = 0; img < ThemeColors[ChosenColor].ImageToColor.Count; img++)
            {
                Image image = ThemeColors[ChosenColor].ImageToColor[img];
                if (image != null)
                {
                    ThemeColors[ChosenColor].ImageToColor[img].color = currentColor;
                }
            }
        }

        if (ThemeColors[ChosenColor].TextToColor.Count != 0)
        {
            for (int txt = 0; txt < ThemeColors[ChosenColor].TextToColor.Count; txt++)
            {
                TextMeshProUGUI text = ThemeColors[ChosenColor].TextToColor[txt];
                if (text != null)
                {
                    ThemeColors[ChosenColor].TextToColor[txt].color = currentColor;
                }
            }
        }

        PreviewColor.color = currentColor;
    }

    public void DarkenColor()
    {
        newColor255 = ThemeColors[ChosenColor].ColorRGB255;

        float r = Mathf.Clamp(newColor255.x - 10, 0, 255);
        float g = Mathf.Clamp(newColor255.y - 10, 0, 255);
        float b = Mathf.Clamp(newColor255.z - 10, 0, 255);

        newColor1 = ConvertRGB(r, g, b);

        ThemeColors[ChosenColor].ColorRGB255 = new Vector3(r, g, b);
        ThemeColors[ChosenColor].ColorRGB1 = newColor1;
        Color currentColor = new Color(newColor1.x, newColor1.y, newColor1.z);

        if (ThemeColors[ChosenColor].ImageToColor.Count != 0)
        {
            for (int img = 0; img < ThemeColors[ChosenColor].ImageToColor.Count; img++)
            {
                Image image = ThemeColors[ChosenColor].ImageToColor[img];
                if (image != null)
                {
                    ThemeColors[ChosenColor].ImageToColor[img].color = currentColor;
                }
            }
        }

        if (ThemeColors[ChosenColor].TextToColor.Count != 0)
        {
            for (int txt = 0; txt < ThemeColors[ChosenColor].TextToColor.Count; txt++)
            {
                TextMeshProUGUI text = ThemeColors[ChosenColor].TextToColor[txt];
                if (text != null)
                {
                    ThemeColors[ChosenColor].TextToColor[txt].color = currentColor;
                }
            }
        }

        ColorSliders[0].value = r;
        ColorSliders[1].value = g;
        ColorSliders[2].value = b;

        ColorInputs[0].text = r.ToString();
        ColorInputs[1].text = g.ToString();
        ColorInputs[2].text = b.ToString();

        PreviewColor.color = currentColor;
    }

    public void LightenColor()
    {
        newColor255 = ThemeColors[ChosenColor].ColorRGB255;

        float r = Mathf.Clamp(newColor255.x + 10, 0, 255);
        float g = Mathf.Clamp(newColor255.y + 10, 0, 255);
        float b = Mathf.Clamp(newColor255.z + 10, 0, 255);

        newColor1 = ConvertRGB(r, g, b);

        ThemeColors[ChosenColor].ColorRGB255 = new Vector3(r, g, b);
        ThemeColors[ChosenColor].ColorRGB1 = newColor1;
        Color currentColor = new Color(newColor1.x, newColor1.y, newColor1.z);

        if (ThemeColors[ChosenColor].ImageToColor.Count != 0)
        {
            for (int img = 0; img < ThemeColors[ChosenColor].ImageToColor.Count; img++)
            {
                Image image = ThemeColors[ChosenColor].ImageToColor[img];
                if (image != null)
                {
                    ThemeColors[ChosenColor].ImageToColor[img].color = currentColor;
                }
            }
        }

        if (ThemeColors[ChosenColor].TextToColor.Count != 0)
        {
            for (int txt = 0; txt < ThemeColors[ChosenColor].TextToColor.Count; txt++)
            {
                TextMeshProUGUI text = ThemeColors[ChosenColor].TextToColor[txt];
                if (text != null)
                {
                    ThemeColors[ChosenColor].TextToColor[txt].color = currentColor;
                }
            }
        }

        ColorSliders[0].value = r;
        ColorSliders[1].value = g;
        ColorSliders[2].value = b;

        ColorInputs[0].text = r.ToString();
        ColorInputs[1].text = g.ToString();
        ColorInputs[2].text = b.ToString();

        PreviewColor.color = currentColor;
    }

    public void CopyColor()
    {
        if (!Copied) Copied = true;
        CopiedColor = ThemeColors[ChosenColor].ColorRGB255;
    }

    public void PasteColor()
    {
        if (Copied)
        {
            ThemeColors[ChosenColor].ColorRGB255 = CopiedColor;
            ChangeColor(ThemeColors[ChosenColor].ColorName);
            StartCoroutine(WaitNow());
        }
    }

    IEnumerator WaitNow()
    {
        yield return new WaitForSeconds(0.2f);
        OnSliderChange();
    }

    public string GetRGB255(string themecolor)
    {
        try
        {
            int index = GetIndexByLabel(themecolor);

            Vector3 ColorRGB255 = ThemeColors[index].ColorRGB255;

            return ColorRGB255.x.ToString() + ",  " + ColorRGB255.y.ToString() + ",  " + ColorRGB255.z.ToString();
        }
        catch
        {
            int index = GetIndexByLabel(themecolor);
            Debug.Log(themecolor);
            return "0";
        }
    }

    public void CopyTextToClipboard()
    {
        string text =
            "// File created with YoRadio Theme Editor created by András Daradics\n" +
            "// File last modified: " + System.DateTime.Now.ToString() + "\n" +
            "// GitHub: https://github.com/andrasdaradici/YoRadio-Theme-Editor\n" +
            "// Itch.io: https://andrasdaradici.itch.io/yoradio-theme-editor\n" +
            "#ifndef _my_theme_h\n" +
            "#define _my_theme_h\n\n" +
            "#define ENABLE_THEME\n" +
            "#ifdef  ENABLE_THEME\n\n" +
            "#define COLOR_BACKGROUND     " + GetRGB255("COLOR_BACKGROUND") + "\n" +

            "#define COLOR_STATION_NAME     " + GetRGB255("COLOR_STATION_NAME") + "\n" +
            "#define COLOR_STATION_BG    " + GetRGB255("COLOR_STATION_BG") + "\n" +
            "#define COLOR_STATION_FILL    " + GetRGB255("COLOR_STATION_FILL") + "\n" +
           
            "#define COLOR_SNG_TITLE_1    " + GetRGB255("COLOR_SNG_TITLE_1") + "\n" +
            "#define COLOR_SNG_TITLE_2    " + GetRGB255("COLOR_SNG_TITLE_2") + "\n" +

            "#define COLOR_WEATHER    " + GetRGB255("COLOR_WEATHER") + "\n" +

            "#define COLOR_VU_MAX    " + GetRGB255("COLOR_VU_MAX") + "\n" +
            "#define COLOR_VU_MIN    " + GetRGB255("COLOR_VU_MIN") + "\n" +

            "#define COLOR_CLOCK    " + GetRGB255("COLOR_CLOCK") + "\n" +
            "#define COLOR_CLOCK_BG    " + GetRGB255("COLOR_CLOCK_BG") + "\n" +

            "#define COLOR_SECONDS    " + GetRGB255("COLOR_SECONDS") + "\n" +
            "#define COLOR_DAY_OF_W    " + GetRGB255("COLOR_DAY_OF_W") + "\n" +
            "#define COLOR_DATE    " + GetRGB255("COLOR_DATE") + "\n" +


            "#define COLOR_BUFFER    " + GetRGB255("COLOR_BUFFER") + "\n" +
            "#define COLOR_IP    " + GetRGB255("COLOR_IP") + "\n" +

            "#define COLOR_VOLUME_VALUE    " + GetRGB255("COLOR_VOLUME_VALUE") + "\n" +

            "#define COLOR_RSSI    " + GetRGB255("COLOR_RSSI") + "\n" +

            "#define COLOR_VOLBAR_OUT    " + GetRGB255("COLOR_VOLBAR_OUT") + "\n" +
            "#define COLOR_VOLBAR_IN    " + GetRGB255("COLOR_VOLBAR_IN") + "\n" +

            "#define COLOR_DIGITS    " + "100, 100, 255" + "\n" +

            "#define COLOR_DIVIDER    " + GetRGB255("COLOR_DIVIDER") + "\n" +
            "#define COLOR_BITRATE    " + GetRGB255("COLOR_BITRATE") + "\n" +

            "#define COLOR_HEAP    " + "255, 168, 162" + "\n" +
            "#define COLOR_PL_CURRENT    " + "0, 0, 0" + "\n" +
            "#define COLOR_PL_CURRENT_BG    " + "91, 118, 255" + "\n" +
            "#define COLOR_PL_CURRENT_FILL    " + "91, 118, 255" + "\n" +

            "#define COLOR_PLAYLIST_0    " + "255, 255, 255" + "\n" +
            "#define COLOR_PLAYLIST_1    " + "255, 255, 255" + "\n" +
            "#define COLOR_PLAYLIST_2    " + "255, 255, 255" + "\n" +
            "#define COLOR_PLAYLIST_3    " + "255, 255, 255" + "\n" +
            "#define COLOR_PLAYLIST_4    " + "255, 255, 255" + "\n\n\n" +
            "#endif\n" +
            "#endif\n"
            ;


        GUIUtility.systemCopyBuffer = text;
    }

    public void SaveToFile()
    {
        string text =
            "// File created with YoRadio Theme Editor created by András Daradics\n" +
            "// File last modified: " + System.DateTime.Now.ToString() + "\n" +
            "// GitHub: https://github.com/andrasdaradici/YoRadio-Theme-Editor\n" +
            "// Itch.io: https://andrasdaradici.itch.io/yoradio-theme-editor\n" +
            "#ifndef _my_theme_h\n" +
            "#define _my_theme_h\n\n" +
            "#define ENABLE_THEME\n" +
            "#ifdef  ENABLE_THEME\n\n" +
            "#define COLOR_BACKGROUND     " + GetRGB255("COLOR_BACKGROUND") + "\n" +

            "#define COLOR_STATION_NAME     " + GetRGB255("COLOR_STATION_NAME") + "\n" +
            "#define COLOR_STATION_BG    " + GetRGB255("COLOR_STATION_BG") + "\n" +
            "#define COLOR_STATION_FILL    " + GetRGB255("COLOR_STATION_FILL") + "\n" +

            "#define COLOR_SNG_TITLE_1    " + GetRGB255("COLOR_SNG_TITLE_1") + "\n" +
            "#define COLOR_SNG_TITLE_2    " + GetRGB255("COLOR_SNG_TITLE_2") + "\n" +

            "#define COLOR_WEATHER    " + GetRGB255("COLOR_WEATHER") + "\n" +

            "#define COLOR_VU_MAX    " + GetRGB255("COLOR_VU_MAX") + "\n" +
            "#define COLOR_VU_MIN    " + GetRGB255("COLOR_VU_MIN") + "\n" +

            "#define COLOR_CLOCK    " + GetRGB255("COLOR_CLOCK") + "\n" +
            "#define COLOR_CLOCK_BG    " + GetRGB255("COLOR_CLOCK_BG") + "\n" +

            "#define COLOR_SECONDS    " + GetRGB255("COLOR_SECONDS") + "\n" +
            "#define COLOR_DAY_OF_W    " + GetRGB255("COLOR_DAY_OF_W") + "\n" +
            "#define COLOR_DATE    " + GetRGB255("COLOR_DATE") + "\n" +


            "#define COLOR_BUFFER    " + GetRGB255("COLOR_BUFFER") + "\n" +
            "#define COLOR_IP    " + GetRGB255("COLOR_IP") + "\n" +

            "#define COLOR_VOLUME_VALUE    " + GetRGB255("COLOR_VOLUME_VALUE") + "\n" +

            "#define COLOR_RSSI    " + GetRGB255("COLOR_RSSI") + "\n" +

            "#define COLOR_VOLBAR_OUT    " + GetRGB255("COLOR_VOLBAR_OUT") + "\n" +
            "#define COLOR_VOLBAR_IN    " + GetRGB255("COLOR_VOLBAR_IN") + "\n" +

            "#define COLOR_DIGITS    " + "100, 100, 255" + "\n" +

            "#define COLOR_DIVIDER    " + GetRGB255("COLOR_DIVIDER") + "\n" +
            "#define COLOR_BITRATE    " + GetRGB255("COLOR_BITRATE") + "\n" +

            "#define COLOR_HEAP    " + "255, 168, 162" + "\n" +
            "#define COLOR_PL_CURRENT    " + GetRGB255("COLOR_PL_CURRENT") + "\n" +
            "#define COLOR_PL_CURRENT_BG    " + GetRGB255("COLOR_PL_CURRENT_BG") + "\n" +
            "#define COLOR_PL_CURRENT_FILL    " + GetRGB255("COLOR_PL_CURRENT_FILL") + "\n" +

            "#define COLOR_PLAYLIST_0    " + GetRGB255("COLOR_PLAYLIST_0") + "\n" +
            "#define COLOR_PLAYLIST_1    " + GetRGB255("COLOR_PLAYLIST_1") + "\n" +
            "#define COLOR_PLAYLIST_2    " + GetRGB255("COLOR_PLAYLIST_2") + "\n" +
            "#define COLOR_PLAYLIST_3    " + GetRGB255("COLOR_PLAYLIST_3") + "\n" +
            "#define COLOR_PLAYLIST_4    " + GetRGB255("COLOR_PLAYLIST_4") + "\n\n\n" +
            "#endif\n" +
            "#endif\n"
            ;

        string filePath = Path.Combine(SaveLoadPath, "mytheme.h");

        if (!Directory.Exists(SaveLoadPath))
        {
            FilePath.text = "This should not appear. Contact the developer.";
        }

        File.WriteAllText(filePath, text);
        
        FilePath.text = "Saved theme to: " + filePath;
    }

    public void ExportImage()
    {
        StartCoroutine(ExportTheImage());
    }

    IEnumerator ExportTheImage()
    {
        DynamicResolution.ToggleWindowSize();
        yield return new WaitForSeconds(0.55f);

        DateTime now = DateTime.Now;
        string formattedDateTime = string.Format("on {0}-{1}-{2} at {3}-{4}-{5}",
            now.Month, now.Day, now.Year, now.Hour, now.Minute, now.Second);
        Debug.Log("Formatted DateTime: " + formattedDateTime);

        string filename = "YoRadio Theme Editor" + " " + formattedDateTime + ".png";
        string exportFolderPath = Application.dataPath.Replace("YoRadio! Theme Editor_Data", "");
        exportFolderPath = exportFolderPath + "/Images";

        if (!Directory.Exists(exportFolderPath))
        {
            Directory.CreateDirectory(exportFolderPath);
        }

        string exportPath = Path.Combine(exportFolderPath, filename);

        Camera cam = ExportCamera;
        RenderTexture prev = cam.targetTexture;
        RenderTexture renderTexture = new RenderTexture(cam.pixelWidth, cam.pixelHeight, 24);
        cam.targetTexture = renderTexture;
        cam.Render();

        Texture2D image = new Texture2D(cam.pixelWidth, cam.pixelHeight);
        RenderTexture.active = renderTexture;
        image.ReadPixels(new Rect(0, 0, cam.pixelWidth, cam.pixelHeight), 0, 0);
        image.Apply();

        byte[] bytes = image.EncodeToPNG();
        File.WriteAllBytes(exportPath, bytes);

        Destroy(image);

        ExportWait.SetActive(true);
        FilePath.text = "Saved image to: " + exportFolderPath;
        yield return new WaitForSeconds(1f);
        ExportWait.SetActive(false);
        DynamicResolution.ToggleWindowSize();
    }

    public void ApplyTheme(string fileUrl)
    {
        StartCoroutine(GetThemeFromURL(fileUrl));
    }

    IEnumerator GetThemeFromURL(string fileUrl)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(fileUrl))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Failed to download file: " + webRequest.error);
                FilePath.text = "Failed to download file. Please check the URL.";
            }
            else
            {
                string text = webRequest.downloadHandler.text;

                string filePath = Path.Combine(SaveLoadPath, "mytheme.h");

                if (!Directory.Exists(SaveLoadPath))
                {
                    Debug.LogError("Directory does not exist: " + SaveLoadPath);
                    FilePath.text = "Directory does not exist. Contact the developer.";
                }
                else
                {
                    File.WriteAllText(filePath, text);
                    yield return new WaitForSeconds(0.05f);
                    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
                }
            }
        }
    }
}
