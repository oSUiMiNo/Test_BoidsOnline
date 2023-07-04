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
        Dictionary<string, Packet> packets = new Dictionary<string, Packet>();


        //�N�����ݐڑ����Ă���̂��Ǘ����郊�X�g�B
        public static List<ExWebSocketBehavior> clientList = new List<ExWebSocketBehavior>();
        //�ڑ��҂ɔԍ���U�邽�߂̕ϐ��B
        static int globalSeq = 0;
        //���g�̔ԍ�
        int seq;

        //�N�������O�C�����Ă����Ƃ��ɌĂ΂�郁�\�b�h
        protected override void OnOpen()
        {
            //���O�C�����Ă����l�ɂ́A�ԍ������āA���X�g�ɓo�^�B
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
            //�ڑ��ґS���Ƀ��b�Z�[�W�𑗂�
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
                client.Send("Seq:" + seq + " Logout.");
            }
        }
    }
}