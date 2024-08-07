using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using GearMessagePack.Resolvers;
using MessagePack;
using MessagePack.Resolvers;
using UnityEngine;
using Newtonsoft.Json;
using UnityEngine.UI;

public class TestCookies : MonoBehaviour {
    [SerializeField] private Text connectionStatusTxt;
    SignalR signalR;
    
    IFormatterResolver compositeResolver = CompositeResolver.Create(
        GeneratedResolver.Instance,
        DynamicGenericResolver.Instance,
        MessagePack.Unity.Extension.UnityBlitWithPrimitiveArrayResolver.Instance,
        MessagePack.Unity.UnityResolver.Instance,
        StandardResolver.Instance
    );
    MessagePackSerializerOptions serializerOptions => MessagePackSerializerOptions.Standard.WithResolver(compositeResolver);
    // Start is called before the first frame update
    string localAddressHung = "http://10.0.104.155:5000";
    async void Start() {
        OnClickConnectTungServer();
    }

    public List<Cookie> GetCookies() {
        var cookies = new List<Cookie>();

        Cookie awsAlbCORS = new Cookie
        {
            Name = "AWSALBCORS",
            Value = "VmR+l8yLrk7t3ZrNmg13tUfLmteaWHLp1RbTP4wLZmxNgZ4rEdlbmXjfMhHXXKlpLcGHwShz3e8IK2XxhB4QY0Kz0lau9xaHh5cRcqJeaF1zBs4UDt8uaNc8pqQj",
            Domain = "sgs-02-staging-chat.whiplashapi.com",
            Path = "/",
            HttpOnly = false,
            Secure = true
        };
        cookies.Add(awsAlbCORS);

        Cookie awsAlb = new Cookie
        {
            Name = "AWSALB",
            Value = "VmR+l8yLrk7t3ZrNmg13tUfLmteaWHLp1RbTP4wLZmxNgZ4rEdlbmXjfMhHXXKlpLcGHwShz3e8IK2XxhB4QY0Kz0lau9xaHh5cRcqJeaF1zBs4UDt8uaNc8pqQj",
            Domain = "sgs-02-staging-chat.whiplashapi.com",
            Path = "/",
            HttpOnly = false,
            Secure = false
        };
        cookies.Add(awsAlb);

        return cookies;
    }

    private void FetchGlobalMessage() {
        #if UNITY_EDITOR
        signalR.Invoke<List<MessageResponseModel>>("FetchGlobalMessagesHistory", 
            handler:  InvokeGlobalMessageCallback);
        #elif UNITY_WEBGL && !UNITY_EDITOR
        signalR.Invoke<string>("FetchGlobalMessagesHistory", 
            handler:  InvokeGlobalMessageCallback);
        #endif
    }


    public void SendGlobalChat() {
        ChatMessageModel dummyMessage = new ChatMessageModel {
            UDID = "111",
            UserName = "abc",
            Message = "Hello, this is the global message!",
            AllianceID = "alliance001",
            TargetAlliance = "alliance002",
            TeamID = "team001",
            Portrait = "profile.png",
            IsRecruitmentMessage = false,
            AllianceLogo = "alliance_logo.png",
            TimeOfSubmission = DateTime.Now,
            TimestampSubmissionMS = DateTimeOffset.Now.ToUnixTimeMilliseconds(),
            Type = "Chat",
            Polarity = "Neutral",
            CustomData = new Dictionary<string, object>()
        };
        
        
        var msgStr = MessagePackSerializer.SerializeToJson(dummyMessage, serializerOptions);
        
        Debug.LogError("<color=cyan>SendGlobalChat </color>" + msgStr);
        signalR.Invoke("SendGlobalMessage", msgStr);
    }

    public void SendTestMessage() {
        ChatMessageModel dummyMessage = new ChatMessageModel {
            UDID = "111",
            UserName = "abc",
            Message = "Hello, this is the global message!",
            AllianceID = "alliance001",
            TargetAlliance = "alliance002",
            TeamID = "team001",
            Portrait = "profile.png",
            IsRecruitmentMessage = false,
            AllianceLogo = "alliance_logo.png",
            TimeOfSubmission = DateTime.Now,
            TimestampSubmissionMS = DateTimeOffset.Now.ToUnixTimeMilliseconds(),
            Type = "Chat",
            Polarity = "Neutral",
            CustomData = new Dictionary<string, object>()
        };
        var msgStr = MessagePackSerializer.SerializeToJson(dummyMessage, serializerOptions);
        
        Debug.LogError("<color=cyan>SendTestMessage </color>" + msgStr);
        signalR.Invoke("SendTestMessage", msgStr);
    }
    
    public void SendFactionFeudChat() {
        var dummyMessage = new ChatMessageModel {
            UDID = "111",
            UserName = "name1",
            Message = "Faction feud message!",
            AllianceID = "alliance001",
            TargetAlliance = "alliance002",
            TeamID = "111",
            TimeOfSubmission = DateTime.Now,
            TimestampSubmissionMS = DateTimeOffset.Now.ToUnixTimeMilliseconds(),
            Type = "Chat",
            Polarity = "Neutral"
        };
        var expiredTime = DateTime.Now.AddMinutes(5).Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds;
        // await HubConnection.InvokeAsync<MessageResponseModel>("SendFactionFeudMessage", message, teamId, expiredAt.ToString());
        var msgStr = MessagePackSerializer.SerializeToJson(dummyMessage, serializerOptions);
        signalR.Invoke("SendFactionFeudMessage", msgStr, dummyMessage.TeamID, expiredTime.ToString(CultureInfo.InvariantCulture));
    }
    
    public void InvokeMessageCallback(string message)
    {
        Debug.LogError("Callback received from JavaScript: " + message);
    }
    
    public void InvokeGlobalMessageCallback(string message)
    {

        Debug.LogError("<color=cyan>InvokeGlobalMessageCallback: </color>" + message);
        byte[] byteArray = MessagePackSerializer.ConvertFromJson(message);

        var result = MessagePackSerializer.Deserialize<List<MessageResponseModel>>(byteArray, serializerOptions);
        Debug.LogError("<color=cyan>parse message: </color>" + result.Count);
        foreach (var item in result) {
            Debug.LogError(item.MessageData);
        }
    }
    
    public void InvokeGlobalMessageCallback(List<MessageResponseModel> result)
    {
        Debug.LogError("<color=cyan>parse message: </color>" + result.Count);
        foreach (var item in result) {
            Debug.LogError(item.MessageData);
        }
    }

    public void OnClickFetchGlobalMessage() {
        Debug.LogError("<color=cyan>OnClickFetchGlobalMessage</color>");
        FetchGlobalMessage();
    }
    
    public void OnClickSendGlobalMessage() {
        Debug.LogError("<color=cyan>OnClickSendGlobalMessage</color>");
        SendGlobalChat();
    }
    
    public void OnClickSendTestMessage() {
        Debug.LogError("<color=cyan>OnClickSendTestMessage</color>");
        SendTestMessage();
    }

    private string add =
        "chatHub?globalChannel=ChatGlobal_English1&access_token=eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJleHAiOjE3MjMwODU3NDYsInVzZXJJZCI6IjA0ODY1ZTZmNGRjODQwMTlhYzM5ZDI0ODlkMTUzMjg1IiwiY2xpZW50VmVyc2lvbiI6IjAuNjc2IiwiZmFjdGlvbklkIjoiNmFlOTEyMGUxM2U3NGNhMmI0OGZlNzY0YzQ5YmVjOGQiLCJ2YXJpYW50SWRzIjoiSURGQV9OZXdVc2Vyc1ZhckEifQ.0gP85gvUWOudPTfbkECQlim1-UTUSb6CcwZCc6ephU4&faction_join_time=1721199480206";

    public void OnClickConnectWebGLServer() {
        string serverAddress = "https://sgs-webgl-staging-chat.whiplashapi.com";
        string address = $"{serverAddress}/{add}";
        ConnectServer(address, "Connecting to WebGL server...");
    }
    
    public void OnClickConnectTungServer() {
        string localAddressTung = "http://10.0.144.17:5000";
        string address = $"{localAddressTung}/{add}";
        ConnectServer(address, "Connecting to Tung's server...");
    }

    public void OnClickSendFFMessage() {
        SendFactionFeudChat();
    }

    private async void ConnectServer(string address, string connectingText) {
        if (signalR != null) {
            connectionStatusTxt.text = "Restarting connection...";
            signalR.Stop();
            await Task.Delay(3000);
        }

        connectionStatusTxt.text = connectingText;
        signalR = new SignalR();
        signalR.Init(address, JsonConvert.SerializeObject(GetCookies()));
        Debug.LogError("<color=cyan>start connect</color>");
        signalR.On<string>("ReceiveGlobalMessage", message => {
            Debug.LogError("<color=cyan>ReceiveGlobalMessage 1</color>" + message);
            var byteArray = MessagePackSerializer.ConvertFromJson(message);
            Debug.LogError("<color=cyan>ReceiveGlobalMessage 2 </color>" + byteArray.Length);
            var result = MessagePackSerializer.Deserialize<MessageResponseModel>(byteArray, serializerOptions);
            Debug.LogError("<color=cyan>result </color>" +
                           JsonConvert.DeserializeObject<ChatMessageModel>(result.MessageData));
        });

        
        signalR.On<string>("ReceiveFactionFeudMessage", message => {
            Debug.LogError("<color=cyan>ReceiveFactionFeudMessage 1</color>" + message);
            var byteArray = MessagePackSerializer.ConvertFromJson(message);
            Debug.LogError("<color=cyan>ReceiveFactionFeudMessage 2 </color>" + byteArray.Length);
            var result = MessagePackSerializer.Deserialize<MessageResponseModel>(byteArray, serializerOptions);
            Debug.LogError("<color=cyan>ReceiveFactionFeudMessage result </color>" +
                           JsonConvert.DeserializeObject<ChatMessageModel>(result.MessageData));
        });
        
        signalR.Connect();
        signalR.ConnectionStarted += (object sender, ConnectionEventArgs e) => {
            // Log the connected ID
            Debug.LogError($"Connected: {e.ConnectionId}");
            connectionStatusTxt.text = "Connected";
            // SendGlobalChat();
            // FetchGlobalMessage();
        };
    }


}
