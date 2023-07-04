using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using PlasticGui;
using BoidsSimulationOnGPU;

public class ServerClientCommon : MonoBehaviourMyExtention
{
    public string json_Received;
    public GameObject avatar;
    public ClientManager clientManager;
    System.Random rand = new System.Random();

    private void Start()
    {
        clientManager = GetComponent<ClientManager>();
    }


    public void Exequte()
    {
        Debug.Log(json_Received);
        if (json_Received.Contains("LoadAvatar")) LoadAvatar();
        if (json_Received.Contains("ChangeSpeed")) ChangeSpeed();
    }


    public void LoadAvatar(bool isLocalCall = false, string avatarName = "")
    {
        Debug.Log("LoadAvatar");
        avatar = LoadPrefab("GPUBoids");

        if (isLocalCall)
        {
            avatar.transform.position = new Vector3(rand.Next(-4, 4), 0.5f, rand.Next(-4, 4));
            Packet_LoadAvatar packet_LoadAvatar = new Packet_LoadAvatar()
            {
                avatarName = avatarName,
                position = avatar.transform.position,
            };
            string json_Send = JsonConvert.SerializeObject(packet_LoadAvatar);
            clientManager.ws.Send(json_Send);
        }
        else 
        {
            var model = JsonConvert.DeserializeObject<Packet_LoadAvatar>(json_Received);
            avatar.name = model.avatarName;
            avatar.transform.position = model.position;
        }
    }


    public void ChangeSpeed(bool isLocalCall = false, int maxSpeed = 1)
    {
        Debug.Log("ChangeSpeed");
        
        // もし自分の関数呼び出しだったら
        if (isLocalCall)
        {
            // スピード変更
            avatar.GetComponent<GPUBoids>().MaxSpeed = maxSpeed;  
            // サーバーに送信
            Packet_ChangeSeed packet_ChangeSeed = new Packet_ChangeSeed()
            {
                maxSpeed = maxSpeed,
            };
            string json_Send = JsonConvert.SerializeObject(packet_ChangeSeed);
            clientManager.ws.Send(json_Send);
        }
        else
        {
            // サーバーから受信
            var model = JsonConvert.DeserializeObject<Packet_ChangeSeed>(json_Received);
            // スピード変更
            avatar.GetComponent<GPUBoids>().MaxSpeed = model.maxSpeed;
        }
    }


    public void ChangeSize(bool isLocalCall = false, int maxSize = 1)
    {
        Debug.Log("ChangeSize");
    }
}



public abstract class Packet
{
    public string funcName;
}


public class Packet_LoadAvatar : Packet
{
    public Packet_LoadAvatar() 
    {
        funcName = "LoadAvatar";
    }
    public string avatarName;
    public Vector3 position;
}

public class Packet_ChangeSeed : Packet
{
    public Packet_ChangeSeed() 
    {
        funcName = "ChangeSeed";
    }
    public int maxSpeed;
}

public class Packet_ChangeSize : Packet
{
    public Packet_ChangeSize()
    {
        funcName = "ChangeSize";
    }
    public int size;
}
