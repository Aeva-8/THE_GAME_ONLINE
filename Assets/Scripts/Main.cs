using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using WebSocketSharp;
using System.Threading;
using Newtonsoft.Json;
using DG.Tweening;

public static class GameData
{
    public static string UUId;
    public static int Player_Index;
    public static int Turn_Index;
    public static string RoomId;
    public static string Player_Name;
    public static int gameState = 0;
    public static int Clear = -1;
    public static int PlayerLimit=-1;
    public static int Hand_Limit = 6;
    public static int Turn = 0;
    public static bool ReturnTitle=false;
    //Turnはどのプレイヤーのターンかの情報を持つ
    public static  List<int> Deck = new List<int>();
    public static List<List<int>> Field = new List<List<int>>();
    public static List<PlayerData> Players = new List<PlayerData>();
}
public  class PlayerData
{
    public  List<int> hands = new List<int>();
    public  string id;
    public  string name;
    public  int plays;
}







[Serializable]
public  class Create_StartSerializData
{
    public  string func;
    public  string name;
    public  int playerLimit;
    public  string roomId;
    public  string uuid;
}
[Serializable]
public class Enter_StartSerializData
{
    public string func;
    public string name;
    public string roomId;
    public string uuid;
}
[Serializable]
public class HeatbeatData
{
    public string func;
    public string kind;
}
[Serializable]
public class GameSerializData
{
    public string func;
    public RoomObject roomObject;
}
[Serializable]
public  class RoomObject
{
    public  List<int> deck = new List<int>();
    public int gameState;
    public int gameTurnIndex;
    public int minPlays;
    public int playerLimit;
    public Dictionary<string, List<int>> leads = new Dictionary<string, List<int>>();
    public string roomId;
    public List<Player> players = new List<Player>();
}
[Serializable]
public class Leads
{
    public List<int> asc = new List<int>();
}
[Serializable]
public class Player
{
    public List<int> hands = new List<int>();
    public string id;
    public string name;
    public int plays;
}
[Serializable]
public class Update
{
    public string func;
    public RoomObject roomObject;
    public string updateType;
}
[Serializable]
public class Progress
{
    public string func;
    public RoomObject roomObject;
    public string progType;
}
[Serializable]
public class GameEnd
{
    public string func;
    public string endType;
}






public class Main : MonoBehaviour
{
    [SerializeField] GameObject cardPrefab;
    [SerializeField] Transform Hand_Field;
    [SerializeField] GameObject readyUI;
    [SerializeField] GameObject Status;
    [SerializeField] Transform Board;
    [SerializeField] GameObject Massagetext;
    [SerializeField] GameObject PopupObj;

    public int player_state = -1;
    public int play_count = 0;
    public int min_plays = 2;
    public bool start=false;
    public WebSocket webSocket = null;
    public bool updatefield=false;
    public bool yourturn = false;
    public bool updatehand = false;
    public bool badend = false;
    public bool preEnd = false;
    public bool preForcedEnd = false;
    public bool perfectEnd = false;
    public bool gameerror = false;

    public List<List<int>> before_field= new List<List<int>>();
    public List<int> before_hand = new List<int>();

    //-1=操作不能.0=開始待機,1=操作可能,2=終了後待機


    void  Instantiate_Card(int i,int j)
    {
        //-1で手札0,1,2,3はField
        if (j == -1)
        {
            if (cardPrefab == null)
            {
                Debug.Log("NULL");
            }
             GameObject obj =  Instantiate(cardPrefab, Hand_Field);
            obj.name = i.ToString();
            Text obj_num = obj.transform.GetChild(0).gameObject.GetComponent<Text>();
            obj_num.text = i.ToString();
            HandSort();
        }
        else {
            GameObject obj = Instantiate(cardPrefab, Board.GetChild(j).GetChild(0).GetChild(0).gameObject.transform);
            obj.name = i.ToString();
            Text obj_num = obj.transform.GetChild(0).gameObject.GetComponent<Text>();
            obj_num.text = i.ToString();
            obj.GetComponent<CanvasGroup>().blocksRaycasts = false;
            obj_num.fontSize = 70;



        }
        
    }
    public void Turn_End()
    {
        Progress play_progress = new Progress();
        play_progress.func = "game-progress";
        play_progress.progType = "turn-end";
        play_progress.roomObject = SerializingData();
        string json = JsonConvert.SerializeObject(play_progress); //整形する
        Debug.Log("play json :" + json);
        webSocket.Send(json);
        player_state = 0;
    }



    public void Update_Ui()
    {
        int temp = 0;
        foreach (PlayerData item in GameData.Players)
        {
            temp += item.hands.Count();
        }
        Text text_tmp = Status.transform.GetChild(0).gameObject.GetComponent<Text>();
        text_tmp.text = "山札" + GameData.Deck.Count() + "枚 : 残りカード" + (GameData.Deck.Count() + temp)+"枚\r\n"+"現在のプレイ数/必要プレイ数 : "+GameData.Players[GameData.Player_Index].plays+"/"+min_plays;

        Massagetext.GetComponent<Text>().text = GameData.Players[GameData.Turn_Index].name+"のターンです";
    }
    void StartHand()
    {
        //Debug.Log("Count" + GameData.Players[GameData.Turn_Index].hands.Count());
        
        foreach (int i in GameData.Players[GameData.Player_Index].hands)
        {
            Instantiate_Card(i,-1);
        }
    }
    void UpdateHand(List<int> before_tmp, List<int> after_tmp)
    {
        IEnumerable<int> exceptlist = after_tmp.Except<int>(before_tmp);
        foreach (int i in exceptlist)
        {

            Instantiate_Card(i,-1);
        }
    }
    void UpdateField(List<int> before_tmp, List<int> after_tmp,int j)
    {
        IEnumerable<int> exceptlist = after_tmp.Except<int>(before_tmp);

        foreach (int i in exceptlist)
        {
             Instantiate_Card(i,j);
        }
    }
    public void PlayCard(int card_num,int Field_num)
    {
        GameData.Players[GameData.Player_Index].hands.Remove(card_num);
        //GameData.PlayerHand[GameData.Turn_Index].Remove(card_num);
        GameData.Field[Field_num].Add(card_num);
        //サーバーに送る
        Progress play_progress = new Progress();
        play_progress.func="game-progress";
        play_progress.progType = "play";
        play_progress.roomObject = SerializingData();
        string json = JsonConvert.SerializeObject(play_progress); //整形する
        Debug.Log("play json :" + json);
        webSocket.Send(json);
        GameData.Players[GameData.Player_Index].plays = 0;
    }
    void SubGameData(RoomObject sD)
    {

        GameData.Deck = sD.deck;
        GameData.gameState = sD.gameState;
        GameData.Turn_Index = sD.gameTurnIndex;
        GameData.PlayerLimit = sD.playerLimit;
        min_plays = sD.minPlays;
        GameData.Field.Clear();
        GameData.Field.Add(sD.leads["desc01"]);
        GameData.Field.Add(sD.leads["desc02"]);
        GameData.Field.Add(sD.leads["asc01"]);
        GameData.Field.Add(sD.leads["asc02"]);

        GameData.Players.Clear();

        foreach (Player temp in sD.players)
        {
            PlayerData temp_player = new PlayerData();
            temp_player.hands = temp.hands;
            temp_player.id = temp.id;
            temp_player.name = temp.name;
            temp_player.plays = temp.plays;
            GameData.Players.Add(temp_player);
        }
        //Debug.Log(GameData.Players[0].hands.Count());
    }
    RoomObject SerializingData()
    {
        RoomObject newobj = new RoomObject();
        newobj.deck = GameData.Deck;
        newobj.gameState = GameData.gameState;
        newobj.gameTurnIndex = GameData.Turn_Index;
        newobj.minPlays = min_plays;
        newobj.playerLimit = GameData.PlayerLimit;
        newobj.roomId = GameData.RoomId;
        newobj.leads.Add("desc01", GameData.Field[0]);
        newobj.leads.Add("desc02", GameData.Field[1]);
        newobj.leads.Add("asc01", GameData.Field[2]);
        newobj.leads.Add("asc02", GameData.Field[3]);
        foreach (PlayerData temp in GameData.Players)
        {
            Player ply_temp = new Player();
            ply_temp.hands = temp.hands;
            ply_temp.id = temp.id;
            ply_temp.name = temp.name;
            ply_temp.plays = temp.plays;
            newobj.players.Add(ply_temp);
        }
        return newobj;
    }
    void Popup(string msg)
    {
        PopupObj.transform.DOScale(1f, 0.2f)
            .SetEase(Ease.OutExpo);
        PopupObj.transform.GetChild(1).gameObject.GetComponent<Text>().text = msg;
    }
    public void HandSort()
    {
        List<Transform> objList = new List<Transform>();

        // 子階層のGameObject取得
        var childCount = Hand_Field.gameObject.transform.childCount;
        for (int i = 0; i < childCount; i++)
        {
            objList.Add(Hand_Field.gameObject.transform.GetChild(i));
        }

        // オブジェクトを名前で昇順ソート
        // ★ここを用途に合わせて変更してください
        objList.Sort((obj1, obj2) => string.Compare(obj1.name, obj2.name));

        // ソート結果順にGameObjectの順序を反映
        foreach (var obj in objList)
        {
            obj.SetSiblingIndex(childCount - 1);
        }
    }
    void GameStart()
    {
        StartHand();
    }
    //サーバーに接続
    void Connect_Start()
    {

        webSocket = new WebSocket("ws://bews.tgo.asuka.icu");
        webSocket.OnOpen += (sender, args) => { Debug.Log("WebSocket opened."); };
        webSocket.OnMessage += (sender, args) => 
        {
            Debug.Log("受信 is "+args.Data);
            if (args.Data.Contains("game-heartbeat"))
            {
                //pingpong
                var send_heart = new HeatbeatData();
                send_heart.func = "game-heartbeat";
                send_heart.kind = "pong";
                string json_heart = JsonConvert.SerializeObject(send_heart);
                //Debug.Log(json_heart);
                webSocket.Send(json_heart);
            }
            if (args.Data.Contains("game-start"))
            {
                
                //ゲームを開始
                GameSerializData start_data = new GameSerializData();
                start_data = JsonConvert.DeserializeObject<GameSerializData>(args.Data);
                //自分のindexを検索
                int i = 0;
                int myindex=0;
                
                foreach (Player temp_player in start_data.roomObject.players)
                {
                    if (temp_player.id == GameData.UUId)
                    {
                        myindex = i;
                    }
                    i++;
                }
                Debug.Log("myindex is " + myindex);
                GameData.Player_Index = myindex;

                Debug.Log("myindex is " + GameData.Player_Index + "nowindex is " + start_data.roomObject.gameTurnIndex);
                //自分のターンかどうか判断
                if (start_data.roomObject.gameTurnIndex == GameData.Player_Index)
                {
                    player_state = 1;
                    yourturn = true;
                }

                //送られたデータを反映させる・
                SubGameData(start_data.roomObject);
                GameData.gameState = 1;


            }
            if (args.Data.Contains("game-update"))
            {
                Update update_data = JsonConvert.DeserializeObject<Update>(args.Data);
                if (args.Data.Contains("next-turn"))
                {
                    GameData.Turn_Index= update_data.roomObject.gameTurnIndex;
                    //手札補充
                    before_hand.Clear();
                    before_hand = GameData.Players[GameData.Player_Index].hands;
                    SubGameData(update_data.roomObject);
                    updatehand = true;

                    //次のターンに変え、自分のターンだったらプレイ可能にする。
                    if (GameData.Turn_Index == GameData.Player_Index)
                    {
                        yourturn = true;
                        player_state = 1;
                    }


                }
                else
                {
                    //Fieldの差分をチェックし画面更新し、データを全更新する

                    before_field.Clear();
                    for (int i = 0; i < 4; i++)
                    {
                        before_field.Add(GameData.Field[i]);
                    }

                    SubGameData(update_data.roomObject);
                    updatefield = true;

                }

                
                
            }
            if (args.Data.Contains("game-end"))
            {
                GameEnd end_data = JsonConvert.DeserializeObject<GameEnd>(args.Data);
                if (end_data.endType == "badEnd")
                {
                    GameData.ReturnTitle = true;
                    badend = true;
                }
                else if (end_data.endType == "preEnd")
                {
                    preEnd = true;
                }
                else if (end_data.endType == "preForcedEnd")
                {
                    GameData.ReturnTitle = true;
                    preForcedEnd = true;
                }
                else if (end_data.endType == "perfectEnd")
                {
                    GameData.ReturnTitle = true;
                    perfectEnd = true;
                }
            }
            if (args.Data.Contains("game-error"))
            {
                GameData.ReturnTitle = true;
                gameerror = true;
                
            }



        };
        webSocket.OnError += (sender, args) => { Debug.Log("Error"); };
        webSocket.OnClose += (sender, args) => { Debug.Log("WebScoket closed"); };
        webSocket.Connect();

        if (GameData.PlayerLimit == -1)
        {
            //部屋に入るを選択
            var data = new Enter_StartSerializData();

            data.func = "join-game";
            data.name = GameData.Player_Name;
            data.roomId = GameData.RoomId;
            System.Guid guid = System.Guid.NewGuid();
            GameData.UUId = guid.ToString();
            data.uuid = guid.ToString();
            string json = JsonConvert.SerializeObject(data); //整形する
            webSocket.Send(json);
        }
        else
        {
            //部屋を作るを選択
            var data = new Create_StartSerializData();
            data.func = "join-game";
            data.name = GameData.Player_Name;
            data.playerLimit = GameData.PlayerLimit;
            data.roomId = GameData.RoomId;
            System.Guid guid = System.Guid.NewGuid();
            data.uuid = guid.ToString();
            string json = JsonConvert.SerializeObject(data); //整形する
            webSocket.Send(json);
        }
        
    }
    // Start is called before the first frame update
    void Start()
    {
        //サーバーに接続
        Connect_Start();
        
    }
    
    // Update is called once per frame
    void Update()
    {
        if (GameData.gameState == 1)
        {
            if (start == false)
            {
                //ゲームが開始された
                GameStart();
                start = true;
                Debug.Log(player_state);
            }
        }
        if (updatefield == true)
        {
            for (int i = 0; i < 4; i++)
            {
                UpdateField(before_field[i], GameData.Field[i], i);
            }
            updatefield = false;
        }
        if (yourturn == true)
        {
            Popup("あなたのターンです");
            yourturn = false;

        }
        if (updatehand == true)
        {
            UpdateHand(before_hand, GameData.Players[GameData.Player_Index].hands);
            updatehand = false;
        }
        if (badend == true)
        {
            //バッドエンド時の処理
            Debug.Log("gameend");
            player_state = 0;
            GameData.gameState = 0;

            Massagetext.GetComponent<Text>().text = "ゲーム失敗です!!!!";
            Popup("ゲーム失敗");
            badend =false;
        }
        if (preEnd == true)
        {
            Popup("ゲームクリアです!!\n引き続きパーフェクトクリアに向けて\nプレイできます");
            preEnd = false;
        }
        if (preForcedEnd == true)
        {
            player_state = 0;
            GameData.gameState = 0;
            Popup("ゲームクリア");
            preForcedEnd = false;
        }
        if (perfectEnd == true)
        {
            player_state = 0;
            GameData.gameState = 0;
            Popup("ゲームパーフェクトクリア!!\nThank you for playing");
            perfectEnd = false;
        }
        if (gameerror == true)
        {
            player_state = 0;
            GameData.gameState = 0;
            Popup("ゲームエラー : 接続できません\nタイトルに戻ります");
        }
        if (GameData.gameState != 0)
        {
            Update_Ui();
        }

    }
}
