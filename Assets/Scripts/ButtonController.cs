using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UniRx;

public class ButtonController : Title_Mannager
{
    [SerializeField] GameObject Main;
    [SerializeField] GameObject Entry;
    [SerializeField] GameObject Create;
    [SerializeField] Button MainCreateBtn;
    [SerializeField] Button MainEntryBtn;
    string roomid;
    string player_name;
    int player_limit;
    protected override void OnClick(string objectName)
    {
        ButtonAnimation(objectName);
        Debug.Log("ButtonName is "+ objectName);
        // 渡されたオブジェクト名で処理を分岐
        if ("Create".Equals(objectName))
        {
            this.CreateButtonClick();
        }
        else if ("Entry".Equals(objectName))
        {
            this.EntryButtonClick();
        }
        else if ("Create_Enter".Equals(objectName))
        {
            this.Create_EntryButtonClick();
        }
        else if ("Entry_Enter".Equals(objectName))
        {
            this.Entry_EntryButtonClick();
        }
        else if ("Entry_Return".Equals(objectName))
        {
            this.Entry_ReturnButtonClick();
        }
        else if ("Create_Return".Equals(objectName))
        {
            this.Create_ReturnButtonClick();
        }
        else
        {
            throw new System.Exception("Not implemented!!");
        }
    }
    private void Create_EntryButtonClick()
    {
        try
        {
            roomid = Create.transform.Find("RoomNum").gameObject.GetComponent<InputField>().text;
        }
        catch (SystemException e)
        {
            return;
        }
        if (roomid.Length < 5)
        {
            //5文字以下
            return;
        }
        try
        {
            player_name = Create.transform.Find("Name").gameObject.GetComponent<InputField>().text;
        }
        catch (SystemException e)
        {
            return;
        }
        try
        {
            player_limit = int.Parse(Create.transform.Find("PlayerLimit").gameObject.GetComponent<InputField>().text);
        }
        catch (SystemException e)
        {
            return;
        }
        if (player_limit<2)
        {
            return;
        }
        GameData.RoomId = roomid;
        GameData.Player_Name = player_name;
        GameData.PlayerLimit = player_limit;

        //シーン遷移
        Observable.Return(Unit.Default)
        .Delay(TimeSpan.FromMilliseconds(200))
        .Take(1)
          .Subscribe(_ =>
          {
              SceneManager.LoadScene("MainScene");
          });

    }
    private void Entry_EntryButtonClick()
    {
        try
        {
            roomid = Entry.transform.Find("RoomNum").gameObject.GetComponent<InputField>().text;
        }
        catch (SystemException e)
        {
            return;
        }
        if (roomid.Length < 5)
        {
            //5文字以下
            return;
        }
        try
        {
            player_name = Entry.transform.Find("Name").gameObject.GetComponent<InputField>().text;
        }
        catch (SystemException e)
        {
            return;
        }
        
        GameData.RoomId = roomid;
        GameData.Player_Name = player_name;
        //シーン遷移
        Observable.Return(Unit.Default)
        .Delay(TimeSpan.FromMilliseconds(200))
        .Take(1)
          .Subscribe(_ =>
          {
              Debug.Log("Entry_Enter");
              SceneManager.LoadScene("MainScene");
          });
    }
    private void CreateButtonClick()
    {
        Debug.Log("CreateButton Click");
        Observable.Return(Unit.Default)
        .Delay(TimeSpan.FromMilliseconds(200))
        .Take(1)
          .Subscribe(_ => {
              Create.SetActive(true);
              Create.transform.DOScale(1f, 0.2f)
              .SetEase(Ease.OutExpo);
              MainCreateBtn.interactable = false;
              MainEntryBtn.interactable = false;
          });
    }

    private void EntryButtonClick()
    {
        Debug.Log("EntryButton Click");
        Observable.Return(Unit.Default)
        .Delay(TimeSpan.FromMilliseconds(200))
        .Take(1)
          .Subscribe(_ => {
              Entry.transform.DOScale(1f, 0.2f)
              .SetEase(Ease.OutExpo);
              MainCreateBtn.interactable = false;
              MainEntryBtn.interactable = false;
          });
        
    }
    public void Entry_ReturnButtonClick()
    {
        Observable.Return(Unit.Default)
        .Delay(TimeSpan.FromMilliseconds(200))
        .Take(1)
        .Subscribe(_ => {
            Entry.transform.DOScale(0f, 0.2f)
              .SetEase(Ease.OutExpo);
            MainCreateBtn.interactable = true;
            MainEntryBtn.interactable = true;
        });
        
    }
    public void Create_ReturnButtonClick()
    {
        Observable.Return(Unit.Default)
        .Delay(TimeSpan.FromMilliseconds(200))
        .Take(1)
        .Subscribe(_ => {
            Create.transform.DOScale(0f, 0.2f)
            .SetEase(Ease.OutExpo);
            MainCreateBtn.interactable = true;
            MainEntryBtn.interactable = true;
        });
        
    }
    public void ButtonAnimation(string objname)
    {
        GameObject obj = GameObject.Find(objname);
        obj.transform.DOPunchScale(
            new Vector3(0.1f, 0.1f),0.2f ,1 
        ).SetEase(Ease.OutExpo); 
        Observable.Return(Unit.Default)
        .Delay(TimeSpan.FromMilliseconds(200))
        .Take(1)
        .Subscribe(_ => {
            obj.transform.DOScale(1f, 0f).SetEase(Ease.OutElastic);
        });
    }
}
