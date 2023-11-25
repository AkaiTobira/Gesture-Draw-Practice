using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class ImageParser : MonoBehaviour
{
    private static ImageParser _parser;
    void Start()
    {
        if(_parser == null){
            DontDestroyOnLoad(gameObject);
            _parser = this;
            
            if(!Directory.Exists(Settings.ImagePath))  {Directory.CreateDirectory(Settings.ImagePath);}
            if(!Directory.Exists(Settings.PredefPaths)){Directory.CreateDirectory(Settings.PredefPaths);}

            Settings.bindedPaths = new Dictionary<string, string>();
            MapImages();
        }else{
            Destroy(gameObject);
        }
    }

    private List<string> _toProcess = new List<string>();

    public static string GetProgress(){
        if(_parser != null){
            return "Images to process : " + _parser._toProcess.Count;
        }

        return "Parser Error";
    }

    public static bool IsProcessing(){
        if(_parser != null){
            return _parser._toProcess.Count > 0;
        }

        return false;
    }

    private void MapImages(){
        Settings.PredefList = Directory.GetFiles(Settings.PredefPaths);
        Settings.ImagesList = Directory.GetFiles(Settings.ImagePath);

        if(_toProcess.Count > 0) return;

        for(int j = 0; j < Settings.ImagesList.Length; j++) {
            string fileName = GetFileName(Settings.ImagesList[j]);

            if(Settings.bindedPaths.ContainsKey(fileName)) continue;
            if(fileName.Contains(".meta")) continue;

            bool found = false;
            for(int i = 0; i < Settings.PredefList.Length; i++) {
                string fileName2 = GetFileName(Settings.PredefList[i]);
                if(fileName2.Contains(".meta")) continue;

                if(fileName == fileName2){
                    found = true;
                    Settings.bindedPaths[Settings.ImagesList[j]] = Settings.PredefList[i];
                    break;
                }
            }

            if(!found) _toProcess.Add(Settings.ImagesList[j]);
        }

        Debug.Log("Mapped images :" + Settings.bindedPaths.Count);
    }

    private bool loadingTexture = false;
    private Texture2D _textureToParse = null;

    float timer = 5;

    void Update()
    {
        if(_toProcess.Count != 0){
            if(loadingTexture) return;
            StartCoroutine(OptimizeTexture());

            Debug.Log(_toProcess.Count);
        }else{
            timer -= Time.deltaTime;
            if(timer < 0){
                MapImages();
                timer = 5;
            }
        }
    }

    IEnumerator OptimizeTexture(){
        loadingTexture = true;
        _textureToParse = new Texture2D(4, 4, TextureFormat.RGB24, false);

        WWW www = new WWW("file://" + _toProcess[0]);    
        yield return www;
        www.LoadImageIntoTexture(_textureToParse);
        Debug.Log("Loaded file://" + _toProcess[0]);

        int textureBiggerSize = _textureToParse.height;
        float scale = _textureToParse.height / 1024f;


        Texture2D newScreenshot = ScaleTexture(
            _textureToParse, 
            (int)(_textureToParse.width/scale), 
            (int)(_textureToParse.height/scale));

        byte[] bytes = newScreenshot.EncodeToPNG();
        File.WriteAllBytes(Settings.PredefPaths + "\\" + GetFileName(_toProcess[0]), bytes);
        
        loadingTexture = false;
        _toProcess.RemoveAt(0);
        _textureToParse = null;

        if(_toProcess.Count == 0) MapImages();
    }
    private Texture2D ScaleTexture(Texture2D source,int targetWidth,int targetHeight) {
        Texture2D result = new Texture2D(targetWidth,targetHeight,source.format,true);
        Color[] rpixels = result.GetPixels(0);
        float incX=((float)1/source.width)*((float)source.width/targetWidth);
        float incY=((float)1/source.height)*((float)source.height/targetHeight);
        for(int px=0; px<rpixels.Length; px++) {
                rpixels[px] = source.GetPixelBilinear(incX*((float)px%targetWidth),
                                    incY*((float)Mathf.Floor(px/targetWidth)));
        }
        result.SetPixels(rpixels,0);
        result.Apply();
        return result;
    }

    private string GetFileName(string path){
        string[] pathParths = path.Split('\\');
        return pathParths[pathParths.Length-1];
    }
}
