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

 
    //サーバの接続が切れたときのメッセージを、ChatTextに表示する
    public void RecvClose()
    {

    }


    string json;


    void Start()
    {
        common = GetComponent<ServerClientCommon>();
        


        //ws = new WebSocket("ws://192.168.1.13:12345/"); //多分実家のwifi
        ws = new WebSocket("ws://localhost:12345/");  //ローカル ( 自PC内 ) のサーバーとやり取りする時
        ws.Connect();

        //サーバからメッセージを受信したときに実行する処理「RecvText」を登録する
        ws.OnMessage += (sender, e) => json = e.Data;
        ws.OnMessage += (sender, e) => messageReceived = true;

        OnMessage += () =>
        {
            Debug.Log("OnMessage");
            common.LoadAvatar(json);
        };

        //サーバとの接続が切れたときに実行する処理「RecvClose」を登録する
        ws.OnClose += (sender, e) => RecvClose();
    }


    //WebSocket の OnMessage だと何故か Resources からのロードが出来ないので仕方ないからフラグで別の Action 作って間接的にやる
    bool messageReceived = false;
    public event System.Action OnMessage;
    private void Update()
    {
        if (messageReceived == true)
        {
            messageReceived = false;
            OnMessage?.Invoke();
        }
    }
}

