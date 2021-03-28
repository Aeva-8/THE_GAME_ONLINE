using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class FieldAnimationMannager : MonoBehaviour
{
    [SerializeField] GameObject NumUi;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (GameData.card_move.Count() != 0)
        {
            for (int i = 0; i < 4; i++)
            {
                GameObject Num_obj = NumUi.transform.GetChild(i).gameObject;
                Image Num_img = Num_obj.transform.GetChild(0).gameObject.GetComponent<Image>();
                Text Num_text = Num_obj.transform.GetChild(1).gameObject.GetComponent<Text>();
                if (GameData.card_move[i] == 0)
                {
                    Num_img.color = Color.red;
                    Num_text.color = Color.red;
                }
                else
                {
                    Num_img.color = Color.green;
                    Num_text.color = Color.green;
                }

            }
        }
        else
        {
            for (int i = 0; i < 4; i++)
            {
                GameObject Num_obj = NumUi.transform.GetChild(i).gameObject;
                Image Num_img = Num_obj.transform.GetChild(0).gameObject.GetComponent<Image>();
                Text Num_text = Num_obj.transform.GetChild(1).gameObject.GetComponent<Text>();
                Num_img.color = Color.white;
                Num_text.color = Color.white;

            }
        }
    }
}
