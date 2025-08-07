using System.Runtime.InteropServices;
using UnityEngine;

public class JSFunctCalls : MonoBehaviour
{
  [DllImport("__Internal")] private static extern void SendPostMessage(string message);

  internal void SendCustomMessage(string message)
  {
#if UNITY_WEBGL && !UNITY_EDITOR
    SendPostMessage(message);
#endif
  }
}
