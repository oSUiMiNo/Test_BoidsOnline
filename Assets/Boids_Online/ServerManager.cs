using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp;
using WebSocketSharp.Server;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
//using WebSocketSharp.Net;
public class ServerManager : MonoBehaviourMyExtention
{
    WebSocketServer ws;
    void Start()
    {
        //ポート番号を指定
        ws = new WebSocketServer(12345);
        //クライアントからの通信時の挙動を定義したクラス、「ExWebSocketBehavior」を登録
        ws.AddWebSocketService<ExWebSocketBehavior>("/");
        //サーバ起動
        ws.Start();
        Debug.Log("サーバ起動");
    }
    private void OnApplicationQuit()
    {
        Debug.Log("サーバ停止");
        ws.Stop();
    }




    public class ExWebSocketBehavior : WebSocketBehavior
    {
        public List<Dictionary<string, Packet>> playerInfomations = new List<Dictionary<string, Packet>>();
        Dictionary<string, Packet> packets = new Dictionary<string, Packet>();


        //誰が現在接続しているのか管理するリスト。
        public static List<ExWebSocketBehavior> clientList = new List<ExWebSocketBehavior>();
        //接続者に番号を振るための変数。
        static int globalSeq = 0;
        //自身の番号
        int seq;

        //誰かがログインしてきたときに呼ばれるメソッド
        protected override void OnOpen()
        {
            //ログインしてきた人には、番号をつけて、リストに登録。
            globalSeq++;
            this.seq = globalSeq;
            clientList.Add(this);

            Debug.Log("Seq" + this.seq + " Login. (" + this.ID + ")");

            packets.Add("LoadAvatar", 
                new Packet_LoadAvatar()
                {
                    avatarName = $"avatar{seq}"
                });
            packets.Add("ChangeSeed",
                new Packet_ChangeSeed()
                {
                    maxSpeed = 5
                });
            packets.Add("ChangeSize",
                new Packet_ChangeSize()
                {
                    size = 2
                });

            playerInfomations.Add(packets);

            //string json = JsonConvert.SerializeObject(playerInfomations[seq - 1]["LoadAvatar"]);
            //接続者全員にメッセージを送る
            foreach (var client in clientList)
            {
                client.Send("Seq:" + seq + " Login.");
                foreach (var a in playerInfomations)
                {
                    foreach(var b in a)
                    {
                        string json = JsonConvert.SerializeObject(b);
                        client.Send(json);
                    }
                }
            }
        }
        //誰かがメッセージを送信してきたときに呼ばれるメソッド
        protected override void OnMessage(MessageEventArgs e)
        {
            Debug.Log("Seq:" + seq + "..." + e.Data);

            JObject model = JObject.Parse(e.Data);

            playerInfomations[seq - 1][(string)model["funcName"]] = (Packet)(object)model;
            
            //接続者全員にメッセージを送る
            foreach (var client in clientList)
            {
                if(client != clientList[seq - 1])
                {
                    client.Send(e.Data);
                }
            }
        }

        //誰かがログアウトしてきたときに呼ばれるメソッド
        protected override void OnClose(CloseEventArgs e)
        {
            Debug.Log("Seq" + this.seq + " Logout. (" + this.ID + ")");

            //ログアウトした人を、リストから削除。
            clientList.Remove(this);

            //接続者全員にメッセージを送る
            foreach (var client in clientList)
            {
                client.Send("Seq:" + seq + " Logout.");
            }
        }
    }
}