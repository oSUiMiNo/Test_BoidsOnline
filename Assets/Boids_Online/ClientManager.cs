using UnityEngine;
using UnityEngine.UI;
using TMPro;
using WebSocketSharp;
using Newtonsoft.Json;

public class ClientManager : MonoBehaviourMyExtention
{
    WebSocket ws;

    ServerClientCommon common;


    //�T�[�o�ցA���b�Z�[�W�𑗐M����
    public void SendText()
    {
        ws.Send("");
    }

    //�T�[�o����󂯎�������b�Z�[�W���AChatText�ɕ\������
    public void RecvText(string json)
    {
        var loadData = JsonConvert.DeserializeObject<LoadAvatarJModel>(json);
        if(loadData.funcName == "LoadAvatar")
        {
            common.LoadAvatar(loadData.avatarName);
        }
    }

    //�T�[�o�̐ڑ����؂ꂽ�Ƃ��̃��b�Z�[�W���AChatText�ɕ\������
    public void RecvClose()
    {

    }

    void Start()
    {
        common = GetComponent<ServerClientCommon>();

        //ws = new WebSocket("ws://192.168.1.13:12345/"); //�������Ƃ�wifi
        ws = new WebSocket("ws://localhost:12345/");  //���[�J�� ( ��PC�� ) �̃T�[�o�[�Ƃ���肷�鎞
        ws.Connect();

        //�T�[�o���烁�b�Z�[�W����M�����Ƃ��Ɏ��s���鏈���uRecvText�v��o�^����
        ws.OnMessage += (sender, e) => RecvText(e.Data);
        //�T�[�o�Ƃ̐ڑ����؂ꂽ�Ƃ��Ɏ��s���鏈���uRecvClose�v��o�^����
        ws.OnClose += (sender, e) => RecvClose();
    }
}

