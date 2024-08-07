using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using UnityEngine;
using Microsoft.AspNetCore.SignalR.Client;
using System.Threading.Tasks;
using MessagePack;
using Newtonsoft.Json;

/// <summary>
/// Manages WebSocket connections for Chat system using SignalR.
/// </summary>
public class ChatSocketManager : WebGLChatSocketManager {
  
  public static ChatSocketManager Instance;
  public static bool HasInstance() => Instance != null;
  public delegate void MessageHandler(string message, MessageTypeEnum messageType);
  
  protected override string GetAlias() => "Chat Socket V2";
  
  private readonly Dictionary<string, MessageHandler> _messageHandlers = new();

  
  private long _factionJoinTime;

  public override void Awake() {
    if (Instance == null) {
      Instance = this;
    }
    else if (Instance != this) {
      DestroyImmediate(gameObject);
    }

    base.Awake();
  }
  
  /// <summary>
  /// Subscribe messageHandler with channel name
  /// </summary>
  /// <param name="channelName"></param>
  /// <param name="handler"></param>
  public void Subscribe(string channelName, MessageHandler handler) {
    Debug.LogError("<color=cyan>Subscribe to Chat socket </color>" + channelName);
    _messageHandlers[channelName] = handler;
  }
  
  /// <summary>
  /// Remove channel name that is subscribed
  /// </summary>
  /// <param name="channelName"></param>
  public void Unsubscribe(string channelName) {
    _messageHandlers.Remove(channelName);
  }

  public void SetFactionJoinTime(long factionJoinTime) {
    // Debug.LogError("<color=cyan>SetFactionJoinTime </color>" + factionJoinTime);
    _factionJoinTime = factionJoinTime;
  }
  
  public override async void Init() {
    if (IsSocketConnectionCreated()) {
      return;
    }

    CurrentState = ConnectionState.Invalid;
    NetworkReachability = NetworkReachability.ReachableViaLocalAreaNetwork;
    //Check if _cookies = null & player has joined a faction -> call api to get cookies.
    // if (_cookies == null && !string.IsNullOrEmpty(PlayerInfoScript.Instance.AccessTokenModel.FactionId)) {
    //   Multiplayer.Multiplayer.GetJwtToken(OnGetJwtTokenDone).Execute();
    // }
    // else {
      await ConnectSocket();
    // }
  }

  private async void OnGetJwtTokenDone(bool success, Dictionary<string, object> data) {
    await ConnectSocket();
  }

  protected override CookieContainer GetRequestCookies() {
    // if (_cookies is { Count: > 0 }) {
    //   var cookieContainer = new CookieContainer();
    //   foreach (var cookie in _cookies) {
    //     cookieContainer.Add(new Uri(SQSettings.CHAT_SERVER_URL), cookie);
    //   }
    //
    //   return cookieContainer;
    // }

    return null;
  }
  
  protected override bool ShouldIncludeCookiesInHeader() {
    return true;
  }
  
  protected override string GetCookieString() {
    // if (_cookies is { Count: > 0 }) {
    //   return BattleUtils.ConvertToJsonString(_cookies);
    // }

    return string.Empty;
  }
  
  public List<Cookie> GetCookiesA()
  {
    var cookies = new List<Cookie>();

    // Server A cookies
    cookies.Add(new Cookie 
    { 
      Name = "AWSALB", 
      Value = "jRIBMWuPTb5KTZmV4L8NXGIyWz396HESOOPHKUS3ynBV9k0yCrm7ucm2R4IryjywzeYTQVapVeXdwEVZafh4n1VpxGY7EHeWuATSItGVFBkWrDWRlG4m+xoBJFsr", 
      Path = "/", 
      Domain = "172.23.93.105" 
    });
    cookies.Add(new Cookie 
    { 
      Name = "AWSALBCORS", 
      Value = "jRIBMWuPTb5KTZmV4L8NXGIyWz396HESOOPHKUS3ynBV9k0yCrm7ucm2R4IryjywzeYTQVapVeXdwEVZafh4n1VpxGY7EHeWuATSItGVFBkWrDWRlG4m+xoBJFsr", 
      Path = "/", 
      Domain = "172.23.93.105" 
    });
    

    return cookies;
  }
  
  
  public List<Cookie> GetCookiesB()
  {
    var cookies = new List<Cookie>();
    

    cookies.Add(new Cookie 
    { 
      Name = "AWSALB", 
      Value = "JrMnwEpA9OK1PcKouYikvf7E7ZXWLVAGika2AhlVmiaxzeyDMPBgmA3j/f8ewHVTAZcwanfdxklmPCJFD3TBWkEq3cDrveLH3E7QxQ/xVhXMdMJQduPeZR8oCxua", 
      Path = "/", 
      Domain = "172.23.92.242" 
    });
    cookies.Add(new Cookie 
    { 
      Name = "AWSALBCORS", 
      Value = "JrMnwEpA9OK1PcKouYikvf7E7ZXWLVAGika2AhlVmiaxzeyDMPBgmA3j/f8ewHVTAZcwanfdxklmPCJFD3TBWkEq3cDrveLH3E7QxQ/xVhXMdMJQduPeZR8oCxua", 
      Path = "/", 
      Domain = "172.23.92.242" 
    });

    return cookies;
  }


  protected override string GetAddress() {
   // string add =
   //  "chatHub?globalChannel=ChatGlobal_English1&access_token=eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJleHAiOjE3MjMwODU3NDYsInVzZXJJZCI6IjA0ODY1ZTZmNGRjODQwMTlhYzM5ZDI0ODlkMTUzMjg1IiwiY2xpZW50VmVyc2lvbiI6IjAuNjc2IiwiZmFjdGlvbklkIjoiNmFlOTEyMGUxM2U3NGNhMmI0OGZlNzY0YzQ5YmVjOGQiLCJ2YXJpYW50SWRzIjoiSURGQV9OZXdVc2Vyc1ZhckEifQ.0gP85gvUWOudPTfbkECQlim1-UTUSb6CcwZCc6ephU4&faction_join_time=1721199480206";
   string add =
     "chatHub?globalChannel=ChatGlobal_English4&access_token=eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJleHAiOjE3MjMyNjYwMzMsInVzZXJJZCI6ImRiOTE5YjZlZTdkODQzNzNhZDQyOGI1NTdhODk5MzliIiwiY2xpZW50VmVyc2lvbiI6IjAuNjc1IiwiZmFjdGlvbklkIjoiYjE1M2E4Yjk5ZjllNGUyMzk2YWI2NjhlMGNmM2FmOTAiLCJ2YXJpYW50SWRzIjpudWxsfQ.gZlgkQ9z55Hyk2tcHG2Ux4MatZjrl4MZc9XDkzuIlDs&faction_join_time=1722938197109";
   string serverAddress = "https://sgs-webgl-staging-chat.whiplashapi.com";
   string address = $"{serverAddress}/{add}";
   return address;
  }
  
  protected override Task OnConnected() {
    Debug.LogError("<color=cyan>Chatsocket connected</color>");
    FetchGlobalChatMessage();
    return base.OnConnected();
  }
  
  protected override void OnError(HubConnection connection, string error) {
    base.OnError(connection, error);
    SetConnectState(ConnectionState.SocketError);
  }
  
  
  protected override int GetReconnectMillisecondsDelay() {
    return 1500;
  }
  
  
  /// <summary>
  /// Register socket event to receive messages
  /// </summary>
  protected override void RegisterSocketEvent() {
    On<string>("ReceiveGlobalMessage", message => {
      Debug.LogError("<color=cyan>ReceiveGlobalMessage 1</color>" + message);
      var byteArray = MessagePackSerializer.ConvertFromJson(message);
      Debug.LogError("<color=cyan>ReceiveGlobalMessage 2 </color>" + byteArray.Length);
      var result = MessagePackSerializer.Deserialize<MessageResponseModel>(byteArray, serializerOptions);
      Debug.LogError("<color=cyan>result </color>" +
                     JsonConvert.DeserializeObject<ChatMessageModel>(result.MessageData));
    });
      
    On<string>("ReceiveFactionMessage", message => {
      Debug.LogError("<color=cyan>ReceiveFactionMessage 1</color>" + message);
      var byteArray = MessagePackSerializer.ConvertFromJson(message);
      Debug.LogError("<color=cyan>ReceiveFactionMessage 2 </color>" + byteArray.Length);
      var result = MessagePackSerializer.Deserialize<MessageResponseModel>(byteArray, serializerOptions);
      Debug.LogError("<color=cyan>ReceiveFactionMessage result </color>" +
                     JsonConvert.DeserializeObject<ChatMessageModel>(result.MessageData));
    });
      
    On<string>("ReceiveFactionFeudMessage", message => {
      Debug.LogError("<color=cyan>ReceiveFactionFeudMessage 1</color>" + message);
      var byteArray = MessagePackSerializer.ConvertFromJson(message);
      Debug.LogError("<color=cyan>ReceiveFactionFeudMessage 2 </color>" + byteArray.Length);
      var result = MessagePackSerializer.Deserialize<MessageResponseModel>(byteArray, serializerOptions);
      Debug.LogError("<color=cyan>ReceiveFactionFeudMessage result </color>" +
                     JsonConvert.DeserializeObject<ChatMessageModel>(result.MessageData));
    });
  }

  // private void OnReceiveMessage(MessageResponseModel message, ChatChannelType chatType) {
  //   Debug.LogError($"<color=cyan>OnReceive {chatType} Message </color>" + message.MessageType + " - " + message.MessageData);
  //   if (!string.IsNullOrEmpty(message.MessageData)) {
  //     var channelName = ChatChannelManager.Instance.GetChannel(chatType).channelName;
  //
  //     if (!string.IsNullOrEmpty(channelName)) {
  //       //Force message handler invoke on MainThread to update UI
  //       MainThread.Instance.Call(() => {
  //         _messageHandlers[channelName]?.Invoke(message.MessageData, message.MessageType);
  //       });
  //     }
  //   }
  // }
  
  /// <summary>
  /// Fetch all global chat messages
  /// </summary>
  public async void FetchGlobalChatMessage() {
    Debug.LogError("<color=cyan>start FetchGlobalChatMessage</color>");
    Invoke<string>("FetchGlobalMessagesHistory", 
      handler: responseMsg => {
        Debug.LogError("<color=cyan>InvokeGlobalMessageCallback: </color>" + responseMsg);
        // if (!string.IsNullOrEmpty(responseMsg)){
        //   try {
        //     var byteArray = MessagePackSerializer.ConvertFromJson(responseMsg);
        //     var deserializeData = MessagePackSerializer.Deserialize<List<MessageResponseModel>>(byteArray, serializerOptions);
        //     if (deserializeData is { Count: > 0 }) {
        //       ChatChannel channel = ChatChannelManager.Instance.GetChannel(ChatChannelType.GLOBAL);
        //       if (!string.IsNullOrEmpty(channel.channelName)) {
        //         foreach (var item in deserializeData) {
        //           _messageHandlers[channel.channelName]?.Invoke(item.MessageData, item.MessageType);
        //         }
        //       }
        //     }
        //   }
        //   catch {
        //     // ignored
        //   }
        // }
      });
  }
  
  
  /// <summary>
  /// Fetch all faction chat messages
  /// </summary>
  public async void FetchFactionChatMessage(Action callback = null) {
    Invoke<string>("FetchFactionChatMessage", 
      handler: responseMsg => {
        Debug.LogError("<color=cyan>InvokeFactionMessageCallback: </color>" + responseMsg);
        // if (!string.IsNullOrEmpty(responseMsg)){
        //   try {
        //     var byteArray = MessagePackSerializer.ConvertFromJson(responseMsg);
        //     var deserializeData = MessagePackSerializer.Deserialize<List<MessageResponseModel>>(byteArray, serializerOptions);
        //     if (deserializeData is { Count: > 0 }) {
        //       ChatChannel channel = ChatChannelManager.Instance.GetChannel(ChatChannelType.FACTION);
        //       if (!string.IsNullOrEmpty(channel.channelName)) {
        //         foreach (var item in deserializeData) {
        //           _messageHandlers[channel.channelName]?.Invoke(item.MessageData, item.MessageType);
        //         }
        //       }
        //     }
        //   }
        //   catch {
        //     // ignored
        //   }
        // }
      });
    
    
    callback?.Invoke();
  }
  
    
  /// <summary>
  /// Fetch all faction feud chat messages
  /// </summary>
  public async void FetchFactionFeudChatMessage(string teamId, Action callback = null) {
    // var messages = await HubConnection.InvokeAsync<List<MessageResponseModel>?>("FetchFactionFeudMessagesHistory", teamId);
    // Debug.LogError("<color=cyan>FetchFactionFeudChatMessage </color>" + BattleUtils.ConvertToJsonString(messages));
    // if (messages != null) {
    //   ChatChannel channel = ChatChannelManager.Instance.GetChannel(ChatChannelType.FACTION_FEUD);
    //   if (!string.IsNullOrEmpty(channel.channelName)) {
    //     foreach (var message in messages) {
    //       _messageHandlers[channel.channelName]?.Invoke(message.MessageData, message.MessageType);
    //     }
    //   }
    // }
    
    Invoke<string>("FetchFactionFeudMessagesHistory", teamId,
      handler: responseMsg => {
        Debug.LogError("<color=cyan>InvokeFactionFeudMessageCallback: </color>" + responseMsg);
        if (!string.IsNullOrEmpty(responseMsg)){
          try {
            var byteArray = MessagePackSerializer.ConvertFromJson(responseMsg);
            var deserializeData = MessagePackSerializer.Deserialize<List<MessageResponseModel>>(byteArray, serializerOptions);
            // if (deserializeData is { Count: > 0 }) {
            //   ChatChannel channel = ChatChannelManager.Instance.GetChannel(ChatChannelType.FACTION_FEUD);
            //   if (!string.IsNullOrEmpty(channel.channelName)) {
            //     foreach (var item in deserializeData) {
            //       _messageHandlers[channel.channelName]?.Invoke(item.MessageData, item.MessageType);
            //     }
            //   }
            // }
          }
          catch {
            // ignored
          }
        }
      });

    callback?.Invoke();
  }
  
  
  /// <summary>
  /// Send global chat message to the server
  /// </summary>
  /// <param name="message"></param>
  public async void SendGlobalChat(ChatMessageModel message) {
    try {
      var msgStr = MessagePackSerializer.SerializeToJson(message, serializerOptions);
      Invoke("SendGlobalMessage", msgStr);
    }
    catch (Exception e) {
      Debug.LogError("<color=cyan>SendGlobalChat Error: </color>" + e);
    }
  }
  
  /// <summary>
  /// Send faction chat message to server
  /// </summary>
  /// <param name="message"></param>
  public async void SendFactionChat(ChatMessageModel message) {
    try {
      var msgStr = MessagePackSerializer.SerializeToJson(message, serializerOptions);
      Invoke("SendFactionMessage", msgStr);
    }
    catch (Exception e) {
      Debug.LogError("<color=cyan>SendFactionMessage Error: </color>" + e);
    }
  }
  
  
  /// <summary>
  /// Send faction feud chat message to server
  /// </summary>
  /// <param name="message"></param>
  /// <param name="teamId">team id from FF brawl</param>
  /// <param name="expiredAt">brawl end time</param>
  public async void SendFactionFeudChat(ChatMessageModel message, string teamId, long expiredAt) {
    try {
      //Must convert params to string to avoid Exception: 
      //System.Reflection.TargetInvocationException: Exception has been thrown by the target of an invocation. --->
      //System.ExecutionEngineException: Attempting to call method 'MessagePack.MessagePackSerializer::Serialize<System.Int64>' for which no ahead of time (AOT) code was generated.
      var msgStr = MessagePackSerializer.SerializeToJson(message, serializerOptions);
      Invoke("SendFactionFeudMessage", msgStr, teamId, expiredAt.ToString(CultureInfo.InvariantCulture));
    }
    catch (Exception e) {
      Debug.LogError("<color=cyan>SendFactionFeudChat Error: </color>" + e);
    }
  }
  
  // public void SaveChatCookie(List<object> cookieList) {
  //   // Debug.LogError("<color=cyan>SaveChatCookie </color>" + (cookieList == null ? "null" : BattleUtils.ConvertToJsonString(cookieList)));
  //   if (cookieList == null) {
  //     _cookies = null;
  //   }
  //   else {
  //     _cookies = new List<Cookie>();
  //     foreach (var cookie in cookieList) {
  //       var chatCookie = (Dictionary<string, object>)cookie;
  //       _cookies.Add(new Cookie {
  //         Name = TFUtils.LoadString(chatCookie, "name", ""),
  //         Value = TFUtils.LoadString(chatCookie, "value", ""),
  //         Domain = TFUtils.LoadString(chatCookie, "domain", ""),
  //         Path = TFUtils.LoadString(chatCookie, "path", ""),
  //         Secure = TFUtils.LoadBool(chatCookie, "secure", false)
  //       });
  //     }
  //   }
  // }
}
