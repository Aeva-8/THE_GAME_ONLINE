using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public static class GameData
{
    public static int RoomId;
    public static int GameStatus = 1;
    public static int Clear = -1;
    public static int PlayerLimit = 4;
    public static int Hand_Limit = 6;
    public static int Turn = 0;
    //Turnはどのプレイヤーのターンかの情報を持つ
    public static  List<int> Deck = new List<int>();
    public static List<List<int>> PlayerHand = new List<List<int>>();
    public static List<List<int>> Field = new List<List<int>>();
}
public class Main : MonoBehaviour
{
    [SerializeField] GameObject cardPrefab;
    [SerializeField] Transform Hand_Field;
    [SerializeField] GameObject readyUI;
    [SerializeField] GameObject Status;
    public int player_state = -1;
    public int play_count = 0;
    public int min_plays = 2;
    //-1=操作不能.0=開始待機,1=操作可能,2=終了後待機
    void Generate_Field()
    {
        for (int i = 0; i < 4; i++)
        {
            List<int> add_tmp = new List<int>();
            GameData.Field.Add(add_tmp);
        }
        GameData.Field[0].Add(100);
        GameData.Field[1].Add(100);
        GameData.Field[2].Add(1);
        GameData.Field[3].Add(1);

    }
    void Generate_Deck()
    {
        for (int i=2; i < 100; i++)
        {
            GameData.Deck.Add(i);
        }
        GameData.Deck = GameData.Deck.OrderBy(i => Guid.NewGuid()).ToList();
    }
    void SetPlayer_Hand()
    {
        for (int i = 0; i < GameData.PlayerLimit; i++)
        {
            GameData.PlayerHand.Add(new List<int>());
        }
        foreach (List<int> item in GameData.PlayerHand)
        {
            Draw_Card(item);
        }
        Text txt_tmp = readyUI.transform.GetChild(0).gameObject.GetComponent<Text>();
        txt_tmp.text = "player" + GameData.Turn.ToString() + "のターン";
        player_state = 0;

    }
    void Generate_bundleHand()
    {
        foreach (int i in GameData.PlayerHand[GameData.Turn])
        {
            Instantiate_Card(i);
        }
    }
    void Generate_SplitHand()
    {
        List<int> before_tmp = new List<int>(GameData.PlayerHand[GameData.Turn]);
        Draw_Card(GameData.PlayerHand[GameData.Turn]);
        List<int> after_tmp = GameData.PlayerHand[GameData.Turn];
        IEnumerable<int> exceptlist = after_tmp.Except<int>(before_tmp);
        foreach (int i in exceptlist)
        {

            Instantiate_Card(i);
        }
    }
    void Draw_Card(List<int> item)
    {
        //上限までカードを山札から引いて手札に加える
        while (item.Count() != GameData.Hand_Limit)
        {
            if (GameData.Deck.Count() == 0)
            {
                return;
            }
            item.Add(GameData.Deck[0]);
            GameData.Deck.RemoveAt(0);
        }
        
    }
    void Instantiate_Card(int i)
    {
        GameObject obj = Instantiate(cardPrefab,Hand_Field);
        obj.name = i.ToString();
        Text obj_num = obj.transform.GetChild(0).gameObject.GetComponent<Text>();
        obj_num.text = i.ToString();
    }
    public int GameClear_Check()
    {
        int tmp = 0;
        foreach (List<int> item in GameData.PlayerHand)
        {
            tmp += item.Count();
        }
        if (tmp == 0)
        {
            return 1;
        }
        else if (tmp <= 10)
        {
            return 0;
        }
        return -1;
    }
    public void GameEnd_Check()
    {
        int count = 0;
        foreach (int i in GameData.PlayerHand[GameData.Turn])
        {
            int j = 0;
            foreach (List<int> tmp_Field in GameData.Field)
            {
                if (j < 2)
                {
                    //100からスタートの列
                    if (i < tmp_Field.Last()  || i == tmp_Field.Last() + 10)
                    {
                        count++;
                        break;
                    }
                }
                else
                {
                    //1からスタートの列
                    if (i > tmp_Field.Last() || i == tmp_Field.Last() - 10)
                    {
                        count++;
                        break;
                    }
                }
                j++;
            }
            
        }
        if (count >= min_plays - play_count)
        {
            return;
        }
        //ゲームエンド
        //Debug.Log("count = " + count+"min-playcount"+ (min_plays - play_count));
        readyUI.SetActive(true);
        Text txt_tmp = readyUI.transform.GetChild(0).gameObject.GetComponent<Text>();
        txt_tmp.text = "ゲーム失敗!!!\r\nん～～残念！w";
        player_state = -1;
    }
    void Clear()
    {
        Text txt_tmp = readyUI.transform.GetChild(0).gameObject.GetComponent<Text>();
        txt_tmp.text = "勝利!!!\r\n引き続き完全勝利を目指しましょう";
        GameData.Clear = 0;
        min_plays = 1;
    }
    public  void PerfectClear()
    {
        readyUI.SetActive(true);
        Text txt_tmp = readyUI.transform.GetChild(0).gameObject.GetComponent<Text>();
        txt_tmp.text = "完全勝利!!!!!";
        GameData.Clear = 1;
        player_state = -1;
    }
    public void Turn_End()
    {
        Generate_SplitHand();
        player_state = 2;
        Text txt_tmp = readyUI.transform.GetChild(0).gameObject.GetComponent<Text>();
        txt_tmp.text = "player"+GameData.Turn.ToString() + "のターンが終了";
        readyUI.SetActive(true);
        play_count = 0;
        int i=0;
        foreach (List<int> hand in GameData.PlayerHand)
        {
            foreach (int j in hand)
            {
                Debug.Log("player" + i + " : " + j);
            }
            i++;
        }
    }
    public void Ready()
    {
        if (player_state == 2)
        {
            //終了後待機
            //次のプレイヤーのターンにする。
            GameData.Turn++;
            if (GameData.Turn == GameData.PlayerLimit)
            {
                GameData.Turn = 0;
            }
            //手札全部消去
            foreach (Transform child in Hand_Field)
            {
                GameObject.Destroy(child.gameObject);
            }
            //ゲームクリア判定→終了判定をここに入れる
            Text txt_tmp;
            switch (GameClear_Check())
            {
                case 1:
                    //パーフェクトクリア
                    PerfectClear();
                    break;
                case 0:
                    //クリア
                    if (GameData.Clear == 0)
                    {
                        //すでにクリア条件達成済みのため続行
                        txt_tmp = readyUI.transform.GetChild(0).gameObject.GetComponent<Text>();
                        txt_tmp.text = "player" + GameData.Turn.ToString() + "のターン";
                        player_state = 0;

                    }
                    else
                    {
                        //初クリア
                        Clear();
                    }
                    break;
                case -1:
                    //未クリア
                    
                    txt_tmp = readyUI.transform.GetChild(0).gameObject.GetComponent<Text>();
                    txt_tmp.text = "player" + GameData.Turn.ToString() + "のターン";
                    player_state = 0;
                    break;
            }
            //終了判定
            GameEnd_Check();
        }
        else if(player_state ==0)
        {
            //開始待機時
            Generate_bundleHand();
            readyUI.SetActive(false);
            player_state = 1;
        }
    }
    public void Update_Ui()
    {
        int temp = 0;
        foreach (List<int> item in GameData.PlayerHand)
        {
            temp += item.Count();
        }
        Text text_tmp = Status.transform.GetChild(0).gameObject.GetComponent<Text>();
        text_tmp.text = "山札" + GameData.Deck.Count() + "枚 : 残りカード" + (GameData.Deck.Count() + temp)+"枚\r\n"+"現在のプレイ数/必要プレイ数 : "+play_count+"/"+min_plays;
    }
    // Start is called before the first frame update
    void Start()
    {
        Generate_Field();
        Generate_Deck();
        SetPlayer_Hand();
    }

    // Update is called once per frame
    void Update()
    {
        Update_Ui();
    }
}
