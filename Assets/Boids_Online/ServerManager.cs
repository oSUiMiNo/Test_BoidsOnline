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
        

        //誰が現在接続しているのか管理するリスト。
        public static List<ExWebSocketBehavior> clientList = new List<ExWebSocketBehavior>();
        //接続者に番号を振るための変数。
        //static int globalSeq = 0;
        //自身の番号
        int seq;

        //誰かがログインしてきたときに呼ばれるメソッド
        protected override void OnOpen()
        {
            //ログインしてきた人には、番号をつけて、リストに登録。
            //globalSeq++;
            //this.seq = globalSeq;
            clientList.Add(this);
            seq = clientList.IndexOf(this);

            Debug.Log("Seq" + this.seq + " Login. (" + this.ID + ")");


            Packet_ClientInfo packet_ClientInfo = new Packet_ClientInfo()
            {
                clientNnmber = seq
            };
            string json = JsonConvert.SerializeObject(packet_ClientInfo);
            this.Send(json);

            Dictionary<string, Packet> packets = new Dictionary<string, Packet>()
            {
                {
                    "ClientInfo",
                    new Packet_ClientInfo()
                    {
                        clientNnmber = seq
                    }
                },
                {
                    "LoadAvatar",
                    new Packet_LoadAvatar()
                    {
                        avatarName = $"avatar{seq}"
                    }
                },
                {
                    "ChangeSeed",
                    new Packet_ChangeSeed()
                    {
                        maxSpeed = 5
                    }
                },
                {
                    "ChangeSize",
                new Packet_ChangeSize()
                    {
                        size = 2
                    }
                }

            };
          

            playerInfomations.Add(packets);

            string json1 = JsonConvert.SerializeObject(playerInfomations);

            Debug.Log(json1);

            //string json = JsonConvert.SerializeObject(playerInfomations[seq - 1]["LoadAvatar"]);

            foreach (var a in playerInfomations)
            {
                foreach (var b in a)
                {
                    string json2 = JsonConvert.SerializeObject(b);
                    clientList[seq - 1].Send(json2);
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
                string json;

                Packet_Text packet_Text = new Packet_Text()
                {
                    text = $"Seq:{seq}  Logout."
                };
                json = JsonConvert.SerializeObject(packet_Text);
                client.Send(json);

                int newSeq = clientList.IndexOf(client);
                Packet_ClientInfo packet_ClientInfo = new Packet_ClientInfo()
                {
                    clientNnmber = newSeq
                };
                json = JsonConvert.SerializeObject(packet_ClientInfo);
                client.Send(json);
            }
        }
    }
}