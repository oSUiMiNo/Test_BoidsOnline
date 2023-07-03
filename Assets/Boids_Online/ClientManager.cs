using UnityEngine;
using UnityEngine.UI;
using TMPro;
using WebSocketSharp;
using Newtonsoft.Json;

public class ClientManager : MonoBehaviourMyExtention
{
    WebSocket ws;

    ServerClientCommon common;


    //サーバへ、メッセージを送信する
    public void SendText()
    {
        ws.Send("");
    }

    //サーバから受け取ったメッセージを、ChatTextに表示する
    public void RecvText(string json)
    {
        var loadData = JsonConvert.DeserializeObject<LoadAvatarJModel>(json);
        if(loadData.funcName == "LoadAvatar")
        {
            common.LoadAvatar(loadData.avatarName);
        }
    }

    //サーバの接続が切れたときのメッセージを、ChatTextに表示する
    public void RecvClose()
    {

    }

    void Start()
    {
        common = GetComponent<ServerClientCommon>();

        //ws = new WebSocket("ws://192.168.1.13:12345/"); //多分実家のwifi
        ws = new WebSocket("ws://localhost:12345/");  //ローカル ( 自PC内 ) のサーバーとやり取りする時
        ws.Connect();

        //サーバからメッセージを受信したときに実行する処理「RecvText」を登録する
        ws.OnMessage += (sender, e) => RecvText(e.Data);
        //サーバとの接続が切れたときに実行する処理「RecvClose」を登録する
        ws.OnClose += (sender, e) => RecvClose();
    }
}

