using System.Collections;
using System.Collections.Generic;
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
