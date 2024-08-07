using System;
using System.Collections.Generic;
using System.Net;
using Microsoft.AspNetCore.SignalR.Client;
using UniRx.Async;
using UnityEngine;
using System.Threading.Tasks;
using GearMessagePack.Resolvers;
using MessagePack;
using MessagePack.Resolvers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;


/// <summary>
/// Manages WebSocket connections for Unity applications using SignalR.
/// </summary>
/// <remarks>
/// This class handles tasks such as connection initialization, maintaining connection state, handling connection events, and disconnection.
/// </remarks>
public class GearSocketManagerV2 : MonoBehaviour {
  public event Action<bool> UpdateSpinningEvent;
  public bool IsShowingSpinning { get; set; }
  protected HubConnection HubConnection { get; set; }
  protected ConnectionState CurrentState = ConnectionState.Invalid;
  protected NetworkReachability NetworkReachability = NetworkReachability.ReachableViaLocalAreaNetwork;

  protected bool IsResolverRegistered;

  protected virtual string GetAlias() => "socket manager v2";
  
  protected List<Cookie> _cookies;

  public virtual async void Init() {
    if (IsSocketConnectionCreated()) {
      return;
    }

    CurrentState = ConnectionState.Invalid;
    NetworkReachability = NetworkReachability.ReachableViaLocalAreaNetwork;
    await ConnectSocket();
  }

  protected virtual void Update() {
    // if (SceneFlowManager.Instance.InFrontEnd()) {
      switch (CurrentState) {
        case ConnectionState.Connected:
          // Check for disconnect, change network
          NetworkReachability internetReachability = Application.internetReachability;
          if (internetReachability == NetworkReachability.NotReachable) {
            SetConnectState(ConnectionState.Disconnected);
          }
          else if (internetReachability != NetworkReachability) {
            // Network changed, restart connection
            SetConnectState(ConnectionState.NetworkChanged);
          }

          break;

        case ConnectionState.Disconnected:
        case ConnectionState.SocketError:
          if (Application.internetReachability != NetworkReachability.NotReachable) {
            SetConnectState(ConnectionState.Connect);
          }

          break;
      }
    // }
  }

  private void OnApplicationPause(bool pauseStatus) {
    bool isPaused = pauseStatus;
    if (!isPaused && CurrentState == ConnectionState.Connected) {
      OnAppFocused();
    }
  }

  protected virtual void OnAppFocused() { }

  public virtual async void SetConnectState(ConnectionState state) {
    if (CurrentState == state) {
      return;
    }

    CurrentState = state;
    switch (CurrentState) {
      case ConnectionState.Disconnected:
      case ConnectionState.Connected:
      case ConnectionState.SocketError:
        HideSpinning();
        break;

      case ConnectionState.Connect:
        ShowSpinning();
        await UniTask.Delay(GetReconnectMillisecondsDelay());
        await ConnectSocket();
        break;

      case ConnectionState.NetworkChanged:
        await RestartConnect();
        ShowSpinning();
        break;
    }
  }

  protected virtual bool ShouldIncludeCookiesInHeader() {
    return false;
  }

  protected virtual CookieContainer GetRequestCookies() {
    return null;
  }

  protected virtual string GetCookieString() {
    return string.Empty;
  }

  public virtual async UniTask<bool> ConnectSocket() {
    TFUtils.MyLogError(new object[] { $"{GetAlias()} ___ ConnectSocket", HubConnection }, go: null, color: "magenta");
//     if (!IsSocketConnectionCreated()) {
//       string address = GetAddress();
//       Debug.Log($"{GetAlias()} ___ ConnectSocket: address: {address}");
//       if (string.IsNullOrEmpty(address)) {
//         Debug.Log($"{GetAlias()} ___ ConnectSocket: address is empty -> stop connecting");
//         return false;
//       }
//       
//       HubConnection = new HubConnectionBuilder()
//         .WithUrl(address, options => {
//           var cookieContainer = GetRequestCookies();
//           if (cookieContainer != null && ShouldIncludeCookiesInHeader()) {
//             options.Cookies = cookieContainer;
//           }
//         })
//         .AddGearMessagePackProtocol(options =>
//         {
//           if (!IsResolverRegistered) {
//             StaticCompositeResolver.Instance.Register(
//               GeneratedResolver.Instance,
//               DynamicGenericResolver.Instance,
//               MessagePack.Unity.Extension.UnityBlitWithPrimitiveArrayResolver.Instance,
//               MessagePack.Unity.UnityResolver.Instance,
//               StandardResolver.Instance
//             );
//           }
//           options.SerializerOptions = MessagePackSerializerOptions.Standard
//             .WithSecurity(MessagePackSecurity.UntrustedData)
//             .WithCompression(MessagePackCompression.Lz4BlockArray)
//             .WithResolver(StaticCompositeResolver.Instance);
//             
//           IsResolverRegistered = true;
//         })
//         // .AddNewtonsoftJsonProtocol()
//         .WithAutomaticReconnect()
// #if !KFF_RELEASE
//         // .ConfigureLogging(logging => {
//         //   logging.AddProvider(new UnityLoggerProvider());
//         //   logging.SetMinimumLevel(LogLevel.Trace);
//         // })
// #endif
//         .Build();
//
//
//       ConfigureHubConnectionSettings();
//       // Subscribe to the WS events
//       HubConnection.Closed += OnClosed;
//       HubConnection.Reconnecting += OnReconnecting;
//       HubConnection.Reconnected += OnReconnected;
//       RegisterSocketEvent();
//     }
//
//     NetworkReachability = Application.internetReachability;
//     try {
//       ShowSpinning();
//       await HubConnection.StartAsync();
//       await OnConnected();
//     }
//     catch (Exception e) {
//       OnError(HubConnection, "ConnectSocket Exception: " + e.Message);
//     }

    return true;
  }

  protected virtual void ConfigureHubConnectionSettings() {
    HubConnection.ServerTimeout = TimeSpan.FromSeconds(30);
    HubConnection.KeepAliveInterval = TimeSpan.FromSeconds(15);
    HubConnection.HandshakeTimeout = TimeSpan.FromSeconds(15);
  }

  protected virtual string GetAddress() => "";

  protected virtual void RegisterSocketEvent() { }

  protected virtual Task OnConnected() {
    TFUtils.MyLogError(new object[] { $"{GetAlias()} ___ OnConnected", HubConnection }, go: null, color: "magenta");
    SetConnectState(ConnectionState.Connected);
    return Task.CompletedTask;
  }

  protected virtual Task OnClosed(Exception ex) {
    TFUtils.MyLogError(new object[] { $"{GetAlias()} ___ OnClosed", ex }, go: null, color: "magenta");
    return Task.CompletedTask;
  }

  protected virtual Task OnReconnecting(Exception ex) {
    TFUtils.MyLogError(new object[] { $"{GetAlias()} ___ OnReconnecting", ex }, go: null, color: "magenta");
    return Task.CompletedTask;
  }

  protected virtual Task OnReconnected(string newConnectionId) {
    TFUtils.MyLogError(new object[] { $"{GetAlias()} ___ OnReconnected", newConnectionId }, go: null, color: "magenta");
    return Task.CompletedTask;
  }

  protected virtual void OnError(HubConnection connection, string error) {
    TFUtils.MyLogError(new object[] { $"{GetAlias()} ___ OnError", error }, go: null, color: "magenta");
    DisposeHubConnection();
    HideSpinning();
  }

  protected virtual void ShowSpinning() {
    if (!IsShowingSpinning) {
      IsShowingSpinning = true;
      UpdateSpinningEvent?.Invoke(true);
    }
  }

  protected virtual void HideSpinning() {
    if (IsShowingSpinning) {
      IsShowingSpinning = false;
      UpdateSpinningEvent?.Invoke(false);
    }
  }

  protected virtual string ChangeHost(string _address, string _newHost, int _port, bool _isHttps = false) {
    var builder = new UriBuilder(_address) { Host = _newHost, Port = _port };
    return _isHttps ? builder.ToString() : builder.ToString().Replace("https", "http");
  }

  public virtual bool IsConnectionAvailable() => IsSocketConnectionCreated() && HubConnection.State == HubConnectionState.Connected;

  public virtual async UniTask<bool> CloseSocket() {
    if (IsConnectionAvailable()) {
      await HubConnection.StopAsync();
    }

    DisposeHubConnection();
    HideSpinning();
    CurrentState = ConnectionState.Invalid;
    TFUtils.MyLogError(new object[] { $"{GetAlias()} ___ CloseSocket", HubConnection }, go: null, color: "magenta");
    return true;
  }

  public virtual async UniTask<bool> RestartConnect() {
    await CloseSocket();
    await UniTask.Delay(GetReconnectMillisecondsDelay());
    if (!IsSocketConnectionCreated()) {
      await ConnectSocket();
    }

    TFUtils.MyLogError(new object[] { $"{GetAlias()} ___ RestartConnect", null, Time.realtimeSinceStartup }, go: null,
      color: "magenta");
    return true;
  }

  public virtual async void ToggleConnectManually() {
    if (!IsSocketConnectionCreated()) {
      await ConnectSocket();
    }
    else {
      await CloseSocket();
    }
  }

  protected virtual int GetReconnectMillisecondsDelay() {
    return 2000;
  }

  public virtual void DisposeHubConnection() {
    if (IsSocketConnectionCreated()) {
      HubConnection.Closed -= OnClosed;
      HubConnection.Reconnecting -= OnReconnecting;
      HubConnection.Reconnected -= OnReconnected;
      UnRegisterSocketEvent();
    }

    HubConnection = null;
  }

  protected virtual void UnRegisterSocketEvent() { }

  public enum ConnectionState {
    Invalid,
    Connect,
    Connected,
    Disconnected,
    NetworkChanged,
    SocketError
  }

  protected virtual bool IsSocketConnectionCreated() {
    return HubConnection != null;
  }
}