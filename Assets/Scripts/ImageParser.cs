using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.Networking;

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
            if(!Directory.Exists(Settings.MinisPaths)) {Directory.CreateDirectory(Settings.MinisPaths);}

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
        Settings.MinisList  = Directory.GetFiles(Settings.MinisPaths);

        if(_toProcess.Count > 0 || Settings.ImagesList.Length == Settings.ImagesSet.Count ) return;

        for(int i = 0; i < Settings.PredefList.Length; i++) Settings.PredefSet.Add(Settings.PredefList[i]);
        for(int i = 0; i < Settings.ImagesList.Length; i++) Settings.ImagesSet.Add(Settings.ImagesList[i]);

        foreach(string str in Settings.ImagesSet)
        {
            string fileName = GetFileName(str);
            if(fileName.Contains(".meta")) continue;
            if(Settings.bindedPaths.ContainsKey(fileName)) continue;

            string replaced = str.Replace("Images", "Optimized");
            if(Settings.PredefSet.Contains(replaced))
            {
                Settings.bindedPaths[str] = replaced;
            }
            else
            {
                _toProcess.Add(str);
            }
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
                timer = 5f;
            }
        }
    }


    IEnumerator OptimizeTexture(){
        loadingTexture = true;
        _textureToParse = new Texture2D(4, 4, TextureFormat.RGB24, false);

        using (UnityWebRequest loader = UnityWebRequestTexture.GetTexture("file://" + _toProcess[0]))
        {
            yield return loader.SendWebRequest();
 
            if (string.IsNullOrEmpty(loader.error))
            {
                _textureToParse = DownloadHandlerTexture.GetContent(loader);
                Debug.Log("Loaded file://" + _toProcess[0]);
            }
            else
            {
                Debug.LogError("Couldn't load file://" + _toProcess[0]);
        //        this.LogErrorFormat("Error loading Texture '{0}': {1}", loader.uri, loader.error);
            }
        }

    //    WWW www = new WWW("file://" + _toProcess[0]);    
    //    yield return www;
    //    www.LoadImageIntoTexture(_textureToParse);
    //    Debug.Log("Loaded file://" + _toProcess[0]);

        int textureBiggerSize = _textureToParse.height;
        float scale = _textureToParse.height / 1024f;
        float scaleMinis = _textureToParse.height / 128f;

        Texture2D newScreenshot = ScaleTexture(
            _textureToParse, 
            (int)(_textureToParse.width/scaleMinis), 
            (int)(_textureToParse.height/scaleMinis));

        byte[] bytes = newScreenshot.EncodeToPNG();
        File.WriteAllBytes(Settings.MinisPaths + "\\mini_" + GetFileName(_toProcess[0]), bytes);

        newScreenshot = ScaleTexture(
            _textureToParse, 
            (int)(_textureToParse.width/scale), 
            (int)(_textureToParse.height/scale));

        bytes = newScreenshot.EncodeToPNG();
        File.WriteAllBytes(Settings.PredefPaths + "\\" + GetFileName(_toProcess[0]), bytes);
        
        loadingTexture = false;
        _toProcess.RemoveAt(0);
        _textureToParse = null;

        if(_toProcess.Count == 0) MapImages();

        Resources.UnloadUnusedAssets();
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
