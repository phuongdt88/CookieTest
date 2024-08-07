var SignalRLib = {

    $vars: {
        connection: null,
        lastConnectionId: '',
        UTF8ToString: function (arg) {
            return UTF8ToString(arg);
        },
        invokeCallback: function (args, callback) { 
            var sig = 'v';
            var messages = [];
            for (var i = 0; i < args.length; i++) {
                var message = args[i];
                console.log(message);
                if (typeof message === 'object') {
                    message = JSON.stringify(message);
                }
                var bufferSize = lengthBytesUTF8(message) + 1;
                var buffer = _malloc(bufferSize);
                stringToUTF8(message, buffer, bufferSize);
                messages.push(buffer);
                sig += 'i';
            }
            if (typeof Runtime === 'undefined') {
                dynCall(sig, callback, messages);
            } else {
                Runtime.dynCall(sig, callback, messages);
            }
        },
        tryParseJson: function(str) {
            try {
                return JSON.parse(str);
            } catch (e) {
                return str;
            }
        },
    },

    InitJs: function (url, cookieParam1, cookieParam2) {
        console.log("init js");
        url = vars.UTF8ToString(url);
        console.log(url);
        var cookiesJson = UTF8ToString(cookieParam1);
        if (cookiesJson) {
            console.log('cookies before parse');
            console.log(cookiesJson);
            var cookies = JSON.parse(cookiesJson);
            console.log('cookies after parse: ' + cookies.length);
            console.log(cookies);
            cookies.forEach(function(cookie) {
                console.log('cookies loop');
                console.log(cookie);
                var cookieString = cookie.Name + "=" + cookie.Value + "; path=" + cookie.Path;
                if (cookie.Domain) {
                    cookieString += "; domain=" + cookie.Domain;
                }
                if (cookie.Secure) {
                    cookieString += "; secure";
                }
                console.log('cookiezzz: ' + cookieString);
                document.cookie = cookieString;
            });
        }

        vars.connection = new signalR.HubConnectionBuilder()
            .withUrl(url, {
                withCredentials: true
            })
            .withHubProtocol(new signalR.protocols.msgpack.MessagePackHubProtocol())
            .withAutomaticReconnect()
            .build();
    },

    ConnectJs: function (connectedCallback, disconnectedCallback, reconnectingCallback, reconnectedCallback) {
        vars.connection.start()
            .then(function () {
                vars.lastConnectionId = vars.connection.connectionId;
                vars.connection.onclose(function (err) {
                    if (err) {
                        console.error('Connection closed due to error: "' + err.toString() + '".');
                    }
                    vars.invokeCallback([err.toString()], disconnectedCallback);
                });
                vars.connection.onreconnecting(function (err) {
                    console.log('Connection lost due to error: "' + err.toString() + '". Reconnecting.');
                    vars.invokeCallback([err.toString()], reconnectingCallback);
                });
                vars.connection.onreconnected(function (connectionId) {
                    console.log('Connection reestablished. Connected with connectionId: "' + connectionId + '".');
                    vars.lastConnectionId = connectionId;
                    vars.invokeCallback([vars.lastConnectionId], reconnectedCallback);
                });
                vars.invokeCallback([vars.lastConnectionId], connectedCallback);
            }).catch(function (err) {
                return console.error(err.toString());
            });
    },

    StopJs: function () {
        if (vars.connection) {
            vars.connection.stop()
                .then(function () {
                    console.log('connection stopped');
                })
                .catch(function (err) {
                    return console.error(err.toString());
                });
        }
    },

    InvokeJs: function (methodName, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, callback) {
        methodName = vars.UTF8ToString(methodName);
        if (arg1 && arg2 && arg3 && arg4 && arg5 && arg6 && arg7 && arg8 && arg9 && arg10) {
            arg1 = vars.UTF8ToString(arg1);
            arg2 = vars.UTF8ToString(arg2);
            arg3 = vars.UTF8ToString(arg3);
            arg4 = vars.UTF8ToString(arg4);
            arg5 = vars.UTF8ToString(arg5);
            arg6 = vars.UTF8ToString(arg6);
            arg7 = vars.UTF8ToString(arg7);
            arg8 = vars.UTF8ToString(arg8);
            arg9 = vars.UTF8ToString(arg9);
            arg10 = vars.UTF8ToString(arg10);
            vars.connection.invoke(methodName, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10)
                .catch(function (err) {
                    return console.error(err.toString());
                });
        } else if (arg1 && arg2 && arg3 && arg4 && arg5 && arg6 && arg7 && arg8 && arg9) {
            arg1 = vars.UTF8ToString(arg1);
            arg2 = vars.UTF8ToString(arg2);
            arg3 = vars.UTF8ToString(arg3);
            arg4 = vars.UTF8ToString(arg4);
            arg5 = vars.UTF8ToString(arg5);
            arg6 = vars.UTF8ToString(arg6);
            arg7 = vars.UTF8ToString(arg7);
            arg8 = vars.UTF8ToString(arg8);
            arg9 = vars.UTF8ToString(arg9);
            vars.connection.invoke(methodName, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9)
                .catch(function (err) {
                    return console.error(err.toString());
                });
        } else if (arg1 && arg2 && arg3 && arg4 && arg5 && arg6 && arg7 && arg8) {
            arg1 = vars.UTF8ToString(arg1);
            arg2 = vars.UTF8ToString(arg2);
            arg3 = vars.UTF8ToString(arg3);
            arg4 = vars.UTF8ToString(arg4);
            arg5 = vars.UTF8ToString(arg5);
            arg6 = vars.UTF8ToString(arg6);
            arg7 = vars.UTF8ToString(arg7);
            arg8 = vars.UTF8ToString(arg8);
            vars.connection.invoke(methodName, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8)
                .catch(function (err) {
                    return console.error(err.toString());
                });
        } else if (arg1 && arg2 && arg3 && arg4 && arg5 && arg6 && arg7) {
            arg1 = vars.UTF8ToString(arg1);
            arg2 = vars.UTF8ToString(arg2);
            arg3 = vars.UTF8ToString(arg3);
            arg4 = vars.UTF8ToString(arg4);
            arg5 = vars.UTF8ToString(arg5);
            arg6 = vars.UTF8ToString(arg6);
            arg7 = vars.UTF8ToString(arg7);
            vars.connection.invoke(methodName, arg1, arg2, arg3, arg4, arg5, arg6, arg7)
                .catch(function (err) {
                    return console.error(err.toString());
                });
        } else if (arg1 && arg2 && arg3 && arg4 && arg5 && arg6) {
            arg1 = vars.UTF8ToString(arg1);
            arg2 = vars.UTF8ToString(arg2);
            arg3 = vars.UTF8ToString(arg3);
            arg4 = vars.UTF8ToString(arg4);
            arg5 = vars.UTF8ToString(arg5);
            arg6 = vars.UTF8ToString(arg6);
            vars.connection.invoke(methodName, arg1, arg2, arg3, arg4, arg5, arg6)
                .catch(function (err) {
                    return console.error(err.toString());
                });
        } else if (arg1 && arg2 && arg3 && arg4 && arg5) {
            arg1 = vars.UTF8ToString(arg1);
            arg2 = vars.UTF8ToString(arg2);
            arg3 = vars.UTF8ToString(arg3);
            arg4 = vars.UTF8ToString(arg4);
            arg5 = vars.UTF8ToString(arg5);
            vars.connection.invoke(methodName, arg1, arg2, arg3, arg4, arg5)
                .catch(function (err) {
                    return console.error(err.toString());
                });
        } else if (arg1 && arg2 && arg3 && arg4) {
            arg1 = vars.UTF8ToString(arg1);
            arg2 = vars.UTF8ToString(arg2);
            arg3 = vars.UTF8ToString(arg3);
            arg4 = vars.UTF8ToString(arg4);
            vars.connection.invoke(methodName, arg1, arg2, arg3, arg4)
                .catch(function (err) {
                    return console.error(err.toString());
                });
        } else if (arg1 && arg2 && arg3) {
            arg1 = vars.UTF8ToString(arg1);
            arg2 = vars.UTF8ToString(arg2);
            arg3 = vars.UTF8ToString(arg3);
            arg1 = vars.tryParseJson(arg1);
            console.log(methodName);
            console.log(arg1);
            console.log(arg2);
            console.log(arg3);
            vars.connection.invoke(methodName, arg1, arg2, arg3)
                .catch(function (err) {
                    return console.error(err.toString());
                });
        } else if (arg1 && arg2) {
            arg1 = vars.UTF8ToString(arg1);
            arg2 = vars.UTF8ToString(arg2);
            vars.connection.invoke(methodName, arg1, arg2)
                .catch(function (err) {
                    return console.error(err.toString());
                });
        } else if (arg1) {
            arg1 = vars.UTF8ToString(arg1);
            var data = vars.tryParseJson(arg1);
            vars.connection.invoke(methodName, data)
                .catch(function (err) {
                    return console.error(err.toString());
                });
        } else {
            vars.connection.invoke(methodName)
                .then(function (result) {
                    vars.invokeCallback([methodName, result], callback);
                })
                .catch(function (err) {
                    return console.error(err.toString());
                });
        }
    },

    OnJs: function (methodName, argCount, callback) {
        methodName = vars.UTF8ToString(methodName);
        argCount = Number.parseInt(vars.UTF8ToString(argCount));
        if (argCount === 1) {
            // vars.handlerCallback1 = callback;
            vars.connection.on(methodName, function (arg1) {
                vars.invokeCallback([methodName, arg1], callback);
            });
        } else if (argCount === 2) {
            // vars.handlerCallback2 = callback;
            vars.connection.on(methodName, function (arg1, arg2) {
                vars.invokeCallback([methodName, arg1, arg2], callback);
            });
        } else if (argCount === 3) {
            // vars.handlerCallback3 = callback;
            vars.connection.on(methodName, function (arg1, arg2, arg3) {
                vars.invokeCallback([methodName, arg1, arg2, arg3], callback);
            });
        } else if (argCount === 4) {
            // vars.handlerCallback4 = callback;
            vars.connection.on(methodName, function (arg1, arg2, arg3, arg4) {
                vars.invokeCallback([methodName, arg1, arg2, arg3, arg4], callback);
            });
        } else if (argCount === 5) {
            // vars.handlerCallback5 = callback;
            vars.connection.on(methodName, function (arg1, arg2, arg3, arg4, arg5) {
                vars.invokeCallback([methodName, arg1, arg2, arg3, arg4, arg5], callback);
            });
        } else if (argCount === 6) {
            // vars.handlerCallback6 = callback;
            vars.connection.on(methodName, function (arg1, arg2, arg3, arg4, arg5, arg6) {
                vars.invokeCallback([methodName, arg1, arg2, arg3, arg4, arg5, arg6], callback);
            });
        } else if (argCount === 7) {
            // vars.handlerCallback7 = callback;
            vars.connection.on(methodName, function (arg1, arg2, arg3, arg4, arg5, arg6, arg7) {
                vars.invokeCallback([methodName, arg1, arg2, arg3, arg4, arg5, arg6, arg7], callback);
            });
        } else if (argCount === 8) {
            // vars.handlerCallback8 = callback;
            vars.connection.on(methodName, function (arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8) {
                vars.invokeCallback([methodName, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8], callback);
            });
        }
    },
};

autoAddDeps(SignalRLib, '$vars');
mergeInto(LibraryManager.library, SignalRLib);
