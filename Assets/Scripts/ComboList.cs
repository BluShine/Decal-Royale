using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ComboList", menuName = "DecalRoyale/ComboList", order = 4)]
public class ComboList : ScriptableObject
{
    public List<StickerCombo> combos;
}
