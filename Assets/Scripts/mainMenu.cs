using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public Image menuImage;
    public Text pressToContinue;
    public Text tutorial;
    public AudioSource audioSource;

    public void ShowMenu()
    {
        menuImage.enabled = true;
        pressToContinue.enabled = true;
        audioSource.Play();
    }

    public void HideMenu()
    {
        menuImage.enabled = false;
        pressToContinue.enabled = false;
        audioSource.Stop();
    }

    public void ShowTutorial()
    {
        tutorial.enabled = true;
    }
    
    public void HideTutorial()
    {
        tutorial.enabled = false;
    }
}
