using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StickerCombo", menuName = "DecalRoyale/StickerCombo", order = 3)]
public class StickerCombo : ScriptableObject {
    public StickerTag first;
    public StickerTag second;
    public string bonusText = "$a and $b";
    public float multiplier = 2;
    public Color color = Color.white;

    public bool CheckCombo(StickerTag[] top, StickerTag[] bottom)
    {
        foreach(StickerTag t in top)
        {
            foreach(StickerTag b in bottom)
            {
                if(t.name == first.name && b.name == second.name)
                {
                    return true;
                }
            }
        }
        return false;
    }

    public string GetComboString(StickerData top, StickerData bottom)
    {
        string text = "" + bonusText;
        text = text.Replace("$a", top.GetPrefix());
        text = text.Replace("$b", bottom.GetSuffix());
        text = text.ToUpper();
        text = multiplier + "x " + text;
        return text;
    }
}
