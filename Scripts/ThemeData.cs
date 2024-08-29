using TMPro;
using UnityEngine;

public class ThemeData : MonoBehaviour
{
    public string Name;
    public string Creator;
    public int Index;

    public ThemeStore Store;

    public TextMeshProUGUI NameDisplay;
    public TextMeshProUGUI CreatorDisplay;

    public void ApplyData(string name, string creator, int index)
    {
        Name = name;
        Creator = creator;
        Index = index;

        CreatorDisplay.text = creator;
        NameDisplay.text = name;
    }

    public void ChangeThemePreview()
    {
        Store.selectedIndex = Index;
    }
}
