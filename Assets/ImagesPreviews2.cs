using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.IO;
using System.Linq;
using TMPro;
using CCP.Core;
using UnityEngine.SceneManagement;


public class ImagesPreviews2 : MonoBehaviour
{
    [SerializeField] RawImage MainScreen;
    [SerializeField] GameObject PrevButton;
    [SerializeField] GameObject NextButton;
    [SerializeField] GameObject _scroolBar;
    [SerializeField] GameObject _scroolBarEnableButton;
    [SerializeField] Transform  _parent;
    [SerializeField] GameObject _prefab;


    int currentImageID = 0;

    void Start()
    {
        for(int i = 0; i < Settings._textureCache.Count; i++) {

            GameObject holder = Instantiate(_prefab, new Vector3(), Quaternion.identity, _parent);
            holder.GetComponent<ScrollbarElement>().Setup(this, i, Settings._textureCache[i]);
        }

        OnChangePicture(0);
        ToggleScrollBar(false);
    }

    public void SetPrevies(int index){
        MainScreen.texture = Settings._textureCache[index];
        if(Guard.IsValid(MainScreen)) MainScreen.SizeToParent();
    }

    public void OnChangePicture(int direction){
        if(currentImageID + direction < 0 || currentImageID + direction > Settings._textureCache.Count - 1) return;
        currentImageID += direction;
        
        PrevButton.SetActive(currentImageID != 0);
        NextButton.SetActive(currentImageID != Settings._textureCache.Count - 1);

        if(Settings._textureCache.Count > currentImageID){ 
            MainScreen.texture = Settings._textureCache[currentImageID];
            OnRectTransformDimensionsChange();
        }
    }

    private void Update(){

        if(Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.P)){
            OnChangePicture(-1);
        }

        if(Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.N)){
            OnChangePicture(1);
        }

        if(Input.GetKeyDown(KeyCode.Escape)){
            OnStopPress();
        }
    }

    public void ToggleScrollBar(bool enable){
        _scroolBar.SetActive(enable);
        _scroolBarEnableButton.SetActive(!enable);
    }


    public void OnStopPress(){
        SceneManager.LoadScene(1);
    }

    private void OnRectTransformDimensionsChange(){
        if(Guard.IsValid(MainScreen)) MainScreen.SizeToParent();
    }
}
