using UnityEngine;
using UnityEngine.UI;
using TMPro;
using WebSocketSharp;
using Newtonsoft.Json;
using PlasticGui;

public class ClientManager : MonoBehaviourMyExtention
{
    WebSocket ws;

    public TextMeshProUGUI chatText;
    public TMP_InputField messageInput;
    public Button sendButton;

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
        chatText.text = ("Close.");
    }

    void Start()
    {
        common = GetComponent<ServerClientCommon>();

        ///<summary>
        /// �ڑ������B�ڑ���T�[�o�ƁA�|�[�g�ԍ����w�肷��
        /// �T�[�o�[�̓R�}���h�v�����v�g�́uipconfig�v�Ƃ��Ŋm�F���āA
        /// �|�[�g�ԍ��́A�T�[�o�[�p�̃N���X�Ŏw�肵���C�ӂ̐���
        /// </summary>
        //ws = new WebSocket("ws://10.70.13.200:12345/"); //�������˂̉Ƃ�wifi
        //ws = new WebSocket("ws://10.11.183.249:12345/");  //�K�X�g��wifi ����q���Ȃ����ƕς����ۂ��B
        //ws = new WebSocket("ws://192.168.1.13:12345/"); //�������Ƃ�wifi
        ws = new WebSocket("ws://localhost:12345/");  //���[�J�� ( ��PC�� ) �̃T�[�o�[�Ƃ���肷�鎞
        ws.Connect();

        //���M�{�^���������ꂽ�Ƃ��Ɏ��s���鏈���uSendText�v��o�^����
        sendButton.onClick.AddListener(SendText);
        //�T�[�o���烁�b�Z�[�W����M�����Ƃ��Ɏ��s���鏈���uRecvText�v��o�^����
        ws.OnMessage += (sender, e) => RecvText(e.Data);
        //�T�[�o�Ƃ̐ڑ����؂ꂽ�Ƃ��Ɏ��s���鏈���uRecvClose�v��o�^����
        ws.OnClose += (sender, e) => RecvClose();
    }
}

