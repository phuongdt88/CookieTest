// #define ASSERTS_ON
// #if UNITY_EDITOR || DEVELOPMENT_BUILD
// #define DEBUG
// #endif

using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using System.Text;
using System.Globalization;
using System.Threading.Tasks;
using UniRx.Async;
using UnityEngine.UI;

public partial class TFUtils {
  
  // sboily: changed this as it was causing issues with alliance help
  public static void MyLogError(object[] objs, bool shouldBreak = false, GameObject go = null, string color = null) {

#if DISABLE_LOG
      return;
#endif

#if KFF_RELEASE
      return;
#endif

    string s = "";
    for (int i = 0; i < objs.Length; i++) {
      if (objs[i] != null) {
        s += objs[i].ToString() + " _ ";
      }
      else {
        s += "null" + " _ ";
      }
    }

    if (color != null) {
      Debug.LogError("<color=" + color + ">" + s + "</color>", go);
    }
    else {
      Debug.LogError("<color=magenta>" + s + "</color>", go);
    }

    if (shouldBreak) {
      Debug.Break();
    }
  }
}
  
