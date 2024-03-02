using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using System.IO;
using System;
using TMPro;

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
    public List<ThemeColor> ThemeColors;

    public int ChosenColor;
    public TextMeshProUGUI ChosenColorText;
    public TextMeshProUGUI FilePath;
    public List<Slider> ColorSliders;
    public Image PreviewColor;
    public bool Wait;
    void Start()
    {
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

    void Update()
    {
        ChosenColorText.text = ThemeColors[ChosenColor].ColorName;
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

        float R = ThemeColors[ChosenColor].ColorRGB1.x;
        float G = ThemeColors[ChosenColor].ColorRGB1.y;
        float B = ThemeColors[ChosenColor].ColorRGB1.z;

        Color preview = new Color(R, G, B);

        PreviewColor.color = preview;

        StartCoroutine(WaitABit());
    }

    IEnumerator WaitABit()
    {
        yield return new WaitForSeconds(0.5f);
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

            "#define COLOR_HEAP    " + "255, 168, 162" + "\n" +

            "#define COLOR_BUFFER    " + GetRGB255("COLOR_BUFFER") + "\n" +
            "#define COLOR_IP    " + GetRGB255("COLOR_IP") + "\n" +

            "#define COLOR_VOLUME_VALUE    " + GetRGB255("COLOR_VOLUME_VALUE") + "\n" +

            "#define COLOR_RSSI    " + GetRGB255("COLOR_RSSI") + "\n" +

            "#define COLOR_VOLBAR_OUT    " + GetRGB255("COLOR_VOLBAR_OUT") + "\n" +
            "#define COLOR_VOLBAR_IN    " + GetRGB255("COLOR_VOLBAR_IN") + "\n" +

            "#define COLOR_DIGITS    " + "100, 100, 255" + "\n" +

            "#define COLOR_DIVIDER    " + GetRGB255("COLOR_DIVIDER") + "\n" +
            "#define COLOR_BITRATE    " + GetRGB255("COLOR_BITRATE") + "\n" +

            "#define COLOR_PL_CURRENT    " + "0, 0, 0" + "\n" +
            "#define COLOR_PL_CURRENT_BG    " + "91, 118, 255" + "\n" +
            "#define COLOR_PL_CURRENT_FILL    " + "91, 118, 255" + "\n" +

            "#define COLOR_PLAYLIST_0    " + "255, 0, 0" + "\n" +
            "#define COLOR_PLAYLIST_1    " + "0, 255, 0" + "\n" +
            "#define COLOR_PLAYLIST_2    " + "255, 0, 255" + "\n" +
            "#define COLOR_PLAYLIST_3    " + "0, 0, 255" + "\n" +
            "#define COLOR_PLAYLIST_4    " + "0, 255, 255" + "\n\n\n" +
            "#endif\n" +
            "#endif\n"
            ;


        GUIUtility.systemCopyBuffer = text;
    }

    public void SaveToFile()
    {
        string text =
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

            "#define COLOR_HEAP    " + "255, 168, 162" + "\n" +

            "#define COLOR_BUFFER    " + GetRGB255("COLOR_BUFFER") + "\n" +
            "#define COLOR_IP    " + GetRGB255("COLOR_IP") + "\n" +

            "#define COLOR_VOLUME_VALUE    " + GetRGB255("COLOR_VOLUME_VALUE") + "\n" +

            "#define COLOR_RSSI    " + GetRGB255("COLOR_RSSI") + "\n" +

            "#define COLOR_VOLBAR_OUT    " + GetRGB255("COLOR_VOLBAR_OUT") + "\n" +
            "#define COLOR_VOLBAR_IN    " + GetRGB255("COLOR_VOLBAR_IN") + "\n" +

            "#define COLOR_DIGITS    " + "100, 100, 255" + "\n" +

            "#define COLOR_DIVIDER    " + GetRGB255("COLOR_DIVIDER") + "\n" +
            "#define COLOR_BITRATE    " + GetRGB255("COLOR_BITRATE") + "\n" +

            "#define COLOR_PL_CURRENT    " + "0, 0, 0" + "\n" +
            "#define COLOR_PL_CURRENT_BG    " + "91, 118, 255" + "\n" +
            "#define COLOR_PL_CURRENT_FILL    " + "91, 118, 255" + "\n" +

            "#define COLOR_PLAYLIST_0    " + "255, 0, 0" + "\n" +
            "#define COLOR_PLAYLIST_1    " + "0, 255, 0" + "\n" +
            "#define COLOR_PLAYLIST_2    " + "255, 0, 255" + "\n" +
            "#define COLOR_PLAYLIST_3    " + "0, 0, 255" + "\n" +
            "#define COLOR_PLAYLIST_4    " + "0, 255, 255" + "\n\n\n" +
            "#endif\n" +
            "#endif\n"
            ;

        string directoryPath = "C:/Users/" + System.Environment.UserName + "/Documents/YoRadioThemeEditor/";
        string filePath = Path.Combine(directoryPath, "mytheme.h");

        // Check if the directory exists, if not, create it
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }

        // Write string to file
        File.WriteAllText(filePath, text);

        FilePath.text = "Themed saved to: " + filePath;
    }
}
