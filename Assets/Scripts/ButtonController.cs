using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ButtonController : Title_Mannager
{
    [SerializeField] GameObject Main;
    [SerializeField] GameObject Entry;
    [SerializeField] GameObject Create;
    string roomid;
    string player_name;
    int player_limit;
    protected override void OnClick(string objectName)
    {
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
        player_name = Create.transform.Find("Name").gameObject.GetComponent<InputField>().text;
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
        SceneManager.LoadScene("MainScene");


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
        
        player_name = Entry.transform.Find("Name").gameObject.GetComponent<InputField>().text;
        GameData.RoomId = roomid;
        GameData.Player_Name = player_name;
        //シーン遷移
        Debug.Log("Entry_Enter");
        SceneManager.LoadScene("MainScene");
    }
    private void CreateButtonClick()
    {
        Debug.Log("CreateButton Click");
        Create.SetActive(true);
        Main.SetActive(false);
    }

    private void EntryButtonClick()
    {
        Debug.Log("EntryButton Click");
        Entry.SetActive(true);
        Main.SetActive(false);
    }
}
