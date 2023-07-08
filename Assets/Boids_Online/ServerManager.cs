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
        //�|�[�g�ԍ����w��
        ws = new WebSocketServer(12345);
        //�N���C�A���g����̒ʐM���̋������`�����N���X�A�uExWebSocketBehavior�v��o�^
        ws.AddWebSocketService<ExWebSocketBehavior>("/");
        //�T�[�o�N��
        ws.Start();
        Debug.Log("�T�[�o�N��");
    }
    private void OnApplicationQuit()
    {
        Debug.Log("�T�[�o��~");
        ws.Stop();
    }




    public class ExWebSocketBehavior : WebSocketBehavior
    {
        public List<Dictionary<string, Packet>> playerInfomations = new List<Dictionary<string, Packet>>();
        

        //�N�����ݐڑ����Ă���̂��Ǘ����郊�X�g�B
        public static List<ExWebSocketBehavior> clientList = new List<ExWebSocketBehavior>();
        //�ڑ��҂ɔԍ���U�邽�߂̕ϐ��B
        //static int globalSeq = 0;
        //���g�̔ԍ�
        int seq;

        //�N�������O�C�����Ă����Ƃ��ɌĂ΂�郁�\�b�h
        protected override void OnOpen()
        {
            //���O�C�����Ă����l�ɂ́A�ԍ������āA���X�g�ɓo�^�B
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

        //�N�������b�Z�[�W�𑗐M���Ă����Ƃ��ɌĂ΂�郁�\�b�h
        protected override void OnMessage(MessageEventArgs e)
        {
            Debug.Log("Seq:" + seq + "..." + e.Data);

            JObject model = JObject.Parse(e.Data);

            playerInfomations[seq - 1][(string)model["funcName"]] = (Packet)(object)model;
            
            //�ڑ��ґS���Ƀ��b�Z�[�W�𑗂�
            foreach (var client in clientList)
            {
                if(client != clientList[seq - 1])
                {
                    client.Send(e.Data);
                }
            }
        }

        //�N�������O�A�E�g���Ă����Ƃ��ɌĂ΂�郁�\�b�h
        protected override void OnClose(CloseEventArgs e)
        {
            Debug.Log("Seq" + this.seq + " Logout. (" + this.ID + ")");

            //���O�A�E�g�����l���A���X�g����폜�B
            clientList.Remove(this);


            //�ڑ��ґS���Ƀ��b�Z�[�W�𑗂�
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