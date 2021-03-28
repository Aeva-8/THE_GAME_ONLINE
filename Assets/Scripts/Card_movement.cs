using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;


public class Card_movement : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Transform Field;
    public void OnBeginDrag(PointerEventData eventData)
    {
        Field = transform.parent;
        if (Field.name == "Hand_Field")
        {
            transform.SetParent(Field.parent, false);
            GetComponent<CanvasGroup>().blocksRaycasts = false;
            Check(int.Parse(this.name));
        }
        
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (Field.name == "Hand_Field")
        {
            transform.position = eventData.position;
        }

    }

    public void OnEndDrag(PointerEventData eventData)
    {

        transform.SetParent(Field, false);
        if (transform.parent.name != "Content")
        {
            GetComponent<CanvasGroup>().blocksRaycasts = true;
        }
        Main mainscript = GameObject.Find("Game_Maneger").GetComponent<Main>();
        mainscript.HandSort();
        GameData.card_move = new List<int>();

    }
    public void   Check(int card)
    {
        if (card != -1)
        {
            int last_num = -1;
            int nowcard = card;
            //カードを持った状態
            for (int i = 0; i < 4; i++)
            {
                try
                {
                    last_num = GameData.Field[i].Last();
                }
                catch (SystemException e)
                {
                    if (i == 0 || i == 1)
                    {
                        last_num = 100;
                    }
                    else if (i == 2 || i == 3)
                    {
                        last_num = 1;
                    }
                }
                if (i < 2)
                {
                    //100
                    if (nowcard > last_num && nowcard != last_num + 10)
                    {
                        //×
                        GameData.card_move.Add(0);
                    }
                    else
                    {
                        //〇
                        GameData.card_move.Add(1);
                    }
                }
                else
                {
                    //1
                    if (nowcard < last_num && nowcard != last_num - 10)
                    {
                        //×
                        GameData.card_move.Add(0);
                    }
                    else
                    {
                        //〇
                        GameData.card_move.Add(1);
                    }

                }
            }
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
