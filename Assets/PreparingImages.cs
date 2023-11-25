using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class PreparingImages : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _progress;

    void Update()
    {
        _progress.text = ImageParser.GetProgress();

        if(!ImageParser.IsProcessing()) SceneManager.LoadScene(1); 
    }
}
