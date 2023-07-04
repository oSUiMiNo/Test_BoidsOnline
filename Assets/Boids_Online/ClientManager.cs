using UnityEngine;
using UnityEngine.UI;
using TMPro;
using WebSocketSharp;
using Newtonsoft.Json;

public class ClientManager : MonoBehaviourMyExtention
{
    public WebSocket ws;

    ServerClientCommon common;

    //�T�[�o�ցA���b�Z�[�W�𑗐M����
    public void SendText()
    {
        ws.Send("");
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
        ws.OnMessage += (sender, e) => common.json_Received = e.Data;
        ws.OnMessage += (sender, e) => messageReceived = true;

        OnMessage += () =>
        {
            common.Exequte();
            common.json_Received = string.Empty;
        };

        InputEventHandler.OnKeyDown_A += () => common.LoadAvatar(true);
        InputEventHandler.OnKeyDown_S += () => common.ChangeSpeed(true, 15);
        
        //�T�[�o�Ƃ̐ڑ����؂ꂽ�Ƃ��Ɏ��s���鏈���uRecvClose�v��o�^����
        ws.OnClose += (sender, e) => RecvClose();
    }


    //WebSocket �� OnMessage ���Ɖ��̂� Resources ����̃��[�h���o���Ȃ��̂Ŏd���Ȃ�����t���O�ŕʂ� Action ����ĊԐړI�ɂ��
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

