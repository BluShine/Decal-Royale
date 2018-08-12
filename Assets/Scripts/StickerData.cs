using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StickerData : MonoBehaviour {

    public StickerTag[] tags;
    public string prefixName;
    public string suffixName;

    public string GetPrefix()
    {
        if (prefixName == "")
        {
            return name;
        }
        return prefixName;
    }

    public string GetSuffix()
    {
        if(suffixName == "")
        {
            return name;
        }
        return suffixName;
    }
}
