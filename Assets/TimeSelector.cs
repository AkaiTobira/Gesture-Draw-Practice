using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.IO;
using UnityEngine.SceneManagement;

static class Settings{
    public static int SelectedTime = 0;
    public static int NumberOfPicture = 0;
    public static int CompletedImages = 0;
    public static int PreviewedImages = 0;


    public static string ImagePath = Directory.GetCurrentDirectory() + "/Images";
    public static string PredefPaths = Directory.GetCurrentDirectory() + "/Optimized";
    public static string MinisPaths = Directory.GetCurrentDirectory() + "/Optimized/Minis";

    public static string[] ImagesList = null;
    public static string[] PredefList = null;
    public static string[] MinisList  = null;

    public static HashSet<string> ImagesSet = new HashSet<string>();
    public static HashSet<string> PredefSet = new HashSet<string>();
    public static HashSet<string> MinisSet = new HashSet<string>();

    

    public static Dictionary<string, string> bindedPaths = null;

    public static List<string> _selectedPictures = null;
    public static List<Texture2D> _textureCache = new List<Texture2D>();
}


public class TimeSelector : MonoBehaviour
{
    [SerializeField] GameObject[] TimeButtons;
    [SerializeField] GameObject[] NumberButtons;
    [SerializeField] int[] times;
    [SerializeField] int[] numbers;
    [SerializeField] TextMeshProUGUI _foundedPictures;

    private void Start() {
        for(int i = 0; i < TimeButtons.Length; i++) {
            TimeButtons[i].transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = GetTimeFormated(i);
            TimeButtons[i].transform.parent.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = GetTimeFormated(i);
        }

        for(int i = 0; i < NumberButtons.Length; i++) {
            NumberButtons[i].transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = numbers[i].ToString();
            NumberButtons[i].transform.parent.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = numbers[i].ToString();
        }

        OnTimeButtonSelect(0);
        OnNumberOfPictureButtonSelect(0);

        if(!Directory.Exists(Settings.ImagePath)){Directory.CreateDirectory(Settings.ImagePath);}
        _foundedPictures.text += Directory.GetFiles(Settings.ImagePath).Length;
        Settings._textureCache = new List<Texture2D>();
    }

    public void OnTimeButtonSelect(int buttonID){
        Settings.SelectedTime = times[buttonID];
        for(int i = 0; i < TimeButtons.Length; i++) {
            TimeButtons[i].SetActive(buttonID != i);
        }
    }

    public void OnNumberOfPictureButtonSelect(int buttonID){
        Settings.NumberOfPicture = numbers[buttonID];
        for(int i = 0; i < NumberButtons.Length; i++) {
            NumberButtons[i].SetActive(buttonID != i);
        }
    }

    private string GetTimeFormated(int timeTableId){
        int time = times[timeTableId];
        return ((int)(time/60)).ToString() + ":" + ((int)(time%60)).ToString().PadLeft(2,'0');
    }

    public void OnContinueButton(){
        SceneManager.LoadScene(2);
    }
}
