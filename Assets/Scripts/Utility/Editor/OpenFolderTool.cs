using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;


namespace Tactics.Utility
{
    
    //alt+Double click folders to reveal them in explorer.
    //I changed shift to alt because I kept group-selecting and launching multiple windows.
    
    //This tool from Warped Imagination: https://www.youtube.com/watch?v=d7vsQ8AkpMY
    public class OpenFolderTool
    {
        [OnOpenAsset]
        public static bool OnOpenAsset(int instanceId)
        {
            Event e = Event.current;
            if (e == null || !e.alt)
            {
                return false;
            }
            
            Object obj = EditorUtility.InstanceIDToObject(instanceId);
            string path = AssetDatabase.GetAssetPath(obj);
            if (AssetDatabase.IsValidFolder(path))
            {
                EditorUtility.RevealInFinder(path);
            }
            else
            {
                return false;
            }

            return true;
        }
    }
}
