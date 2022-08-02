using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TileScript : MonoBehaviour
{
    [SerializeField] bool red;
    [SerializeField] bool blue;
    [SerializeField] bool yellow;
    public bool justChangedState;

    void SetColor(bool r, bool b, bool y)
    {
        red = r;
        blue = b;
        yellow = y;
    }

    public int GetColor()
    {
        int toReturn = 0;
        if(red) toReturn+=100;
        if(blue) toReturn+=10;
        if(yellow) toReturn+=1;
        return toReturn;
    }

    public void SetColor(int colorCode)
    {
        switch(colorCode)
        {
            case 100://red
                red = true;
                blue = false;
                yellow = false;
                GetComponent<Image>().color = new Color32(255,30,30,255);
                break;
            case 10://blue
                red = false;
                blue = true;
                yellow = false;
                GetComponent<Image>().color = new Color32(30,145,255,255);
                break;
            case 1://yellow
                red = false;
                blue = false;
                yellow = true;
                GetComponent<Image>().color = new Color32(240,255,30,255);
                break;
            case 110://purple
                red = true;
                blue = true;
                yellow = false;
                GetComponent<Image>().color = new Color32(130,60,220,255);
                break;
            case 101://orange
                red = true;
                blue = false;
                yellow = true;
                GetComponent<Image>().color = new Color32(255,133,30,255);
                break;
            case 11://green
                red = false;
                blue = true;
                yellow = true;
                GetComponent<Image>().color = new Color32(45,220,50,255);
                break;
        }
        justChangedState = true;
    }
}
