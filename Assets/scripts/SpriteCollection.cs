using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteCollection : MonoBehaviour
{
    #region  setup Singelton pattern
    // only 1 instance of BuildManager in scene that is easy to acsess
    // Dont duplicate this region 
    public static SpriteCollection instance; //self reference
    private void Awake()
    {
        //check if instance already exisist
        if (instance != null)
        {
            Debug.LogError("More than one BuildManager in scene");
            return;
        }

        instance = this;
    }
    #endregion
    public List<Sprite> DiamondSprites;
    public List<Sprite> ClubsSprites;
    public List<Sprite> HeartsSprites;
    public List<Sprite> SpadesSprites;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public List<Sprite> getSprites(string color)
    {
        if (color == "Diamonds")
            return DiamondSprites;
        else if (color == "Clubs")
            return ClubsSprites;
        else if (color == "Hearts")
            return HeartsSprites;
        else if (color == "Spades")
            return SpadesSprites;
        else
            return null;
    }
}
