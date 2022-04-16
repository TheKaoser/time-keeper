using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class LevelGenerator : MonoBehaviour
{
    public MainMenu mainMenu;

    float lastAngle = 0;
    public GameObject gearPrefab;
    public int score;

    public Text scoreNumber;
    
    public List <Gear> gears = new List<Gear>();

    public PlayerMovement playerMovement;

    public void Start()
    {
        mainMenu.HideTutorial();
        mainMenu.ShowMenu();
        playerMovement.ResetPlayer();
        Camera.main.GetComponent<AudioSource>().Pause();
        StartCoroutine(BeginRun());
    }

    IEnumerator BeginRun()
    {
        foreach (Gear gear in gears)
        {
            Destroy(gear.transform.parent.gameObject);
        }
        gears.Clear();
        GenerateFirstGear();
        yield return new WaitUntil(() => Input.anyKeyDown);
        Camera.main.GetComponent<AudioSource>().Play();
        score = 0;
        scoreNumber.text = ToRoman(1);
        mainMenu.HideMenu();
        mainMenu.ShowTutorial();
        StartCoroutine(GenerateOtherGears());
    }

    void GenerateFirstGear()
    {
        Gear newGear = CreateGear();
        newGear.FirstGear();
        playerMovement.AssignGear(newGear);
        newGear.DecidePositionFromOtherGear(null);
    }

    IEnumerator GenerateOtherGears()
    {
        int i=0;
        do
        {
            Gear newGear = CreateGear();
            newGear.RandomizeGear();
            
            Gear selectedConnectedGearParent;
            do 
            {
                selectedConnectedGearParent = gears[UnityEngine.Random.Range(0, gears.Count - 1)];
            }
            while (selectedConnectedGearParent.hasChild);
            newGear.DecidePositionFromOtherGear(selectedConnectedGearParent);
            newGear.DecideRotationFromOtherGear(selectedConnectedGearParent);
            int currentScore = score;
            i++;
            if (i > 2)
                yield return new WaitUntil(() => score == currentScore + 1);
        } while (true);
    }

    float CalculateTotalAngle(float wedgeAngle)
    {
        return lastAngle + wedgeAngle;
    }

    Gear CreateGear()
    {
        Gear newGear = GameObject.Instantiate(gearPrefab).GetComponentInChildren<Gear>();
        gears.Add(newGear);
        return newGear;
    }

    public Gear NextGear()
    {
        if (gears.Count != 1)
        {
            if (score == 0)
            {
                mainMenu.HideTutorial();
            }
            score++;
            scoreNumber.text = ToRoman(score + 1);
            return gears[score];
        }
        else
        {
            return gears[0];
        }
    }

    public Gear NeareastGearFromPlayer()
    {
        float minDistance = 10000;
        float currentDistance;
        Gear closestGear = gears[0];
        foreach (Gear currentGear in gears)
        {
            currentDistance = Vector2.Distance(currentGear.transform.parent.position, playerMovement.transform.position);
            if (currentDistance < minDistance)
            {
                minDistance = currentDistance;
                closestGear = currentGear;
            }
        }
        return closestGear;
    }

    public static string ToRoman(int number)
    {
        if ((number < 0) || (number > 3999)) throw new ArgumentOutOfRangeException("insert value betwheen 1 and 3999");
        if (number < 1) return string.Empty;            
        if (number >= 1000) return "M" + ToRoman(number - 1000);
        if (number >= 900) return "CM" + ToRoman(number - 900); 
        if (number >= 500) return "D" + ToRoman(number - 500);
        if (number >= 400) return "CD" + ToRoman(number - 400);
        if (number >= 100) return "C" + ToRoman(number - 100);            
        if (number >= 90) return "XC" + ToRoman(number - 90);
        if (number >= 50) return "L" + ToRoman(number - 50);
        if (number >= 40) return "XL" + ToRoman(number - 40);
        if (number >= 10) return "X" + ToRoman(number - 10);
        if (number >= 9) return "IX" + ToRoman(number - 9);
        if (number >= 5) return "V" + ToRoman(number - 5);
        if (number >= 4) return "IV" + ToRoman(number - 4);
        if (number >= 1) return "I" + ToRoman(number - 1);
        throw new ArgumentOutOfRangeException("something bad happened");
    }
}