using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ColorMerging : MonoBehaviour
{

    GridLayoutGroup myGrid;
    [SerializeField] TextMeshProUGUI scoreText;
    [SerializeField] TextMeshProUGUI hisghscoreText;
    [SerializeField] Image nextTilePreviewImage;
    [SerializeField] Transform garbageCan;
    [SerializeField] GameObject gameManager;
    [SerializeField] GameObject[] tilePrefabs;
    [SerializeField] GameObject[] colorblindSignPrefabs;
    [SerializeField] int columns = 4;
    [SerializeField] int rows = 4;
    [SerializeField] bool allowSecondaryColorsAdded;
    GameObject blockToBeAdded;
    int movesPlayed = 0;
    bool noMoreAvailableMoves = false;
    int score = 0;
    List<int> totalScores;
    List<int> previousColorsAdded;
    bool colorblindModeOn;
    int gamesSinceLastAd = 0;
    int timeSinceLastAd = 0;
    // Start is called before the first frame update
    void Start()
    {
        myGrid = GetComponent<GridLayoutGroup>();
        totalScores = new List<int>();
        previousColorsAdded = new List<int>();
        colorblindModeOn = PlayerPrefs.GetInt("ColorblindOn",0)>0.5;
        //ClearBoard();
        SetupGame();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.R)) RestartGame();
        if(Input.GetKeyDown(KeyCode.P)) PlayYourself();
        if(Input.GetKeyDown(KeyCode.M)) PrintResults();
        if(Input.GetKeyDown(KeyCode.Q))
        {
            string sdp = "";
            for(int i=0; i<previousColorsAdded.Count; i++)
            {
                sdp+=previousColorsAdded[i]+", ";
            }
            print(sdp);
        }

        if(noMoreAvailableMoves) return;
        if(Input.GetKeyDown(KeyCode.RightArrow)) Swipe("r");
        if(Input.GetKeyDown(KeyCode.LeftArrow)) Swipe("l");
        if(Input.GetKeyDown(KeyCode.UpArrow)) Swipe("u");
        if(Input.GetKeyDown(KeyCode.DownArrow)) Swipe("d");
    }

    void PrintResults()
    {
        totalScores.Sort();
        print(totalScores[0] +" - "+ totalScores[totalScores.Count/2]+" - "+totalScores[totalScores.Count-1]);
        totalScores.Clear();
    }
    void PlayYourself()
    {
        for(int i=0;i<100;i++){
            ClearBoard();
            SetupGame();
            while(!noMoreAvailableMoves)
            {
                switch(Random.Range(0,4))
                {
                    case 0:
                        Swipe("r");
                        break;
                    case 1:
                        Swipe("l");
                        break;
                    case 2:
                        Swipe("u");
                        break;
                    case 3:
                        Swipe("d");
                        break;
                }
            }
        }
    }

    public void RestartGame()
    {
        ClearBoard();
        SetupGame();
    }

    IEnumerator CountSecondsSinceLastAd()
    {
        yield return new WaitForSeconds(1);
        timeSinceLastAd++;
    }

    void SetupGame()
    {
        if(gamesSinceLastAd>=3 || timeSinceLastAd>=160)
        {
            GameObject.Find("AdsManager").GetComponent<InterstitialAds>().ShowAd();
            gamesSinceLastAd = 0;
            timeSinceLastAd = 0;
        }
        StartCoroutine("CountSecondsSinceLastAd");
        movesPlayed = 0;
        scoreText.text = 0+"";
        hisghscoreText.enabled = false;
        score = 0;
        noMoreAvailableMoves = false;
        previousColorsAdded.Clear();
        nextTilePreviewImage.gameObject.SetActive(true);
        RandomizeNextColor();
        while(blockToBeAdded != tilePrefabs[0] && blockToBeAdded != tilePrefabs[1] && blockToBeAdded != tilePrefabs[2])//to make sure at the start of the game the first square will be a primary color
        {
            RandomizeNextColor();
        }
    }

    void ClearBoard()
    {
        for(int i=transform.childCount-1;i>=0;i--)
        {
            if(transform.GetChild(i).childCount>0) MoveToGarbage(transform.GetChild(i).GetChild(0));
        }
        ClearGarbage();
    }

    public void OnColorblindSettingChange()
    {
        colorblindModeOn = !colorblindModeOn;
        if(colorblindModeOn)
        {
            //add colorblind symbols to all squares
            Instantiate(GetGameObjectFromListByName(blockToBeAdded.GetComponent<TileScript>().GetColor()+""),nextTilePreviewImage.transform);
            for(int i=0; i<transform.childCount;i++)
            {
                if(transform.GetChild(i).childCount==1)
                    Instantiate(GetGameObjectFromListByName(transform.GetChild(i).GetChild(0).GetComponent<TileScript>().GetColor()+""),transform.GetChild(i).GetChild(0));
            }
        }else
        {
            //remove all the symbols if not colorblind
            if(nextTilePreviewImage.transform.childCount==1) MoveToGarbage(nextTilePreviewImage.transform.GetChild(0));
            for(int i=0; i<transform.childCount;i++)
            {
                if(transform.GetChild(i).childCount!=0)
                    if(transform.GetChild(i).GetChild(0).childCount!=0)
                        MoveToGarbage(transform.GetChild(i).GetChild(0).GetChild(0));
            }
        }
    }

    void MoveToGarbage(Transform t)
    {
        t.SetParent(garbageCan);
        t.localPosition = new Vector3 (0,0,0);
        t.localScale = new Vector3 (0,0,0);
    }

    void ClearGarbage()
    {
        for(int i=garbageCan.childCount-1;i>=0;i--)
        {
            Destroy(garbageCan.GetChild(i).gameObject);
        }
    }

    int GetSquareIndex(int r, int c)
    {
        return (r*columns + c);
    }

    public void Swipe(string direction)
    {
        int movesSoFar = movesPlayed;
        switch(direction)
        {
            case "r":
                for(int i=0; i<rows; i++)
                    for(int j=columns-2; j>=0; j--)
                        if(transform.GetChild(GetSquareIndex(i,j)).childCount>0)
                            if(AdjacentSquareFree(i,j, direction)) MoveBlock(GetSquareIndex(i,j), direction);
                            else if(CanMerge(GetSquareIndex(i,j), direction)) MergeBlock(GetSquareIndex(i,j), direction);
                            else if(CanCombine(GetSquareIndex(i,j), direction)) CombineBlock(GetSquareIndex(i,j), direction);
                break;
            case "l":
                for(int i=0; i<rows; i++)
                    for(int j=1; j<columns; j++)
                        if(transform.GetChild(GetSquareIndex(i,j)).childCount>0) 
                            if(AdjacentSquareFree(i,j, direction)) MoveBlock(GetSquareIndex(i,j), direction);
                            else if(CanMerge(GetSquareIndex(i,j), direction)) MergeBlock(GetSquareIndex(i,j), direction);
                            else if(CanCombine(GetSquareIndex(i,j), direction)) CombineBlock(GetSquareIndex(i,j), direction);
                break;
            case "u":
                for(int j=0; j<columns; j++)
                    for(int i=rows-2; i>=0; i--)
                        if(transform.GetChild(GetSquareIndex(i,j)).childCount>0)
                            if(AdjacentSquareFree(i,j, direction)) MoveBlock(GetSquareIndex(i,j), direction);
                            else if(CanMerge(GetSquareIndex(i,j), direction)) MergeBlock(GetSquareIndex(i,j), direction);
                            else if(CanCombine(GetSquareIndex(i,j), direction)) CombineBlock(GetSquareIndex(i,j), direction);
                break;
            case "d":
                for(int j=0; j<columns; j++)
                    for(int i=1; i<rows; i++)
                        if(transform.GetChild(GetSquareIndex(i,j)).childCount>0)
                            if(AdjacentSquareFree(i,j, direction)) MoveBlock(GetSquareIndex(i,j), direction);
                            else if(CanMerge(GetSquareIndex(i,j), direction)) MergeBlock(GetSquareIndex(i,j), direction);
                            else if(CanCombine(GetSquareIndex(i,j), direction)) CombineBlock(GetSquareIndex(i,j), direction);
                break;
        }

        AddBlock(direction);

        if(movesPlayed>movesSoFar)
        {
            movesPlayed = movesSoFar+1;
            scoreText.text = score+"";
            CenterAllBlocks();
            RandomizeNextColor();
        } 
        noMoreAvailableMoves = CheckIfGameEnd();
        if(noMoreAvailableMoves) { Vibrate(false); gamesSinceLastAd++; StopCoroutine("CountSecondsSinceLastAd");}//game ended so big vibration
        else Vibrate(true);
    }

    void Vibrate(bool small)
    {
        if(PlayerPrefs.GetInt("VibrationOn",1)>0.5f){
            if(small) gameManager.GetComponent<MainMenuBehaviour>().PlaySmallVibration();
            else gameManager.GetComponent<MainMenuBehaviour>().PlayBigVibration();
        }
    }
    bool AdjacentSquareFree(int r, int c, string direction)
    {
        switch(direction)
        {
            case "r":
                if(transform.GetChild(GetSquareIndex(r,c+1)).childCount==0)
                    return true;
                else return false;
            case "l":
                if(transform.GetChild(GetSquareIndex(r,c-1)).childCount==0)
                    return true;
                else return false;
            case "u":
                if(transform.GetChild(GetSquareIndex(r+1,c)).childCount==0)
                    return true;
                else return false;
            case "d":
                if(transform.GetChild(GetSquareIndex(r-1,c)).childCount==0)
                    return true;
                else return false;
            default:
                return false;
        }
    }

    void MoveBlock(int squareIndex, string direction)//after we have already made sure the target position is empty
    {
        switch(direction)
        {
            case "r":
                transform.GetChild(squareIndex).GetChild(0).SetParent(transform.GetChild(squareIndex+1));
                break;
            case "l":
                transform.GetChild(squareIndex).GetChild(0).SetParent(transform.GetChild(squareIndex-1));
                break;
            case "u":
                transform.GetChild(squareIndex).GetChild(0).SetParent(transform.GetChild(squareIndex+columns));
                break;
            case "d":
                transform.GetChild(squareIndex).GetChild(0).SetParent(transform.GetChild(squareIndex-columns));
                break;
        }
        movesPlayed++;
    }

    bool CanMerge(int squareIndex, string direction)
    {
        int color1 = transform.GetChild(squareIndex).GetChild(0).GetComponent<TileScript>().GetColor();
        int color2 = 0;

        switch(direction)
        {
            case "r":
                color2 = transform.GetChild(squareIndex+1).GetChild(0).GetComponent<TileScript>().GetColor();
                break;
            case "l":
                color2 = transform.GetChild(squareIndex-1).GetChild(0).GetComponent<TileScript>().GetColor();
                break;
            case "u":
                color2 = transform.GetChild(squareIndex+columns).GetChild(0).GetComponent<TileScript>().GetColor();
                break;
            case "d":
                color2 = transform.GetChild(squareIndex-columns).GetChild(0).GetComponent<TileScript>().GetColor();
                break;
        }
        if(color1 + color2 == 111)
            return true;
        return false;
    }

    void MergeBlock(int squareIndex, string direction)
    {
        switch(direction)
        {
            case "r":
                MoveToGarbage(transform.GetChild(squareIndex+1).GetChild(0));
                break;
            case "l":
                MoveToGarbage(transform.GetChild(squareIndex-1).GetChild(0));
                break;
            case "u":
                MoveToGarbage(transform.GetChild(squareIndex+columns).GetChild(0));
                break;
            case "d":
                MoveToGarbage(transform.GetChild(squareIndex-columns).GetChild(0));
                break;
        }
        MoveToGarbage(transform.GetChild(squareIndex).GetChild(0));
        movesPlayed++;
        score += 10;
        gameManager.GetComponent<SoundPlayer>().PlayDeleteClip();
    }
    bool CanCombine(int squareIndex, string direction)
    {
        int color1 = transform.GetChild(squareIndex).GetChild(0).GetComponent<TileScript>().GetColor();
        int color2 = 0;

        switch(direction)
        {
            case "r":
                color2 = transform.GetChild(squareIndex+1).GetChild(0).GetComponent<TileScript>().GetColor();
                break;
            case "l":
                color2 = transform.GetChild(squareIndex-1).GetChild(0).GetComponent<TileScript>().GetColor();
                break;
            case "u":
                color2 = transform.GetChild(squareIndex+columns).GetChild(0).GetComponent<TileScript>().GetColor();
                break;
            case "d":
                color2 = transform.GetChild(squareIndex-columns).GetChild(0).GetComponent<TileScript>().GetColor();
                break;
        }

        if((color1==1 || color1==10 || color1==100) && (color2==1 || color2==10 || color2==100) && color1!=color2)
            return true;
        return false;
    }
    void CombineBlock(int squareIndex, string direction)
    {

        int color1 = transform.GetChild(squareIndex).GetChild(0).GetComponent<TileScript>().GetColor();
        int color2 = 0;

        switch(direction)
        {
            case "r":
                color2 = transform.GetChild(squareIndex+1).GetChild(0).GetComponent<TileScript>().GetColor();
                transform.GetChild(squareIndex+1).GetChild(0).GetComponent<TileScript>().SetColor(color1 + color2);
                if(colorblindModeOn)
                {
                    MoveToGarbage(transform.GetChild(squareIndex+1).GetChild(0).GetChild(0));
                    Instantiate(GetGameObjectFromListByName(color1+color2+""), transform.GetChild(squareIndex+1).GetChild(0));
                }
                break;
            case "l":
                color2 = transform.GetChild(squareIndex-1).GetChild(0).GetComponent<TileScript>().GetColor();
                transform.GetChild(squareIndex-1).GetChild(0).GetComponent<TileScript>().SetColor(color1 + color2);
                if(colorblindModeOn)
                {
                    MoveToGarbage(transform.GetChild(squareIndex-1).GetChild(0).GetChild(0));
                    Instantiate(GetGameObjectFromListByName(color1+color2+""), transform.GetChild(squareIndex-1).GetChild(0));
                }
                break;
            case "u":
                color2 = transform.GetChild(squareIndex+columns).GetChild(0).GetComponent<TileScript>().GetColor();
                transform.GetChild(squareIndex+columns).GetChild(0).GetComponent<TileScript>().SetColor(color1 + color2);
                if(colorblindModeOn) 
                {
                    MoveToGarbage(transform.GetChild(squareIndex+columns).GetChild(0).GetChild(0));
                    Instantiate(GetGameObjectFromListByName(color1+color2+""), transform.GetChild(squareIndex+columns).GetChild(0));
                }  
                break;
            case "d":
                color2 = transform.GetChild(squareIndex-columns).GetChild(0).GetComponent<TileScript>().GetColor();
                transform.GetChild(squareIndex-columns).GetChild(0).GetComponent<TileScript>().SetColor(color1 + color2);
                if(colorblindModeOn)
                {
                    MoveToGarbage(transform.GetChild(squareIndex-columns).GetChild(0).GetChild(0));
                    Instantiate(GetGameObjectFromListByName(color1+color2+""), transform.GetChild(squareIndex-columns).GetChild(0));
                }
                break;
        }
        MoveToGarbage(transform.GetChild(squareIndex).GetChild(0));
        movesPlayed++;
        score += 4;
        gameManager.GetComponent<SoundPlayer>().PlayCombineClip();
    }

    GameObject GetGameObjectFromListByName(string name)
    {
        for(int i=0; i<colorblindSignPrefabs.Length; i++)
        {
            if(colorblindSignPrefabs[i].name == name) return colorblindSignPrefabs[i];
        }
        return null;
    }
    void AddBlock(string direction)
    {
        switch(direction)
        {
            case "r":
                if(!CheckIfColumnFull(0))
                {
                    if(colorblindModeOn)
                        Instantiate(nextTilePreviewImage.transform.GetChild(0),Instantiate(blockToBeAdded,transform.GetChild(GetFreeSquareIndexAtColumn(0))).transform);
                    else
                        Instantiate(blockToBeAdded,transform.GetChild(GetFreeSquareIndexAtColumn(0)));
                    movesPlayed++;
                }
                //else print("Column full");
                break;
            case "l":
                if(!CheckIfColumnFull(columns-1))
                {
                    if(colorblindModeOn)
                        Instantiate(nextTilePreviewImage.transform.GetChild(0),Instantiate(blockToBeAdded,transform.GetChild(GetFreeSquareIndexAtColumn(columns-1))).transform);
                    else
                        Instantiate(blockToBeAdded,transform.GetChild(GetFreeSquareIndexAtColumn(columns-1)));
                    movesPlayed++;
                }
                //else print("Column full");
                break;
            case "u":
                if(!CheckIfRowFull(0))
                {
                    if(colorblindModeOn)
                        Instantiate(nextTilePreviewImage.transform.GetChild(0),Instantiate(blockToBeAdded,transform.GetChild(GetFreeSquareIndexAtRow(0))).transform);
                    else
                        Instantiate(blockToBeAdded,transform.GetChild(GetFreeSquareIndexAtRow(0)));
                    movesPlayed++;
                }
                //else print("Row full");
                break;
            case "d":
                if(!CheckIfRowFull(rows-1))
                {
                    if(colorblindModeOn)
                        Instantiate(nextTilePreviewImage.transform.GetChild(0),Instantiate(blockToBeAdded,transform.GetChild(GetFreeSquareIndexAtRow(rows-1))).transform);
                    else
                        Instantiate(blockToBeAdded,transform.GetChild(GetFreeSquareIndexAtRow(rows-1)));
                    movesPlayed++;
                }
                //else print("Row full");
                break;
        }
        score += 1;
        gameManager.GetComponent<SoundPlayer>().PlaySwipeClip();
    }

    bool CheckIfRowFull(int r)
    {
        for(int i=0; i<columns; i++)
        {
            if(transform.GetChild(GetSquareIndex(r,i)).childCount==0) return false;
        }
        return true;
    }
    bool CheckIfColumnFull(int c)
    {
        for(int i=0; i<rows; i++)
        {
            if(transform.GetChild(GetSquareIndex(i,c)).childCount==0) return false;
        }
        return true;
    }

    int GetFreeSquareIndexAtRow(int r)
    {
        int index = Random.Range(0,columns);
        while(transform.GetChild(GetSquareIndex(r,index)).childCount!=0)
        {
            index = Random.Range(0,columns);
        }
        return GetSquareIndex(r,index);
    }
    int GetFreeSquareIndexAtColumn(int c)
    {
        int index = Random.Range(0,rows);
        while(transform.GetChild(GetSquareIndex(index,c)).childCount!=0)
        {
            index = Random.Range(0,rows);
        }
        return GetSquareIndex(index,c);
    }

    bool CheckIfGameEnd()
    {
        for(int i=0; i<rows*columns; i++)
        {
            if(transform.GetChild(i).childCount==0) return false;
        }
        //we check if the inner blocks can make a move in any direction
        for(int i=0; i<rows; i++)
        {
            for(int j=0; j<columns; j++)
            {
                if(j!=columns-1) if(CanMerge(GetSquareIndex(i,j),"r")) return false;
                if(j!=0) if(CanMerge(GetSquareIndex(i,j),"l")) return false;
                if(i!=rows-1) if(CanMerge(GetSquareIndex(i,j),"u")) return false;
                if(i!=0) if(CanMerge(GetSquareIndex(i,j),"d")) return false;
                if(j!=columns-1)if(CanCombine(GetSquareIndex(i,j),"r")) return false;
                if(j!=0) if(CanCombine(GetSquareIndex(i,j),"l")) return false;
                if(i!=rows-1) if(CanCombine(GetSquareIndex(i,j),"u")) return false;
                if(i!=0) if(CanCombine(GetSquareIndex(i,j),"d")) return false;
            }
        }
        //print("Game End "+movesPlayed);
        hisghscoreText.enabled = true;
        if(score>PlayerPrefs.GetInt("Highscore",0))
        {
            hisghscoreText.text = "NEW HIGHSCORE!";
            gameManager.GetComponent<SoundPlayer>().PlayHighscoreClip();
            PlayerPrefs.SetInt("Highscore",score);
        }else
        {
            hisghscoreText.text = "HIGHSCORE\n"+PlayerPrefs.GetInt("Highscore",0);
            gameManager.GetComponent<SoundPlayer>().PlayGameEndClip();
        }
        totalScores.Add(movesPlayed);
        nextTilePreviewImage.gameObject.SetActive(false);
        //also check if any merges can occur to return true then
        return true;
    }

    void CenterAllBlocks()
    {
        for(int i=0; i<rows*columns; i++)
            if(transform.GetChild(i).childCount>0) transform.GetChild(i).GetChild(0).localPosition = new Vector3(0,0,0);
    }

    void RandomizeNextColor()
    {
        int newColorCode = Random.Range(0,(allowSecondaryColorsAdded?15:3))%6;//primary colors have x1.5 the chances to be added
        
        while(!CheckIfNextColorValid(newColorCode))
        {
            newColorCode = Random.Range(0,(allowSecondaryColorsAdded?15:3))%6;   
        }
        
        previousColorsAdded.Add(newColorCode);

        if(previousColorsAdded.Count>10) previousColorsAdded.RemoveAt(0);
        blockToBeAdded = tilePrefabs[newColorCode];
        nextTilePreviewImage.color = blockToBeAdded.GetComponent<Image>().color;
        if(nextTilePreviewImage.transform.childCount==1) MoveToGarbage(nextTilePreviewImage.transform.GetChild(0));
        if(colorblindModeOn) Instantiate(GetGameObjectFromListByName(blockToBeAdded.GetComponent<TileScript>().GetColor()+""),nextTilePreviewImage.transform);
    }

    bool CheckIfNextColorValid(int newCol)
    {
        //no more than three same secondary in a row
        //no more than 4 same secondary in the last 10
        if(previousColorsAdded.Count>=3)
        {
            if(previousColorsAdded[previousColorsAdded.Count-1]==newCol & previousColorsAdded[previousColorsAdded.Count-2]==newCol && previousColorsAdded[previousColorsAdded.Count-3]==newCol)
                return false;
            if(previousColorsAdded.Count==10)
                if(FindNumberOfInstancesInList(previousColorsAdded,newCol)>=4) return false;
        }
        return true;
    }

    int FindNumberOfInstancesInList(List<int> l, int num)
    {
        int count = 0;
        for(int i=0; i<l.Count; i++)
            if(l[i]==num) count++;
        return count;
    }
}
