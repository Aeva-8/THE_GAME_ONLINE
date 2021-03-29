using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turn_end : MonoBehaviour
{
    [SerializeField] GameObject main;
    [SerializeField] GameObject Audio;
    public AudioClip Clip;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void OnClick()
    {
        Main mainscript = main.GetComponent<Main>();
        if (mainscript.player_state == 1)
        {
            if (GameData.Players[GameData.Player_Index].plays >= mainscript.min_plays)
            {

                mainscript.Turn_End();
                AudioSource audio = Audio.GetComponent<AudioSource>();
                audio.clip = Clip;
                audio.PlayDelayed(1.0f);
            }
            
        }
        transform.DOPunchScale(
            new Vector3(0.1f, 0.1f), 0.2f, 1
        ).SetEase(Ease.OutExpo);
    }
}
