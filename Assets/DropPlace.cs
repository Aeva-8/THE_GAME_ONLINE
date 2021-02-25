using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DropPlace : MonoBehaviour , IDropHandler
{
    [SerializeField] GameObject main;
    public void OnDrop(PointerEventData eventData)
    {
        Card_movement card = eventData.pointerDrag.GetComponent<Card_movement>();
        int obj_num = int.Parse(eventData.pointerDrag.name);
        int Field_num = int.Parse(this.gameObject.name);
        int last_num = GameData.Field[Field_num].Last();
        Main mainscript = main.GetComponent<Main>();
        if (mainscript.player_state == 1)
        {
            if (card != null)
            {
                if (this.gameObject.name == "0" || this.gameObject.name == "1")
                {
                    //100からスタートの列
                    if (obj_num > last_num  && obj_num != last_num + 10)
                    {
                        return;
                    }
                }
                else if (this.gameObject.name == "2" || this.gameObject.name == "3")
                {
                    //1からスタートの列
                    if (obj_num < last_num  && obj_num != last_num - 10)
                    {
                        return;
                    }
                }
                mainscript.play_count++;
                GameData.PlayerHand[GameData.Turn].Remove(obj_num);
                GameData.Field[Field_num].Add(obj_num);
                Text card_text = eventData.pointerDrag.transform.GetChild(0).gameObject.GetComponent<Text>();
                card_text.fontSize = 45;
                card.Field = this.transform;
                if (mainscript.GameClear_Check() == 1)
                {
                    mainscript.PerfectClear();
                }
                mainscript.GameEnd_Check();
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
