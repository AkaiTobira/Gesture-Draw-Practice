using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using CCP.Core;

public class ScrollbarElement : MonoBehaviour, IPointerDownHandler//, IPointerUpHandler
{
    private ImagesPreviews2 _previews;
    private int _index;

    public void Setup(ImagesPreviews2 _parent, int index, Texture2D text){
        _previews = _parent;
        _index = index;
        transform.GetChild(0).GetComponent<RawImage>().texture = text;
        transform.GetChild(0).GetComponent<RawImage>().SizeToParent();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        _previews.SetPrevies(_index);
    }
}
