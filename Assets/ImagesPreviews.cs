using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.IO;
using System.Linq;
using TMPro;
using CCP.Core;
using UnityEngine.SceneManagement;


public class ImagesPreviews : MonoBehaviour
{
    [SerializeField] RawImage MainScreen;
    [SerializeField] GameObject PrevButton;
    [SerializeField] GameObject NextButton;
    [SerializeField] GameObject _scroolBar;
    [SerializeField] GameObject _scroolBarEnableButton;
    [SerializeField] Transform  _parent;
    [SerializeField] GameObject _prefab;


    private List<Texture2D> _textures = new List<Texture2D>();

    int currentImageID = 0;

    void Start()
    {
        for(int i = 0; i < Settings.PreviewedImages; i++) {
            StartCoroutine(
                LoadImage(
                    Settings.bindedPaths[
                        Settings._selectedPictures[i]]
                        , i));
        }

        OnChangePicture(0);
    }

    public void SetPrevies(int index){
        MainScreen.texture = _textures[index];
        if(Guard.IsValid(MainScreen)) MainScreen.SizeToParent();
    }

    public void OnChangePicture(int direction){
        currentImageID += direction;
        
        PrevButton.SetActive(Settings.CompletedImages != 0);
        NextButton.SetActive(Settings.CompletedImages != Settings.NumberOfPicture - 1);

        if(_textures.Count > currentImageID){ 
            MainScreen.texture = _textures[currentImageID]; 
            OnRectTransformDimensionsChange();
        }
    }



    public void ToggleScrollBar(bool enable){
        _scroolBar.SetActive(enable);
        _scroolBarEnableButton.SetActive(!enable);
    }


    public void OnStopPress(){
        SceneManager.LoadScene(1);
    }

    IEnumerator LoadImage(string picture, int index){

        Texture2D tex = new Texture2D(4, 4, TextureFormat.DXT1, false);
        WWW www = new WWW("file://" + picture);    
        yield return www;
        www.LoadImageIntoTexture(tex);
        Debug.Log("Loaded file://" + picture);
        _textures.Add(tex);

        GameObject holder = Instantiate(_prefab, new Vector3(), Quaternion.identity, _parent);
    //    holder.GetComponent<ScrollbarElement>().Setup(this, index, tex);
    }

    private void OnRectTransformDimensionsChange(){
        if(Guard.IsValid(MainScreen)) MainScreen.SizeToParent();
    }
}
