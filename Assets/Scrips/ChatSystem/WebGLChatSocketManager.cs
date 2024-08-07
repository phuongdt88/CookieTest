using System.Collections;
using System.Collections.Generic;
using UniRx.Async;
using UnityEngine;
using System;
using System.Net;
using System.Runtime.InteropServices;
using AOT;
using System.Threading.Tasks;
using GearMessagePack.Resolvers;
using MessagePack;
using MessagePack.Resolvers;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

public class WebGLChatSocketManager : GearSocketManagerV2 {
  private static WebGLChatSocketManager _instance;
  // public static bool HasInstance() => Instance != null;

  protected override string GetAlias() => "WebGL socket manager v2";

  protected List<Cookie> _cookies;

  protected bool _connectionCreated;

  readonly IFormatterResolver _compositeResolver = CompositeResolver.Create(
    GeneratedResolver.Instance,
    DynamicGenericResolver.Instance,
    MessagePack.Unity.Extension.UnityBlitWithPrimitiveArrayResolver.Instance,
    MessagePack.Unity.UnityResolver.Instance,
    StandardResolver.Instance
  );
  protected MessagePackSerializerOptions serializerOptions => MessagePackSerializerOptions.Standard.WithResolver(_compositeResolver);

  public virtual void Awake() {
    Debug.LogError("<color=cyan>WebGLChatSocketManager awake 1</color>");
    if (_instance == null) {
      Debug.LogError("<color=cyan>WebGLChatSocketManager awake 2</color>");
      _instance = this;
    }
    else if (_instance != this) {
      DestroyImmediate(gameObject);
    }

    // ConnectionStarted += OnConnected;
  }

  // Start is called before the first frame update
  public override async UniTask<bool> ConnectSocket() {
    TFUtils.MyLogError(new object[] { $"{GetAlias()} ___ ConnectSocket", HubConnection }, go: null, color: "magenta");
    if (!IsSocketConnectionCreated()) {
      string address = GetAddress();
      Debug.Log($"{GetAlias()} ___ ConnectSocket: address: {address}");
      if (string.IsNullOrEmpty(address)) {
        Debug.Log($"{GetAlias()} ___ ConnectSocket: address is empty -> stop connecting");
        return false;
      }

      InitJs(address, JsonConvert.SerializeObject(GetCookiesA()), JsonConvert.SerializeObject(GetCookiesB()));


      // ConfigureHubConnectionSettings();
      // // Subscribe to the WS events
      // HubConnection.Closed += OnClosed;
      // HubConnection.Reconnecting += OnReconnecting;
      // HubConnection.Reconnected += OnReconnected;
      RegisterSocketEvent();
    }

    NetworkReachability = Application.internetReachability;
    try {
      ShowSpinning();
      ConnectJs(ConnectedCallback, DisconnectedCallback);
      // await OnConnected();
    }
    catch (Exception e) {
      OnError(null, "ConnectSocket Exception: " + e.Message);
    }

    await UniTask.WaitUntil(() => _connectionCreated);
    Debug.Log($"{GetAlias()} ___ Connected to Socket:");
    return true;
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
      Domain = "sgs-webgl-staging-chat.whiplashapi.com" 
    });
    cookies.Add(new Cookie 
    { 
      Name = "AWSALBCORS", 
      Value = "jRIBMWuPTb5KTZmV4L8NXGIyWz396HESOOPHKUS3ynBV9k0yCrm7ucm2R4IryjywzeYTQVapVeXdwEVZafh4n1VpxGY7EHeWuATSItGVFBkWrDWRlG4m+xoBJFsr", 
      Path = "/", 
      Domain = "sgs-webgl-staging-chat.whiplashapi.com" 
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
      Domain = "sgs-webgl-staging-chat.whiplashapi.com" 
    });
    cookies.Add(new Cookie 
    { 
      Name = "AWSALBCORS", 
      Value = "JrMnwEpA9OK1PcKouYikvf7E7ZXWLVAGika2AhlVmiaxzeyDMPBgmA3j/f8ewHVTAZcwanfdxklmPCJFD3TBWkEq3cDrveLH3E7QxQ/xVhXMdMJQduPeZR8oCxua", 
      Path = "/", 
      Domain = "sgs-webgl-staging-chat.whiplashapi.com" 
    });

    return cookies;
  }

  public override void DisposeHubConnection() {
    if (IsSocketConnectionCreated()) {
      // HubConnection.Closed -= OnClosed;
      // HubConnection.Reconnecting -= OnReconnecting;
      // HubConnection.Reconnected -= OnReconnected;
      UnRegisterSocketEvent();
    }

    _connectionCreated = false;
  }

  protected override bool IsSocketConnectionCreated() {
    return _connectionCreated;
  }

  public virtual bool IsConnectionAvailable() => IsSocketConnectionCreated();

  // protected override Task OnConnected() {
  //   Debug.LogError("<color=cyan>WebGL socket connected</color>");
  //   _connectionCreated = true;
  //   return base.OnConnected();
  // }
  
  protected override Task OnConnected() {
    Debug.LogError("<color=cyan>WebGL socket connected</color>");
    _connectionCreated = true;
    // SetConnectState(ConnectionState.Connected);
    return base.OnConnected();
  }
  
  // public static event Action ConnectionStarted;

  #region ConnectSocket

  [DllImport("__Internal")]
  private static extern void InitJs(string hubUrl, string cookies1, string cookies2);

  [DllImport("__Internal")]
  private static extern void ConnectJs(Action<string> connectedCallback, Action<string> disconnectedCallback);

  [MonoPInvokeCallback(typeof(Action<string>))]
  private static void ConnectedCallback(string connectionId) {
    Debug.LogError("<color=cyan>ConnectedCallback </color>" + _instance);
    // ConnectionStarted?.Invoke();
    _instance.OnConnected();
    
  }

  [MonoPInvokeCallback(typeof(Action<string>))]
  private static void DisconnectedCallback(string connectionId) {
    _instance.OnClosed(null);
  }

  #endregion

  #region Invoke JS

  [DllImport("__Internal")]
  private static extern void InvokeJs(string methodName, string arg1, string arg2, string arg3, string arg4,
    string arg5, string arg6, string arg7, string arg8, string arg9, string arg10, Action<string, string> callback);

  public void Invoke<T>(string methodName, Action<T> handler) {
    // Debug.LogError("<color=cyan> InvokeJS : </color>" + methodName);
    types.Add(methodName, new List<Type> { typeof(T) });
    handlers.Add(methodName, args => handler((T)args[0]));
    InvokeJs(methodName, null, null, null, null, null, null, null, null, null, null, InvokeMessageHandlerCallback);
  }
  
  public void Invoke<T>(string methodName, object arg1, Action<T> handler) {
    types.Add(methodName, new List<Type> { typeof(T) });
    handlers.Add(methodName, args => handler((T)args[0]));
    InvokeJs(methodName, arg1.ToString(), null, null, null, null, null, null, null, null, null,
      InvokeMessageHandlerCallback);
  }

  public void Invoke(string methodName, object arg1) {
    InvokeJs(methodName, arg1.ToString(), null, null, null, null, null, null, null, null, null,
      InvokeMessageHandlerCallback);
  }

  public void Invoke(string methodName, object arg1, object arg2) =>
    InvokeJs(methodName, arg1.ToString(), arg2.ToString(), null, null, null, null, null, null, null, null,
      InvokeMessageHandlerCallback);

  public void Invoke(string methodName, object arg1, object arg2, object arg3) =>
    InvokeJs(methodName, arg1.ToString(), arg2.ToString(), arg3.ToString(), null, null, null, null, null, null, null,
      InvokeMessageHandlerCallback);

  public void Invoke(string methodName, object arg1, object arg2, object arg3, object arg4) =>
    InvokeJs(methodName, arg1.ToString(), arg2.ToString(), arg3.ToString(), arg4.ToString(), null, null, null, null,
      null, null, InvokeMessageHandlerCallback);

  public void Invoke(string methodName, object arg1, object arg2, object arg3, object arg4, object arg5) =>
    InvokeJs(methodName, arg1.ToString(), arg2.ToString(), arg3.ToString(), arg4.ToString(), arg5.ToString(), null,
      null, null, null, null, InvokeMessageHandlerCallback);

  public void Invoke(string methodName, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6) =>
    InvokeJs(methodName, arg1.ToString(), arg2.ToString(), arg3.ToString(), arg4.ToString(), arg5.ToString(),
      arg6.ToString(), null, null, null, null, InvokeMessageHandlerCallback);

  public void Invoke(string methodName, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6,
    object arg7) =>
    InvokeJs(methodName, arg1.ToString(), arg2.ToString(), arg3.ToString(), arg4.ToString(), arg5.ToString(),
      arg6.ToString(), arg7.ToString(), null, null, null, InvokeMessageHandlerCallback);

  public void Invoke(string methodName, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6,
    object arg7, object arg8) =>
    InvokeJs(methodName, arg1.ToString(), arg2.ToString(), arg3.ToString(), arg4.ToString(), arg5.ToString(),
      arg6.ToString(), arg7.ToString(), arg8.ToString(), null, null, InvokeMessageHandlerCallback);

  public void Invoke(string methodName, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6,
    object arg7, object arg8, object arg9) =>
    InvokeJs(methodName, arg1.ToString(), arg2.ToString(), arg3.ToString(), arg4.ToString(), arg5.ToString(),
      arg6.ToString(), arg7.ToString(), arg8.ToString(), arg9.ToString(), null, InvokeMessageHandlerCallback);

  public void Invoke(string methodName, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6,
    object arg7, object arg8, object arg9, object arg10) =>
    InvokeJs(methodName, arg1.ToString(), arg2.ToString(), arg3.ToString(), arg4.ToString(), arg5.ToString(),
      arg6.ToString(), arg7.ToString(), arg8.ToString(), arg9.ToString(), arg10.ToString(),
      InvokeMessageHandlerCallback);

  #endregion

  #region On JS

  private delegate void HandlerAction(params object[] args);

  private static readonly Dictionary<string, List<Type>> types = new();
  private static readonly Dictionary<string, HandlerAction> handlers = new();

  [DllImport("__Internal")]
  private static extern void OnJs(string methodName, string argCount, Action<string, string> handlerCallback);

  [DllImport("__Internal")]
  private static extern void OnJs(string methodName, string argCount, Action<string, string, string> handlerCallback);

  [DllImport("__Internal")]
  private static extern void OnJs(string methodName, string argCount,
    Action<string, string, string, string> handlerCallback);

  [DllImport("__Internal")]
  private static extern void OnJs(string methodName, string argCount,
    Action<string, string, string, string, string> handlerCallback);

  [DllImport("__Internal")]
  private static extern void OnJs(string methodName, string argCount,
    Action<string, string, string, string, string, string> handlerCallback);

  [DllImport("__Internal")]
  private static extern void OnJs(string methodName, string argCount,
    Action<string, string, string, string, string, string, string> handlerCallback);

  [DllImport("__Internal")]
  private static extern void OnJs(string methodName, string argCount,
    Action<string, string, string, string, string, string, string, string> handlerCallback);

  [DllImport("__Internal")]
  private static extern void OnJs(string methodName, string argCount,
    Action<string, string, string, string, string, string, string, string, string> handlerCallback);

  [MonoPInvokeCallback(typeof(Action<string, string>))]
  private static void InvokeMessageHandlerCallback(string methodName, string arg1) {
    // Debug.LogError("<color=cyan>InvokeMessageHandlerCallback </color>" + methodName + " - " + arg1);
    handlers.TryGetValue(methodName, out HandlerAction handler);
    types.TryGetValue(methodName, out var type);
    if (type != null)
      handler?.Invoke(Convert.ChangeType(arg1, type[0]));
  }

  [MonoPInvokeCallback(typeof(Action<string, string>))]
  private static void HandlerCallback1(string methodName, string arg1) {
    handlers.TryGetValue(methodName, out HandlerAction handler);
    types.TryGetValue(methodName, out var type);
    if (type != null)
      handler?.Invoke(Convert.ChangeType(arg1, type[0]));
  }

  [MonoPInvokeCallback(typeof(Action<string, string, string>))]
  private static void HandlerCallback2(string methodName, string arg1, string arg2) {
    handlers.TryGetValue(methodName, out HandlerAction handler);
    types.TryGetValue(methodName, out var type);
    if (type != null)
      handler?.Invoke(Convert.ChangeType(arg1, type[0]), Convert.ChangeType(arg2, type[1]));
  }

  [MonoPInvokeCallback(typeof(Action<string, string, string, string>))]
  private static void HandlerCallback3(string methodName, string arg1, string arg2, string arg3) {
    handlers.TryGetValue(methodName, out HandlerAction handler);
    types.TryGetValue(methodName, out var type);
    if (type != null)
      handler?.Invoke(Convert.ChangeType(arg1, type[0]), Convert.ChangeType(arg2, type[1]),
        Convert.ChangeType(arg3, type[2]));
  }

  [MonoPInvokeCallback(typeof(Action<string, string, string, string, string>))]
  private static void HandlerCallback4(string methodName, string arg1, string arg2, string arg3, string arg4) {
    handlers.TryGetValue(methodName, out HandlerAction handler);
    types.TryGetValue(methodName, out var type);
    if (type != null)
      handler?.Invoke(Convert.ChangeType(arg1, type[0]), Convert.ChangeType(arg2, type[1]),
        Convert.ChangeType(arg3, type[2]), Convert.ChangeType(arg4, type[3]));
  }

  [MonoPInvokeCallback(typeof(Action<string, string, string, string, string, string>))]
  private static void HandlerCallback5(string methodName, string arg1, string arg2, string arg3, string arg4,
    string arg5) {
    handlers.TryGetValue(methodName, out HandlerAction handler);
    types.TryGetValue(methodName, out var type);
    if (type != null)
      handler?.Invoke(Convert.ChangeType(arg1, type[0]), Convert.ChangeType(arg2, type[1]),
        Convert.ChangeType(arg3, type[2]), Convert.ChangeType(arg4, type[3]),
        Convert.ChangeType(arg5, type[4]));
  }

  [MonoPInvokeCallback(typeof(Action<string, string, string, string, string, string, string>))]
  private static void HandlerCallback6(string methodName, string arg1, string arg2, string arg3, string arg4,
    string arg5, string arg6) {
    handlers.TryGetValue(methodName, out HandlerAction handler);
    types.TryGetValue(methodName, out var type);
    if (type != null)
      handler?.Invoke(Convert.ChangeType(arg1, type[0]), Convert.ChangeType(arg2, type[1]),
        Convert.ChangeType(arg3, type[2]), Convert.ChangeType(arg4, type[3]), Convert.ChangeType(arg5, type[4]),
        Convert.ChangeType(arg6, type[5]));
  }

  [MonoPInvokeCallback(typeof(Action<string, string, string, string, string, string, string, string>))]
  private static void HandlerCallback7(string methodName, string arg1, string arg2, string arg3, string arg4,
    string arg5, string arg6, string arg7) {
    handlers.TryGetValue(methodName, out HandlerAction handler);
    types.TryGetValue(methodName, out var type);
    if (type != null)
      handler?.Invoke(Convert.ChangeType(arg1, type[0]), Convert.ChangeType(arg2, type[1]),
        Convert.ChangeType(arg3, type[2]), Convert.ChangeType(arg4, type[3]), Convert.ChangeType(arg5, type[4]),
        Convert.ChangeType(arg6, type[5]), Convert.ChangeType(arg7, type[6]));
  }

  [MonoPInvokeCallback(typeof(Action<string, string, string, string, string, string, string, string, string>))]
  private static void HandlerCallback8(string methodName, string arg1, string arg2, string arg3, string arg4,
    string arg5, string arg6, string arg7, string arg8) {
    handlers.TryGetValue(methodName, out HandlerAction handler);
    types.TryGetValue(methodName, out var type);
    if (type != null)
      handler?.Invoke(Convert.ChangeType(arg1, type[0]), Convert.ChangeType(arg2, type[1]),
        Convert.ChangeType(arg3, type[2]), Convert.ChangeType(arg4, type[3]), Convert.ChangeType(arg5, type[4]),
        Convert.ChangeType(arg6, type[5]), Convert.ChangeType(arg7, type[6]),
        Convert.ChangeType(arg8, type[7]));
  }

  public void On<T1>(string methodName, Action<T1> handler) {
    Debug.LogError("<color=cyan>On<T1> </color>" + methodName);
    types.Add(methodName, new List<Type> { typeof(T1) });
    handlers.Add(methodName, args => handler((T1)args[0]));
    OnJs(methodName, "1", HandlerCallback1);
  }

  public void On<T1, T2>(string methodName, Action<T1, T2> handler) {
    types.Add(methodName, new List<Type> { typeof(T1), typeof(T2) });
    handlers.Add(methodName, args => handler((T1)args[0], (T2)args[1]));
    OnJs(methodName, "2", HandlerCallback2);
  }

  public void On<T1, T2, T3>(string methodName, Action<T1, T2, T3> handler) {
    types.Add(methodName, new List<Type> { typeof(T1), typeof(T2), typeof(T3) });
    handlers.Add(methodName, args => handler((T1)args[0], (T2)args[1], (T3)args[2]));
    OnJs(methodName, "3", HandlerCallback3);
  }

  public void On<T1, T2, T3, T4>(string methodName, Action<T1, T2, T3, T4> handler) {
    types.Add(methodName, new List<Type> { typeof(T1), typeof(T2), typeof(T3), typeof(T4) });
    handlers.Add(methodName, args => handler((T1)args[0], (T2)args[1], (T3)args[2], (T4)args[3]));
    OnJs(methodName, "4", HandlerCallback4);
  }

  public void On<T1, T2, T3, T4, T5>(string methodName, Action<T1, T2, T3, T4, T5> handler) {
    types.Add(methodName, new List<Type> { typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5) });
    handlers.Add(methodName, args => handler((T1)args[0], (T2)args[1], (T3)args[2], (T4)args[3], (T5)args[4]));
    OnJs(methodName, "5", HandlerCallback5);
  }

  public void On<T1, T2, T3, T4, T5, T6>(string methodName, Action<T1, T2, T3, T4, T5, T6> handler) {
    types.Add(methodName, new List<Type> { typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6) });
    handlers.Add(methodName,
      args => handler((T1)args[0], (T2)args[1], (T3)args[2], (T4)args[3], (T5)args[4], (T6)args[5]));
    OnJs(methodName, "6", HandlerCallback6);
  }

  public void On<T1, T2, T3, T4, T5, T6, T7>(string methodName, Action<T1, T2, T3, T4, T5, T6, T7> handler) {
    types.Add(methodName,
      new List<Type> { typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7) });
    handlers.Add(methodName,
      args => handler((T1)args[0], (T2)args[1], (T3)args[2], (T4)args[3], (T5)args[4], (T6)args[5], (T7)args[6]));
    OnJs(methodName, "7", HandlerCallback7);
  }

  public void On<T1, T2, T3, T4, T5, T6, T7, T8>(string methodName, Action<T1, T2, T3, T4, T5, T6, T7, T8> handler) {
    types.Add(methodName,
      new List<Type>
        { typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8) });
    handlers.Add(methodName,
      args => handler((T1)args[0], (T2)args[1], (T3)args[2], (T4)args[3], (T5)args[4], (T6)args[5], (T7)args[6],
        (T8)args[7]));
    OnJs(methodName, "8", HandlerCallback8);
  }

  #endregion
}