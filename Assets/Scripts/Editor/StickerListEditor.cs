using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(StickerList))]
public class StickerListEditor : Editor {

    static GameObject parentObject = null;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        StickerList stickerList = (StickerList)target;

        parentObject = (GameObject)EditorGUILayout.ObjectField("Parent Object:", parentObject, typeof(GameObject), true);
        if(GUILayout.Button("Create list from Parent"))
        {
            GenerateList(stickerList);
        }
    }

    void GenerateList(StickerList stickerList)
    {
        if(parentObject != null)
        {
            List<GameObject> gList = new List<GameObject>();
            foreach(Transform t in parentObject.transform)
            {
                gList.Add(t.gameObject);
            }
            stickerList.stickers = gList;
        }
    }
}
