using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.SceneManagement;
using System.Collections.Generic;
using UnityEditor;

public class SceneOpenerData : ScriptableObject
{
    public List<SceneGroup> sceneGroups = new List<SceneGroup>();
}

[System.Serializable]
public class SceneGroup
{
    public string groupName;
    public List<SceneAsset> scenes;
}
