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



public class SpriteImporter : MonoBehaviour {
    
    [SerializeField] RawImage MainScreen;
    [SerializeField] TextMeshProUGUI _timerText;
    [SerializeField] GameObject PrevButton;
    [SerializeField] GameObject NextButton;

    private List<Texture2D> _textures = new List<Texture2D>();

    private void GenerateImagesOrder(){
        Settings._selectedPictures = new List<string>();
        List<string> keys2 = Settings.bindedPaths.Keys.ToList();

        while(Settings._selectedPictures.Count < Settings.NumberOfPicture){
            List<string> tempList = new List<string>();

            for(int i = 0; i < keys2.Count; i++) {
                tempList.Add(keys2[i]);
            }

            tempList.Shuffle();

            for(int i = 0; i < tempList.Count; i++) {
                if(Settings._selectedPictures.Count < Settings.NumberOfPicture)
                    Settings._selectedPictures.Add(tempList[i]);
            }
        }

        Debug.Log("Images generated " + Settings._selectedPictures.Count + " :: " + Settings.NumberOfPicture);
    }

    void Start()
    {
        GenerateImagesOrder();
        Settings.CompletedImages = 0;
        Settings.PreviewedImages = 0;

        OnChangePicture(0);
    }

    private float timer = 0;

    private void  FixedUpdate() {
        timer -= Time.fixedDeltaTime;
        if(timer < 0) {

            if(Settings.CompletedImages == Settings.NumberOfPicture-1){
                OnStopPress();
                return;
            }

            OnChangePicture(1);
        }

        _timerText.text = CUtils.FormatTime_MS((int)timer);
    }

    private void Update() {
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


    public void OnChangePicture(int direction){
        if(Settings.CompletedImages + direction < 0 || Settings.CompletedImages + direction > Settings.NumberOfPicture - 1) return;

        Settings.CompletedImages += direction;
        timer = Settings.SelectedTime;

        if(Settings.CompletedImages > Settings.PreviewedImages) Settings.PreviewedImages = Settings.CompletedImages;
        
        PrevButton.SetActive(Settings.CompletedImages != 0);
        NextButton.SetActive(Settings.CompletedImages != Settings.NumberOfPicture - 1);

        StartCoroutine(
                LoadImage(
                    Settings.bindedPaths[
                        Settings._selectedPictures[Settings.CompletedImages]]
                        ));
    }

    public void OnStopPress(){
        SceneManager.LoadScene(3);
    }

    IEnumerator LoadImage(string picture){

        Texture2D tex = new Texture2D(4, 4, TextureFormat.DXT1, false);
        /* Change to this in free time;
                    using (UnityWebRequest uwr = UnityWebRequest.Get(path))
            {
                yield return uwr.SendWebRequest();
                if (string.IsNullOrEmpty(uwr.error))
                {
                    t.LoadImage(uwr.downloadHandler.data);
                }
                else
                {
                    Debug.Log(uwr.error);
                }
            }
        */

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
        Settings._textureCache.Add(tex);

        MainScreen.texture = tex;
        MainScreen.SizeToParent();
    }

    private void OnRectTransformDimensionsChange(){
        MainScreen.SizeToParent();
    }
}