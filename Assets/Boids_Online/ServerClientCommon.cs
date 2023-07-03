using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerClientCommon : MonoBehaviourMyExtention
{
    public void  LoadAvatar(string avatarName)
    {
        GameObject avatar = LoadPrefab("GPUBoids");
        avatar.name = avatarName;
    }
}





public class LoadAvatarJModel
{
    public string funcName;
    public string avatarName;
}