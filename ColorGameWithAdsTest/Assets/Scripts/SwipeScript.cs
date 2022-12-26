using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwipeScript : MonoBehaviour
{

    float startX;
    float startY;
    float currentX;
    float currentY;
    bool clickDown = false;

    [SerializeField] ColorMerging gameBoard;

    public void SetStartPosition() 
    {
        if(gameBoard.CheckIfGameEnd())
        {
            return;
        }
        startX = Input.mousePosition.x;
        startY = Input.mousePosition.y;
        clickDown = true;
    }

    void Update()
    {
        if(!clickDown) return;
        float currentX = Input.mousePosition.x;
        float currentY = Input.mousePosition.y;
        if (Mathf.Abs(currentX - startX) > Mathf.Abs(currentY - startY))
        {
            if (currentX > startX)
            {
                for(int i=0;i<16;i++)
                {
                    if(gameBoard.transform.GetChild(i).childCount>0)
                    {
                        Transform square = gameBoard.transform.GetChild(i).GetChild(0);
                        if(i%4!=3)
                        {
                            square.localPosition = new Vector3(90*Mathf.Min(1,(currentX - startX)/(0.15f*Screen.width)),0,0);
                            square.GetComponent<RectTransform>().sizeDelta = new Vector2(160,160);
                        }
                        else
                        {
                            square.localPosition = new Vector3(30*Mathf.Min(1,(currentX - startX)/(0.15f*Screen.width)),0,0);
                            square.GetComponent<RectTransform>().sizeDelta = new Vector2(160-40*Mathf.Min(1,(currentX - startX)/(0.15f*Screen.width)),160);
                        }
                    }
                }
            }
            else
            {
                for(int i=0;i<16;i++)
                {
                    if(gameBoard.transform.GetChild(i).childCount>0)
                    {
                        Transform square = gameBoard.transform.GetChild(i).GetChild(0);
                        if(i%4!=0)
                        {
                            square.localPosition = new Vector3(-90*Mathf.Min(1,(startX - currentX)/(0.15f*Screen.width)),0,0);
                            square.GetComponent<RectTransform>().sizeDelta = new Vector2(160,160);
                        }
                        else
                        {
                            square.localPosition = new Vector3(-30*Mathf.Min(1,(startX - currentX)/(0.15f*Screen.width)),0,0);
                            square.GetComponent<RectTransform>().sizeDelta = new Vector2(160-40*Mathf.Min(1,(startX - currentX)/(0.15f*Screen.width)),160);
                        }
                    }
                }            
            }
        }
        else 
        {
            if (currentY > startY)
            {
                for(int i=0;i<16;i++)
                {
                    if(gameBoard.transform.GetChild(i).childCount>0)
                    {
                        Transform square = gameBoard.transform.GetChild(i).GetChild(0);
                        if(i<12)
                        {
                            square.localPosition = new Vector3(0,90*Mathf.Min(1,(currentY - startY)/(0.08f*Screen.height)),0);
                            square.GetComponent<RectTransform>().sizeDelta = new Vector2(160,160);
                        }
                        else
                        {
                            square.localPosition = new Vector3(0,30*Mathf.Min(1,(currentY - startY)/(0.08f*Screen.height)),0);
                            square.GetComponent<RectTransform>().sizeDelta = new Vector2(160,160-40*Mathf.Min(1,(currentY - startY)/(0.08f*Screen.height)));
                        }
                    }
                }    
            }
            else
            {
                for(int i=0;i<16;i++)
                {
                    if(gameBoard.transform.GetChild(i).childCount>0)
                    {
                        Transform square = gameBoard.transform.GetChild(i).GetChild(0);
                        if(i>=4)
                        {
                            square.localPosition = new Vector3(0,-90*Mathf.Min(1,(startY - currentY)/(0.08f*Screen.height)),0);
                            square.GetComponent<RectTransform>().sizeDelta = new Vector2(160,160);
                        }
                        else 
                        {
                            square.localPosition = new Vector3(0,-30*Mathf.Min(1,(startY - currentY)/(0.08f*Screen.height)),0);
                            square.GetComponent<RectTransform>().sizeDelta = new Vector2(160,160-40*Mathf.Min(1,(startY - currentY)/(0.08f*Screen.height)));
                        }
                    }
                }    
            }
        }
    }

    public void PerformSwipe() 
    {
        if(gameBoard.CheckIfGameEnd()) return;
        float currentX = Input.mousePosition.x;
        float currentY = Input.mousePosition.y;
        clickDown = false;
        RestoreSquareTransforms();

        //add a threshhold for a percentage of the screen
        if (Mathf.Abs(currentX - startX) > Mathf.Abs(currentY - startY))
        {
            if (currentX > startX)
            {
                if (currentX - startX <= 0.15f * Screen.width) return;
                gameBoard.Swipe("r");
            }
            else
            {
                if (startX - currentX <= 0.15f * Screen.width) return;
                gameBoard.Swipe("l");
            }
        }
        else 
        {
            if (currentY > startY)
            {
                if (currentY - startY<= 0.08f * Screen.height) return;
                gameBoard.Swipe("u");
            }
            else
            {
                if (startY - currentY <= 0.08f * Screen.height) return;
                gameBoard.Swipe("d");
            }
        }
    }

    void RestoreSquareTransforms()
    {
        for(int i=0;i<16;i++)
        {
            if(gameBoard.transform.GetChild(i).childCount>0)
            {
                Transform square = gameBoard.transform.GetChild(i).GetChild(0);
                square.localPosition = new Vector3(0,0,0);
                square.GetComponent<RectTransform>().sizeDelta = new Vector2(160,160);
            }
        }    
    }

}
