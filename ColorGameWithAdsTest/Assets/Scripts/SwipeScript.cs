using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwipeScript : MonoBehaviour
{

    float startX;
    float startY;
    float currentX;
    float currentY;

    [SerializeField] ColorMerging gameBoard;

    public void SetStartPosition() 
    {
        startX = Input.mousePosition.x;
        startY = Input.mousePosition.y;
    }

    public void PerformSwipe() 
    {
        float currentX = Input.mousePosition.x;
        float currentY = Input.mousePosition.y;

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

}
