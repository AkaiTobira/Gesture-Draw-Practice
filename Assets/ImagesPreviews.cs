using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.IO;
using System.Linq;
using TMPro;
using CCP.Core;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;

public class ImagesPreviews : MonoBehaviour
{
    [SerializeField] RawImage MainScreen;
    [SerializeField] GameObject PrevButton;
    [SerializeField] GameObject NextButton;
    [SerializeField] GameObject _scroolBar;
    [SerializeField] GameObject _scroolBarEnableButton;
    [SerializeField] Transform  _parent;
    [SerializeField] GameObject _prefab;

    private List<Texture2D> _minisTextures = new List<Texture2D>();
    int currentImageID = 0;

    void Start()
    {
        for(int i = 0; i < Settings.PreviewedImages; i++) {
            StartCoroutine(
                LoadPreviewImage(
                    Settings.bindedPaths[Settings._selectedPictures[i]], i));
        }

        OnChangePicture(0);
    }

    public void SetPrevies(int index){
        StartCoroutine(LoadImage(Settings.bindedPaths[Settings._selectedPictures[index]], index));
    
        if(Guard.IsValid(MainScreen)) MainScreen.SizeToParent();
    }

    public void OnChangePicture(int direction){
        currentImageID += direction;
        
        PrevButton.SetActive(Settings.CompletedImages != 0);
        NextButton.SetActive(Settings.CompletedImages != Settings.NumberOfPicture - 1);

      //  if(_textures.Count > currentImageID){ 
            StartCoroutine(LoadImage(
            Settings.bindedPaths[Settings._selectedPictures[currentImageID]], currentImageID));

            OnRectTransformDimensionsChange();
      //  }
    }



    public void ToggleScrollBar(bool enable){
        _scroolBar.SetActive(enable);
        _scroolBarEnableButton.SetActive(!enable);
    }


    public void OnStopPress(){
        SceneManager.LoadScene(1);
    }

    IEnumerator LoadPreviewImage(string picture, int index){

        Texture2D tex = new Texture2D(4, 4, TextureFormat.DXT1, false);

        string[] strings = picture.Split("//");

        string whole = "";
        for(int i = 0; i < strings.Length; i++)
        {
            if(i == strings.Length-1)
            {
                whole += "//Minis//mini_" + strings[i];
            }
            else whole += strings[i] + "//"; 
        }

        using (UnityWebRequest loader = UnityWebRequestTexture.GetTexture("file://" + picture))
        {
            yield return loader.SendWebRequest();
 
            if (string.IsNullOrEmpty(loader.error))
            {
                tex = DownloadHandlerTexture.GetContent(loader);
                Debug.Log("Loaded file://" + picture);
            }
            else
            {
                Debug.LogError("Couldn't load file://" + picture);
        //        this.LogErrorFormat("Error loading Texture '{0}': {1}", loader.uri, loader.error);
            }
        }
        _minisTextures.Add(tex);

        GameObject holder = Instantiate(_prefab, new Vector3(), Quaternion.identity, _parent);
    //    holder.GetComponent<ScrollbarElement>().Setup(this, index, tex);
    }

    IEnumerator LoadImage(string picture, int index){

        Texture2D tex = new Texture2D(4, 4, TextureFormat.DXT1, false);

        using (UnityWebRequest loader = UnityWebRequestTexture.GetTexture("file://" + picture))
        {
            yield return loader.SendWebRequest();
 
            if (string.IsNullOrEmpty(loader.error))
            {
                tex = DownloadHandlerTexture.GetContent(loader);
                Debug.Log("Loaded file://" + picture);
            }
            else
            {
                Debug.LogError("Couldn't load file://" + picture);
        //        this.LogErrorFormat("Error loading Texture '{0}': {1}", loader.uri, loader.error);
            }
        }
        MainScreen.texture = tex;

        GameObject holder = Instantiate(_prefab, new Vector3(), Quaternion.identity, _parent);
    //    holder.GetComponent<ScrollbarElement>().Setup(this, index, tex);
    }

    private void OnRectTransformDimensionsChange(){
        if(Guard.IsValid(MainScreen)) MainScreen.SizeToParent();
    }
}
