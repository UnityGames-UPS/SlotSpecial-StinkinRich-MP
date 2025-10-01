using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Newtonsoft.Json;
using Best.SocketIO;
using Best.SocketIO.Events;
using Newtonsoft.Json.Linq;
using System.Runtime.Serialization;

public class SocketIOManager : MonoBehaviour
{
  [SerializeField]
  private SlotBehaviour slotManager;

  [SerializeField]
  private UIManager uiManager;
  [SerializeField] internal JSFunctCalls JSManager;
  private Socket gameSocket;
  protected string nameSpace = "playground";
  internal GameData initialData = null;
  internal UiData initUIData = null;
  internal GameData resultData = null;
  internal Root ResultData = null;
  internal Player playerdata = null;
  internal List<List<int>> LineData = null;
  //WebSocket currentSocket = null;
  internal bool isResultdone = false;
  internal BonusData tempBonus;

  private SocketManager manager;

  protected string SocketURI = null;
  //protected string TestSocketURI = "https://game-crm-rtp-backend.onrender.com/";
  //protected string TestSocketURI = "https://7p68wzhv-5000.inc1.devtunnels.ms/";
  //protected string TestSocketURI = "https://6f01c04j-5000.inc1.devtunnels.ms/";
  //protected string TestSocketURI = "https://c4xfw9cd-5002.inc1.devtunnels.ms/";
  protected string TestSocketURI = "http://localhost:5000/";

  [SerializeField]
  private string testToken;

  protected string gameID = "SL-SR";
  // protected string gameID = "";

  internal bool isLoaded = false;

  internal bool SetInit = false;

  private const int maxReconnectionAttempts = 6;
  private readonly TimeSpan reconnectionDelay = TimeSpan.FromSeconds(10);

  private bool isConnected = false; //Back2 Start
  private bool hasEverConnected = false;
  private const int MaxReconnectAttempts = 5;
  private const float ReconnectDelaySeconds = 2f;

  private float lastPongTime = 0f;
  private float pingInterval = 2f;
  private float pongTimeout = 3f;
  private bool waitingForPong = false;
  private int missedPongs = 0;
  private const int MaxMissedPongs = 5;
  private Coroutine PingRoutine; //Back2 end
  [SerializeField] private GameObject RaycastBlocker;
  internal int[,] Winmatrix = new int[5, 5]
   {
        { 8, 9, 10, 8, 6 },
        { 6, 9, 9, 9, 8 },
        { 7, 9, 12, 12, 12 },
        { 6, 9, 9, 9, 6 },
        { 7, 9, 10, 9, 7 }
   };


  private void Awake()
  {
    //Debug.unityLogger.logEnabled = false;
    isLoaded = false;
    SetInit = false;

  }

  private void Start()
  {
    //OpenWebsocket();
    OpenSocket();
  }

  void ReceiveAuthToken(string jsonData)
  {
    Debug.Log("Received data: " + jsonData);

    // Parse the JSON data
    var data = JsonUtility.FromJson<AuthTokenData>(jsonData);
    SocketURI = data.socketURL;
    myAuth = data.cookie;
    nameSpace = data.nameSpace;

    // Proceed with connecting to the server using myAuth and socketURL
  }

  string myAuth = null;
  private bool exited;

  private void OpenSocket()
  {
    // Create and setup SocketOptions
    SocketOptions options = new SocketOptions(); //Back2 Start
    options.AutoConnect = false;
    options.Reconnection = false;
    options.Timeout = TimeSpan.FromSeconds(3);
    options.ConnectWith = Best.SocketIO.Transports.TransportTypes.WebSocket;

#if UNITY_WEBGL && !UNITY_EDITOR
        JSManager.SendCustomMessage("authToken");
        StartCoroutine(WaitForAuthToken(options));
#else
    Func<SocketManager, Socket, object> authFunction = (manager, socket) =>
    {
      return new
      {
        token = testToken,
        gameId = gameID
      };
    };
    options.Auth = authFunction;
    // Proceed with connecting to the server
    SetupSocketManager(options);
#endif
  }



  private IEnumerator WaitForAuthToken(SocketOptions options)
  {
    // Wait until myAuth is not null
    while (myAuth == null)
    {
      Debug.Log("My Auth is null");
      yield return null;
    }
    while (SocketURI == null)
    {
      Debug.Log("My Socket is null");
      yield return null;
    }
    Debug.Log("My Auth is not null");
    // Once myAuth is set, configure the authFunction
    Func<SocketManager, Socket, object> authFunction = (manager, socket) =>
    {
      return new
      {
        token = myAuth,
        //  gameId = gameID
      };
    };
    options.Auth = authFunction;

    Debug.Log("Auth function configured with token: " + myAuth);

    // Proceed with connecting to the server
    SetupSocketManager(options);
  }

  private void SetupSocketManager(SocketOptions options)
  {
    // Create and setup SocketManager
#if UNITY_EDITOR
    this.manager = new SocketManager(new Uri(TestSocketURI), options);
#else
        this.manager = new SocketManager(new Uri(SocketURI), options);
#endif

    if (string.IsNullOrEmpty(nameSpace))
    {  //BackendChanges Start
      gameSocket = this.manager.Socket;
    }
    else
    {
      print("nameSpace: " + nameSpace);
      gameSocket = this.manager.GetSocket("/" + nameSpace);
    }
    // Set subscriptions
    gameSocket.On<ConnectResponse>(SocketIOEventTypes.Connect, OnConnected);
    gameSocket.On(SocketIOEventTypes.Disconnect, OnDisconnected); //Back2 Start
    gameSocket.On<Error>(SocketIOEventTypes.Error, OnError);
    // gameSocket.On<string>("message", OnListenEvent);
    gameSocket.On<string>("game:init", OnListenEvent);
    gameSocket.On<string>("result", OnListenEvent);

    gameSocket.On<bool>("socketState", OnSocketState);
    gameSocket.On<string>("internalError", OnSocketError);
    gameSocket.On<string>("alert", OnSocketAlert);
    gameSocket.On<string>("pong", OnPongReceived);
    gameSocket.On<string>("AnotherDevice", OnSocketOtherDevice);
    manager.Open();
  }

  // Connected event handler implementation
  void OnConnected(ConnectResponse resp) //Back2 Start
  {
    Debug.Log("✅ Connected to server.");

    if (hasEverConnected)
    {
      uiManager.CheckAndClosePopups();
    }

    isConnected = true;
    hasEverConnected = true;
    waitingForPong = false;
    missedPongs = 0;
    lastPongTime = Time.time;
    SendPing();
  } //Back2 end

  private void OnDisconnected() //Back2 Start
  {
    Debug.LogWarning("⚠️ Disconnected from server.");
    isConnected = false;
    uiManager.DisconnectionPopup();
    ResetPingRoutine();
  } //Back2 end

  private void OnPongReceived(string data) //Back2 Start
  {
    Debug.Log("✅ Received pong from server.");
    waitingForPong = false;
    missedPongs = 0;
    lastPongTime = Time.time;
    Debug.Log($"⏱️ Updated last pong time: {lastPongTime}");
    Debug.Log($"📦 Pong payload: {data}");
  } //Back2 end


  private void OnError(Error err)
  {
    Debug.LogError("Error: " + err);
#if UNITY_WEBGL && !UNITY_EDITOR
    Debug.Log("Called Send Custom Message");
    JSManager.SendCustomMessage("error");
#endif
  }
  void CloseGame()
  {
    Debug.Log("Unity: Closing Game");
    StartCoroutine(CloseSocket());
  }
  private void OnListenEvent(string data)
  {
    Debug.Log("Received some_event with data: " + data);
    ParseResponse(data);
  }

  private void OnSocketState(bool state)
  {
    if (state)
    {
      Debug.Log("my state is " + state);
    }
    else
    {

    }
  }
  private void OnSocketError(string data)
  {
    Debug.Log("Received error with data: " + data);
  }
  private void OnSocketAlert(string data)
  {
    Debug.Log("Received alert with data: " + data);
  }

  private void OnSocketOtherDevice(string data)
  {
    Debug.Log("Received Device Error with data: " + data);
    uiManager.ADfunction();
  }

  private void SendPing() //Back2 Start
  {
    ResetPingRoutine();
    PingRoutine = StartCoroutine(PingCheck());
  }

  void ResetPingRoutine()
  {
    if (PingRoutine != null)
    {
      StopCoroutine(PingRoutine);
    }
    PingRoutine = null;
  }

  private IEnumerator PingCheck()
  {
    while (true)
    {
      Debug.Log($"🟡 PingCheck | waitingForPong: {waitingForPong}, missedPongs: {missedPongs}, timeSinceLastPong: {Time.time - lastPongTime}");

      if (missedPongs == 0)
      {
        uiManager.CheckAndClosePopups();
      }

      // If waiting for pong, and timeout passed
      if (waitingForPong)
      {
        if (missedPongs == 2)
        {
          uiManager.ReconnectionPopup();
        }
        missedPongs++;
        Debug.LogWarning($"⚠️ Pong missed #{missedPongs}/{MaxMissedPongs}");

        if (missedPongs >= MaxMissedPongs)
        {
          Debug.LogError("❌ Unable to connect to server — 5 consecutive pongs missed.");
          isConnected = false;
          uiManager.DisconnectionPopup();
          yield break;
        }
      }

      // Send next ping
      waitingForPong = true;
      lastPongTime = Time.time;
      Debug.Log("📤 Sending ping...");
      SendDataWithNamespace("ping");
      yield return new WaitForSeconds(pingInterval);
    }
  }

  private void AliveRequest()
  {
    SendDataWithNamespace("YES I AM ALIVE");
  }

  private void SendDataWithNamespace(string eventName, string json = null)
  {
    // Send the message
    if (gameSocket != null && gameSocket.IsOpen)
    {
      if (json != null)
      {
        gameSocket.Emit(eventName, json);
        Debug.Log("JSON data sent: " + json);
      }
      else
      {
        gameSocket.Emit(eventName);
      }
    }
    else
    {
      Debug.LogWarning("Socket is not connected.");
    }
  }



  internal IEnumerator CloseSocket() //Back2 Start
  {
    RaycastBlocker.SetActive(true);
    ResetPingRoutine();

    Debug.Log("Closing Socket");

    manager?.Close();
    manager = null;

    Debug.Log("Waiting for socket to close");

    yield return new WaitForSeconds(0.5f);

    Debug.Log("Socket Closed");

#if UNITY_WEBGL && !UNITY_EDITOR
    JSManager.SendCustomMessage("OnExit"); //Telling the react platform user wants to quit and go back to homepage
#endif
  } //Back2 end
  IEnumerator WaitAndExit()
  {
    yield return new WaitForSeconds(2f);
    if (!exited)
    {
      exited = true;
#if UNITY_WEBGL && !UNITY_EDITOR
      JSManager.SendCustomMessage("onExit");
#endif
    }
  }

  private void ParseResponse(string jsonObject)
  {
    Debug.Log(jsonObject);
    Root myData = JsonConvert.DeserializeObject<Root>(jsonObject);

    string id = myData.id;

    switch (id)
    {
      case "initData":
        {
          initialData = myData.gameData;
          initUIData = myData.uiData;
          playerdata = myData.player;
          LineData = myData.gameData.lines;
          if (!SetInit)
          {
            Debug.Log(jsonObject);
            List<string> LinesString = ConvertListListIntToListString(initialData.lines);
            PopulateSlotSocket(LinesString);
            SetInit = true;
          }
          else
          {
            RefreshUI();
          }
          break;
        }
      case "ResultData":
        {
          Debug.Log(jsonObject);
          ResultData = myData;
          // myData.message.GameData.FinalResultReel = ConvertListOfListsToStrings(myData.message.GameData.ResultReel);
          // myData.message.GameData.FinalsymbolsToEmit = TransformAndRemoveRecurring(myData.message.GameData.symbolsToEmit);
          // resultData = myData.message.GameData;
          // playerdata = myData.player;
          isResultdone = true;
          // tempBonus = myData.message.GameData.bonusData;
          break;
        }
      case "ExitUser":
        {
          gameSocket.Disconnect();
          if (this.manager != null)
          {
            Debug.Log("Dispose my Socket");
            this.manager.Close();
          }
#if UNITY_WEBGL && !UNITY_EDITOR
          JSManager.SendCustomMessage("onExit");
#endif
          exited = true;
          break;
        }
    }
  }

  private void RefreshUI()
  {
    uiManager.InitialiseUIData(initUIData.paylines);
  }

  private void PopulateSlotSocket(List<string> LineIds)
  {
    slotManager.shuffleInitialMatrix();
    for (int i = 0; i < LineIds.Count; i++)
    {
      slotManager.FetchLines(LineIds[i], i);
    }

    slotManager.SetInitialUI();

    isLoaded = true;
    RaycastBlocker.SetActive(false);
#if UNITY_WEBGL && !UNITY_EDITOR
    JSManager.SendCustomMessage("OnEnter");
#endif
  }

  internal void AccumulateResult(int currBet)
  {
    isResultdone = false;
    MessageData message = new MessageData();
    message.type = "SPIN";
    Debug.Log($"current bet is " + currBet);
    message.payload = new Data();
    message.payload.betIndex = currBet;
    string json = JsonUtility.ToJson(message);
    SendDataWithNamespace("request", json);
  }

  private List<string> RemoveQuotes(List<string> stringList)
  {
    for (int i = 0; i < stringList.Count; i++)
    {
      stringList[i] = stringList[i].Replace("\"", ""); // Remove inverted commas
    }
    return stringList;
  }

  private List<string> ConvertListListIntToListString(List<List<int>> listOfLists)
  {
    List<string> resultList = new List<string>();

    foreach (List<int> innerList in listOfLists)
    {
      // Convert each integer in the inner list to string
      List<string> stringList = new List<string>();
      foreach (int number in innerList)
      {
        stringList.Add(number.ToString());
      }

      // Join the string representation of integers with ","
      string joinedString = string.Join(",", stringList.ToArray()).Trim();
      resultList.Add(joinedString);
    }

    return resultList;
  }

  private List<string> ConvertListOfListsToStrings(List<List<string>> inputList)
  {
    List<string> outputList = new List<string>();

    foreach (List<string> row in inputList)
    {
      string concatenatedString = string.Join(",", row);
      outputList.Add(concatenatedString);
    }

    return outputList;
  }

  private List<string> TransformAndRemoveRecurring(List<List<string>> originalList)
  {
    // Flattened list
    List<string> flattenedList = new List<string>();
    foreach (List<string> sublist in originalList)
    {
      flattenedList.AddRange(sublist);
    }

    // Remove recurring elements
    HashSet<string> uniqueElements = new HashSet<string>(flattenedList);

    // Transformed list
    List<string> transformedList = new List<string>();
    foreach (string element in uniqueElements)
    {
      transformedList.Add(element.Replace(",", ""));
    }

    return transformedList;
  }
}

[Serializable]
public class BetData
{
  public double currentBet;
  public double currentLines;
  public double spins;
}

[Serializable]
public class AuthData
{
  public string GameID;
  //public double TotalLines;
}

[Serializable]
public class MessageData
{
  // public int option;
  // public List<int> index;
  public string type;
  public Data payload;

}
[Serializable]
public class Data
{
  public int betIndex;
  //   public string Event;
  //   public List<int> index;
  //   public int option;

}

[Serializable]
public class ExitData
{
  public string id;
}

[Serializable]
public class InitData
{
  public AuthData Data;
  public string id;
}

[Serializable]
public class GameData
{
  //public List<List<int>> lines { get; set; }
  //public List<double> bets { get; set; }
  public List<List<string>> ResultReel { get; set; }
  public List<int> linesToEmit { get; set; }
  public List<List<string>> symbolsToEmit { get; set; }
  public double WinAmout { get; set; }
  public List<string> FinalsymbolsToEmit { get; set; }
  public List<string> FinalResultReel { get; set; }
  public FreeSpin freeSpin { get; set; }
  public BonusData bonusData { get; set; }
  public List<string> scatterWinningSymbols { get; set; }

  public List<List<int>> lines { get; set; }
  public List<double> bets { get; set; }
}

[Serializable]
public class BonusData
{
  public bool isBonus { get; set; }
  public double bonusWin { get; set; }
  public List<double> shuffledBonusValues { get; set; }
  public int selectedBonusMultiplier { get; set; }
  public List<string> trashForCashWinningSymbols { get; set; }
}

[Serializable]
public class FreeSpin
{
  // public bool isNewAdded { get; set; }
  // public int freeSpinCount { get; set; }

  public bool isFreeSpin { get; set; }
  public int count { get; set; }
}

[Serializable]
public class Message
{
  public GameData GameData { get; set; }
  public UiData UIData { get; set; }
  public Player PlayerData { get; set; }
}

[Serializable]
public class Root
{
  public string id { get; set; }
  public Message message { get; set; }

  public GameData gameData { get; set; }
  public UiData uiData { get; set; }
  public Player player { get; set; }
  public List<List<string>> matrix { get; set; }
  public Payload payload { get; set; }
  public Features features { get; set; }
  public bool success { get; set; }
}

[Serializable]
public class UiData
{
  public Paylines paylines { get; set; }
  public List<string> spclSymbolTxt { get; set; }
}
[Serializable]
public class Features
{
  public Scatter scatter { get; set; }
  public Bonus bonus { get; set; }
  public FreeSpin freeSpin { get; set; }
  public double totalWinAmount { get; set; }
}

[Serializable]
public class Scatter
{
  public bool enabled { get; set; }
  public int scatterCount { get; set; }
  public int scatterMultiplier { get; set; }
  public double amount { get; set; }
}
[Serializable]
public class Bonus
{
  public bool enabled { get; set; }
  public double amount { get; set; }
  public int bonusCount { get; set; }
  public SelectedBonus selectedBonus { get; set; }
  public List<double> creditBonus { get; set; }
}
[Serializable]
public class SelectedBonus
{
  public int multiplier { get; set; }
  public double credit { get; set; }
}

[Serializable]
public class Paylines
{
  public List<Symbol> symbols { get; set; }
}
[Serializable]
public class Payload
{
  public double winAmount { get; set; }
  public List<Win> wins { get; set; }
}
[Serializable]
public class Win
{
  public int lineIndex { get; set; }
  public List<int> positions { get; set; }
  public double amount { get; set; }
}

[Serializable]
public class Symbol
{
  // public int ID { get; set; }
  // public string Name { get; set; }
  // [JsonProperty("multiplier")]
  // public object MultiplierObject { get; set; }

  // // This property will hold the properly deserialized list of lists of integers
  // [JsonIgnore]
  // public List<List<int>> Multiplier { get; private set; }

  // // Custom deserialization method to handle the conversion
  // [OnDeserialized]
  // internal void OnDeserializedMethod(StreamingContext context)
  // {
  // if (MultiplierObject == null)
  // {
  //   // If the multiplier is null in the JSON, set an empty list
  //   Multiplier = new List<List<int>>();
  // }
  // else if (MultiplierObject is JObject)
  // {
  //   // If the multiplier is an empty object ({}), treat it as an empty list
  //   Multiplier = new List<List<int>>();
  // }
  // else
  // {
  //   try
  //   {
  //     // Attempt to deserialize the multiplier object as a list of lists of integers
  //     Multiplier = JsonConvert.DeserializeObject<List<List<int>>>(MultiplierObject.ToString());
  //   }
  //   catch (Exception ex)
  //   {
  //     // Handle unexpected format or deserialization errors
  //     Debug.Log($"Deserialization error: {ex.Message}");
  //     Multiplier = new List<List<int>>(); // Fallback to an empty list on error
  //   }
  // }

  // public object defaultAmount { get; set; }
  // public object symbolsCount { get; set; }
  // public object description { get; set; }
  // public int freeSpin { get; set; }


  public int id { get; set; }
  public string name { get; set; }
  public List<int> multiplier { get; set; }
  public string description { get; set; }
}
[Serializable]
public class Player
{
  public double balance { get; set; }
  public double haveWon { get; set; }
  public double currentWining { get; set; }
}
[Serializable]
public class AuthTokenData
{
  public string cookie;
  public string socketURL;
  public string nameSpace;
}


