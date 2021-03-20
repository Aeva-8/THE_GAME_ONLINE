using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Title_Mannager : MonoBehaviour
{

    public Title_Mannager button;

    public void OnClick()
    {
        if (button == null)
        {
            throw new System.Exception("Button instance is null!!");
        }
        // 自身のオブジェクト名を渡す
        button.OnClick(this.gameObject.name);
    }

    protected virtual void OnClick(string objectName)
    {
        // 呼ばれることはない
        Debug.Log("Base Button");
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
