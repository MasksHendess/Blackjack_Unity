using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
    public string color;
    public int value;
    public SpriteRenderer SpriteRenderer;
    public string text;
    // Start is called before the first frame update
    void Start()
    {
        switch (value)
        {
            case 1:
                text = "Ace of " + color;
                break;
            case 11:
                text = "Jack of " + color;
                break;
            case 12:
               text = "Queen of " + color;
                break;
            case 13:
                text = "King of " + color;
                break;
            default:
                text = value.ToString() + " " + color;
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
    }
}
