using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class ServerClientCommon : MonoBehaviourMyExtention
{
    public void LoadAvatar(string json)
    {
        Debug.Log("LoadAvatar1");
        var model = JsonConvert.DeserializeObject<LoadAvatarJModel>(json);
        
        GameObject avatar = LoadPrefab("GPUBoids");
        avatar.name = model.avatarName;
    }
}




public class LoadAvatarJModel
{
    public string funcName;
    public string avatarName;
}