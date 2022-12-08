using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BoardScript : MonoBehaviour
{
    GridLayoutGroup myGrid;
    [SerializeField] GameObject garbageCan;
    [SerializeField] GameObject nextTilesPreviewParent;
    [SerializeField] GameObject[] tiles;
    [SerializeField] GameObject scoreText;
    [SerializeField] Toggle onlyPrimaryColorsToggle;
    [SerializeField] Toggle allowsDivideToggle;
    [SerializeField] TMP_InputField numOfColorsToAddEachRoundText;
    [SerializeField] Toggle fiveXfiveGridToggle;

    bool onlyPrimaryColors=false;
    bool allowsDivide=false;
    bool fourColorBatches=false;
    bool fiveXfiveGrid=false;
    int[] nextTileIndexes = new int[5];
    bool[] omitTileIndexesHorizontal = new bool[5];
    bool[] omitTileIndexesVertical = new bool[5];
    int movesPlayed = 0;
    bool gameEnded = false;    
    int rows;
    int columns;
    bool horizontalMovementPossible = true;
    bool verticalMovementPossible = true;

    // Start is called before the first frame update
    void Start()
    {
        myGrid = GetComponent<GridLayoutGroup>();
        //SetUpBoard(); commented out because the game starts without text in numOfColorsToAddEachRoundText and it shows an error
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R)) SetUpBoard();
        if (gameEnded) return;

        if (Input.GetKeyDown(KeyCode.RightArrow)) SwipeRight();
        if (Input.GetKeyDown(KeyCode.LeftArrow)) SwipeLeft();
        if (Input.GetKeyDown(KeyCode.UpArrow)) SwipeUp();
        if (Input.GetKeyDown(KeyCode.DownArrow)) SwipeDown();
    }

    public void SetUpBoard()
    {
        ClearBoard();
        onlyPrimaryColors = onlyPrimaryColorsToggle.isOn;
        allowsDivide = allowsDivideToggle.isOn;
        fiveXfiveGrid = fiveXfiveGridToggle.isOn;
        rows = fiveXfiveGrid?5:4;
        columns = fiveXfiveGrid?5:4;
        ChangeBoardSize();
        RandomizeNextTiles();
        movesPlayed = 0;
        SetScoreText();
        gameEnded = false;
        horizontalMovementPossible = true;
        verticalMovementPossible = true;
    }

    void ClearBoard()
    {
        for(int i=rows*columns-1; i>=0; i--)
        {
            if(transform.GetChild(i).childCount>0)
                MoveToGarbageCan(transform.GetChild(i).GetChild(0).gameObject);
        }
        ClearGarbageCan();
    }
    void MoveToGarbageCan(GameObject gObject) 
    {
        gObject.transform.SetParent(garbageCan.transform);
        gObject.SetActive(false);
    }

    void ClearGarbageCan() 
    {
        for(int i=garbageCan.transform.childCount-1; i>=0; i--) 
        {
            Destroy(garbageCan.transform.GetChild(i).gameObject);
        }
    }

    public void ChangeBoardSize()
    {
        myGrid.constraintCount = fiveXfiveGrid?5:4;
        while(transform.childCount>(fiveXfiveGrid?25:16))
        {
            MoveToGarbageCan(transform.GetChild(0).gameObject);
        }
        while(transform.childCount<(fiveXfiveGrid?25:16))
        {
            Instantiate(transform.GetChild(0),transform);
        }
        for(int i=0; i<4; i++)
        {
            nextTilesPreviewParent.transform.GetChild(i).GetChild(4).gameObject.SetActive(fiveXfiveGrid);
        }
    }

    void SetScoreText(){scoreText.GetComponent<TextMeshProUGUI>().text = movesPlayed+"";}
    public void SetOnlyPrimaryColors(bool o){onlyPrimaryColors=o;}
    void RandomizeNextTiles()
    {
        if(!onlyPrimaryColors)
        {
            nextTileIndexes[0] = Random.Range(0,9)%6;//so that primary colors (0-2) have twice the chance to appear
            nextTileIndexes[1] = Random.Range(0,9)%6;
            nextTileIndexes[2] = Random.Range(0,9)%6;
            nextTileIndexes[3] = Random.Range(0,9)%6;
            nextTileIndexes[4] = Random.Range(0,9)%6;
        }else
        {
            nextTileIndexes[0] = Random.Range(0,3);
            nextTileIndexes[1] = Random.Range(0,3);
            nextTileIndexes[2] = Random.Range(0,3);
            nextTileIndexes[3] = Random.Range(0,3);
            nextTileIndexes[4] = Random.Range(0,3);
        }

        for(int i = 0; i<5; i++)
        {
            omitTileIndexesHorizontal[i] = true;
        }
        for(int i = 0; i<5; i++)
        {
            omitTileIndexesVertical[i] = true;
        }

        for(int i = 0; i<int.Parse(numOfColorsToAddEachRoundText.text); i++)
        {
            int r = Random.Range(0,fiveXfiveGrid?5:4);
            if(int.Parse(numOfColorsToAddEachRoundText.text)==1 && !CheckBoardFull())
            while(!omitTileIndexesHorizontal[r] || CheckIfRowFull(r))//for >1  you may only have 1 free space left and the game hasn't ended, so infinite loop
            {
                r = Random.Range(0,fiveXfiveGrid?5:4);            
            }
            omitTileIndexesHorizontal[r] = false;
        }
        for(int i = 0; i<int.Parse(numOfColorsToAddEachRoundText.text); i++)
        {
            int r = Random.Range(0,fiveXfiveGrid?5:4);
            if(int.Parse(numOfColorsToAddEachRoundText.text)==1 && !CheckBoardFull())//for when board is full can still be manipulated
            while(!omitTileIndexesVertical[r] || CheckIfColumnFull(r))//for >1  you may only have 1 free space left and the game hasn't ended, so infinite loop
            {
                r = Random.Range(0,fiveXfiveGrid?5:4);            
            }
            omitTileIndexesVertical[r] = false;
        }
        VisualizeNextTiles();
    }
    bool CheckBoardFull()
    {
        for(int i=0; i<transform.childCount; i++)
        {
            if(transform.GetChild(0).childCount==0) return false;
        }
        return true;
    }
    void VisualizeNextTiles()
    {
        for(int i=0; i<4; i++)
        {
            for(int j=0; j<(fiveXfiveGrid?5:4); j++)
            {
                if((i<2 && !omitTileIndexesHorizontal[j]) || (i>=2 && !omitTileIndexesVertical[j]))
                {
                    //nextTilesPreviewParent.transform.GetChild(i).GetChild(j).GetComponent<Image>().enabled = true;
                    if(i==1)print("Horizontal: "+j);else if(i==3) print("Vertical: "+j);
                    nextTilesPreviewParent.transform.GetChild(i).GetChild(j).GetComponent<Image>().color = tiles[nextTileIndexes[j]].GetComponent<Image>().color;
                }else
                {
                    //nextTilesPreviewParent.transform.GetChild(i).GetChild(j).GetComponent<Image>().enabled = false;
                    nextTilesPreviewParent.transform.GetChild(i).GetChild(j).GetComponent<Image>().color = new Color(0.12f,0.12f,0.12f,1.0f);
                }
            }
        }
    }
    void ResetJustChangedStates()
    {
        for(int i=0; i<rows*columns; i++)
        {
            if(transform.GetChild(i).childCount>0)
            transform.GetChild(i).GetChild(0).GetComponent<TileScript>().justChangedState = false;
        }
    }
    int GetIndexFromGridPosition(int row, int column) { return row*columns + column; }
    void MoveToNewPosition(int previousRow, int previousCol, int newRow, int newCol)
    {
        GameObject tileToMoveParent = transform.GetChild(GetIndexFromGridPosition(previousRow, previousCol)).gameObject;
        GameObject tileToMove = tileToMoveParent.transform.GetChild(tileToMoveParent.transform.childCount-1).gameObject;
        tileToMove.transform.SetParent(transform.GetChild(GetIndexFromGridPosition(newRow,newCol)));
        tileToMove.transform.localPosition = new Vector3(0,0,0);
    }
    bool CanMerge(Transform tile1, Transform tile2)
    {
        return CanMerge(tile1.GetChild(0).GetComponent<TileScript>().GetColor(),tile2.GetChild(0).GetComponent<TileScript>().GetColor());
    }
    bool CanMerge(int colorCode1, int colorCode2)
    {
        //print(colorCode1+" - "+colorCode2);
        //print("Merged");
        if(colorCode1 + colorCode2 == 111)
            return true;
        return false;
    }
    bool CanCombine(Transform tile1, Transform tile2)
    {
        return CanCombine(tile1.GetChild(0).GetComponent<TileScript>().GetColor(),tile2.GetChild(0).GetComponent<TileScript>().GetColor());
    }
    bool CanCombine(int colorCode1, int colorCode2)
    {
        if((colorCode1==1 || colorCode1==10 || colorCode1==100) && (colorCode2==1 || colorCode2==10 || colorCode2==100) && colorCode1!=colorCode2)
            return true;
        return false;
    }
    bool CanDivide(int colorCode1, int colorCode2)
    {
        if(!allowsDivide) return false;
        if(Mathf.Abs(colorCode1-colorCode2)==1 || Mathf.Abs(colorCode1-colorCode2)==10 || Mathf.Abs(colorCode1-colorCode2)==100)
            return true;
        return false;
    }
    int CheckFreeSpaceInRow(int rowIndex, bool rightSwipe)
    {
        int freeSpaceIndex = -1;
        if(rightSwipe)
        {
            for(int i=columns-1; i>=0; i--)
            {
                if(transform.GetChild(GetIndexFromGridPosition(rowIndex,i)).childCount==0)
                {
                    freeSpaceIndex = i;
                    break;
                }
            }
        }else
        {
            for(int i=0; i<columns; i++)
            {
                if(transform.GetChild(GetIndexFromGridPosition(rowIndex,i)).childCount==0)
                {
                    freeSpaceIndex = i;
                    break;
                }
            }
        }
        //print(rowIndex+", "+freeSpaceIndex);
        return freeSpaceIndex;
    }
    int CheckFreeSpaceInColumn(int columnIndex, bool upSwipe)
    {
        int freeSpaceIndex = -1;
        if(upSwipe)
        {
            for(int i=rows-1; i>=0; i--)
            {
                if(transform.GetChild(GetIndexFromGridPosition(i,columnIndex)).childCount==0)
                {
                    freeSpaceIndex = i;
                    break;
                }
            }            
        }else
        {
            for(int i=0; i<rows; i++)
            {
                if(transform.GetChild(GetIndexFromGridPosition(i,columnIndex)).childCount==0)
                {
                    freeSpaceIndex = i;
                    break;
                }
            }
        }
        return freeSpaceIndex;
    }
    public void SwipeRight()
    {
        if(!horizontalMovementPossible) return;
        TileScript objectToBeMoved;
        Transform currentTile;

        Transform examinedTile;

        for(int i=0; i<rows; i++)
        {            
            //counter makes sure there are no D-A after C-B matches in a A-B-C-D chain, meaning two outer squares can't interact
            //if there existed two middle ones that merged and disappeared
            int counter = 0;
            for(int j=columns-1; j>=0; j--)
            {
                counter++;
                if(transform.GetChild(GetIndexFromGridPosition(i,j)).childCount > 0)
                {
                    currentTile = transform.GetChild(GetIndexFromGridPosition(i,j));
                    objectToBeMoved = currentTile.GetChild(currentTile.childCount-1).GetComponent<TileScript>();
                    //Search tiles to the right of this one for available options until you have to stop
                    for(int t=j+1; t<=columns; t++)
                    {
                        if(t==columns)//an pera apo to telos ths grammhs
                        {
                            MoveToNewPosition(i,j,i,t-1);
                            break;
                        }
                        examinedTile = transform.GetChild(GetIndexFromGridPosition(i,t));
                        if(examinedTile.childCount != 0)
                        {
                            if(!examinedTile.transform.GetChild(0).GetComponent<TileScript>().justChangedState)

                            if(CanMerge(examinedTile.GetChild(0).GetComponent<TileScript>().GetColor(),objectToBeMoved.GetColor()) && counter!=-2)
                            {
                                MoveToGarbageCan(examinedTile.GetChild(0).gameObject);
                                MoveToGarbageCan(objectToBeMoved.gameObject);
                                counter=-counter;
                                break;
                            }else if(CanCombine(examinedTile.GetChild(0).GetComponent<TileScript>().GetColor(),objectToBeMoved.GetColor()) && counter!=-2)
                            {
                                examinedTile.GetChild(0).GetComponent<TileScript>().SetColor(examinedTile.GetChild(0).GetComponent<TileScript>().GetColor()+objectToBeMoved.GetColor());
                                MoveToGarbageCan(objectToBeMoved.gameObject);
                                break;
                            }else if(CanDivide(examinedTile.GetChild(0).GetComponent<TileScript>().GetColor(),objectToBeMoved.GetColor()))
                            {
                                examinedTile.GetChild(0).GetComponent<TileScript>().SetColor(Mathf.Abs(examinedTile.GetChild(0).GetComponent<TileScript>().GetColor()-objectToBeMoved.GetColor()));
                                MoveToGarbageCan(objectToBeMoved.gameObject);
                                break;
                            }
                            MoveToNewPosition(i,j,i,t-1); //if simply blocked by another
                            break;
                        }
                    }
                }
                if(j==0)
                {
                    int freeIndex = CheckFreeSpaceInRow(i, true);
                    if(!omitTileIndexesHorizontal[i])
                    {
                        if(freeIndex !=-1)
                        {
                            Instantiate(tiles[nextTileIndexes[i]],transform.GetChild(GetIndexFromGridPosition(i,freeIndex)));
                            //print(freeIndex + " " +GetIndexFromGridPosition(i, freeIndex));
                        }
                        //else{
                        //    gameEnded = true;
                        //}
                    }   
                }
            }
        }
        movesPlayed++;
        SetScoreText();
        ResetJustChangedStates();
        ClearGarbageCan();

        CheckIfGameEnd();
        if(!gameEnded) RandomizeNextTiles();

    }
    public void SwipeLeft()
    {
        if(!horizontalMovementPossible) return;
        
        TileScript objectToBeMoved;
        Transform currentTile;

        Transform examinedTile;

        for(int i=0; i<rows; i++)
        {
            int counter = 0;
            for(int j=0; j<columns; j++)
            {
                counter++;
                if(transform.GetChild(GetIndexFromGridPosition(i,j)).childCount > 0)
                {
                    currentTile = transform.GetChild(GetIndexFromGridPosition(i,j));
                    objectToBeMoved = currentTile.GetChild(currentTile.childCount-1).GetComponent<TileScript>();
                    for(int t=j-1; t>=-1; t--)
                    {
                        if(t==-1)
                        {
                            MoveToNewPosition(i,j,i,t+1);
                            break;
                        }
                        examinedTile = transform.GetChild(GetIndexFromGridPosition(i,t)).transform;
                        if(examinedTile.childCount != 0)
                        {
                            if(!examinedTile.transform.GetChild(0).GetComponent<TileScript>().justChangedState)

                            if(CanMerge(examinedTile.GetChild(0).GetComponent<TileScript>().GetColor(),objectToBeMoved.GetColor()) && counter!=-2)
                            {
                                MoveToGarbageCan(examinedTile.GetChild(0).gameObject);
                                MoveToGarbageCan(objectToBeMoved.gameObject);
                                counter=-counter;
                                break;
                            }else if(CanCombine(examinedTile.GetChild(0).GetComponent<TileScript>().GetColor(),objectToBeMoved.GetColor()) && counter!=-2)
                            {
                                examinedTile.GetChild(0).GetComponent<TileScript>().SetColor(examinedTile.GetChild(0).GetComponent<TileScript>().GetColor()+objectToBeMoved.GetColor());
                                MoveToGarbageCan(objectToBeMoved.gameObject);
                                break;
                            }else if(CanDivide(examinedTile.GetChild(0).GetComponent<TileScript>().GetColor(),objectToBeMoved.GetColor()))
                            {
                                examinedTile.GetChild(0).GetComponent<TileScript>().SetColor(Mathf.Abs(examinedTile.GetChild(0).GetComponent<TileScript>().GetColor()-objectToBeMoved.GetColor()));
                                MoveToGarbageCan(objectToBeMoved.gameObject);
                                break;
                            }
                            MoveToNewPosition(i,j,i,t+1);
                            break;
                        }
                    }
                }
                if(j==columns-1)
                {
                    int freeIndex = CheckFreeSpaceInRow(i, false);
                    if(!omitTileIndexesHorizontal[i])
                    {
                        if(freeIndex !=-1)
                        {
                            Instantiate(tiles[nextTileIndexes[i]],transform.GetChild(GetIndexFromGridPosition(i,freeIndex)));
                        }
                        //else{
                        //    gameEnded = true;
                        //}
                    }   
                }
            }
        }
        movesPlayed++;
        SetScoreText();
        ResetJustChangedStates();
        ClearGarbageCan();

        CheckIfGameEnd();
        if(!gameEnded) RandomizeNextTiles();
    }
    public void SwipeUp()
    {
        if(!verticalMovementPossible) return;

        TileScript objectToBeMoved;
        Transform currentTile;

        Transform examinedTile;

        for(int j=0; j<columns; j++)
        {
            int counter = 0;
            for(int i=rows-1; i>=0; i--)
            {
                counter++;
                if(transform.GetChild(GetIndexFromGridPosition(i,j)).childCount > 0)
                {
                    currentTile = transform.GetChild(GetIndexFromGridPosition(i,j));
                    objectToBeMoved = currentTile.GetChild(currentTile.childCount-1).GetComponent<TileScript>();
                    for(int t=i+1; t<=rows; t++)
                    {
                        if(t==rows)
                        {
                            MoveToNewPosition(i,j,t-1,j);
                            break;  
                        }
                        examinedTile = transform.GetChild(GetIndexFromGridPosition(t,j));
                        if(examinedTile.childCount != 0)
                        {
                            if(!examinedTile.transform.GetChild(0).GetComponent<TileScript>().justChangedState)

                            if(CanMerge(examinedTile.GetChild(0).GetComponent<TileScript>().GetColor(),objectToBeMoved.GetColor()) && counter!=-2)
                            {
                                MoveToGarbageCan(examinedTile.GetChild(0).gameObject);
                                MoveToGarbageCan(objectToBeMoved.gameObject);
                                counter=-counter;
                                break;
                            }else if(CanCombine(examinedTile.GetChild(0).GetComponent<TileScript>().GetColor(),objectToBeMoved.GetColor()) && counter!=-2)
                            {
                                examinedTile.GetChild(0).GetComponent<TileScript>().SetColor(examinedTile.GetChild(0).GetComponent<TileScript>().GetColor()+objectToBeMoved.GetColor());
                                MoveToGarbageCan(objectToBeMoved.gameObject);
                                break;
                            }else if(CanDivide(examinedTile.GetChild(0).GetComponent<TileScript>().GetColor(),objectToBeMoved.GetColor()))
                            {
                                examinedTile.GetChild(0).GetComponent<TileScript>().SetColor(Mathf.Abs(examinedTile.GetChild(0).GetComponent<TileScript>().GetColor()-objectToBeMoved.GetColor()));
                                MoveToGarbageCan(objectToBeMoved.gameObject);
                                break;
                            }
                            MoveToNewPosition(i,j,t-1,j);
                            break;
                        }    
                    }
                }
                if(i==0)
                {
                    int freeIndex = CheckFreeSpaceInColumn(j, true);
                    if(!omitTileIndexesVertical[j])
                    {
                        if(freeIndex !=-1)
                        {
                            Instantiate(tiles[nextTileIndexes[j]],transform.GetChild(GetIndexFromGridPosition(freeIndex,j)));
                        }
                        //else{
                        //    gameEnded = true;
                        //}
                    }   
                }
            }
        }
        movesPlayed++;
        SetScoreText();
        ResetJustChangedStates();
        ClearGarbageCan();

        CheckIfGameEnd();
        if(!gameEnded) RandomizeNextTiles();

    }
    public void SwipeDown()
    {
        if(!verticalMovementPossible) return;

        TileScript objectToBeMoved;
        Transform currentTile;

        Transform examinedTile;

        for(int j=0; j<columns; j++)
        {
            int counter = 0;
            for(int i=0; i<rows; i++)
            {
                counter++;
                if(transform.GetChild(GetIndexFromGridPosition(i,j)).childCount > 0)
                {
                    currentTile = transform.GetChild(GetIndexFromGridPosition(i,j));
                    objectToBeMoved = currentTile.GetChild(currentTile.childCount-1).GetComponent<TileScript>();
                    for(int t=i-1; t>=-1; t--)
                    {
                        if(t==-1)
                        {
                            MoveToNewPosition(i,j,t+1,j);
                            break;                           
                        }
                        examinedTile = transform.GetChild(GetIndexFromGridPosition(t,j));
                        if(examinedTile.childCount != 0)
                        {    
                            if(!examinedTile.transform.GetChild(0).GetComponent<TileScript>().justChangedState)

                            if(CanMerge(examinedTile.GetChild(0).GetComponent<TileScript>().GetColor(),objectToBeMoved.GetColor()) && counter!=-2)
                            {
                                MoveToGarbageCan(examinedTile.GetChild(0).gameObject);
                                MoveToGarbageCan(objectToBeMoved.gameObject);
                                counter=-counter;
                                break;
                            }else if(CanCombine(examinedTile.GetChild(0).GetComponent<TileScript>().GetColor(),objectToBeMoved.GetColor()) && counter!=-2)
                            {
                                examinedTile.GetChild(0).GetComponent<TileScript>().SetColor(examinedTile.GetChild(0).GetComponent<TileScript>().GetColor()+objectToBeMoved.GetColor());
                                MoveToGarbageCan(objectToBeMoved.gameObject);
                                break;
                            }else if(CanDivide(examinedTile.GetChild(0).GetComponent<TileScript>().GetColor(),objectToBeMoved.GetColor()))
                            {
                                examinedTile.GetChild(0).GetComponent<TileScript>().SetColor(Mathf.Abs(examinedTile.GetChild(0).GetComponent<TileScript>().GetColor()-objectToBeMoved.GetColor()));
                                MoveToGarbageCan(objectToBeMoved.gameObject);
                                break;
                            }
                            MoveToNewPosition(i,j,t+1,j);
                            break;
                        }
                    }
                }
                if(i==rows-1)
                {
                    int freeIndex = CheckFreeSpaceInColumn(j,false);
                    if(!omitTileIndexesVertical[j])
                    {
                        if(freeIndex !=-1)
                        {
                            Instantiate(tiles[nextTileIndexes[j]],transform.GetChild(GetIndexFromGridPosition(freeIndex,j)));
                        }
                        //else{
                        //    gameEnded = true;
                        //}
                    }   
                }
            }
        }
        movesPlayed++;
        SetScoreText();
        ResetJustChangedStates();
        ClearGarbageCan();

        CheckIfGameEnd();
        if(!gameEnded) RandomizeNextTiles();

    }

    void CheckIfGameEnd()
    {
        horizontalMovementPossible = CheckIfHorizontalMovementPossible();
        verticalMovementPossible = CheckIfVerticalMovementPossible();

        if(!horizontalMovementPossible && !verticalMovementPossible) {
            gameEnded = true;        
        }
    }
    bool CheckIfRowFull(int rowIndex)
    {
        for(int i=0; i<columns;i++)
        {
            if(transform.GetChild(GetIndexFromGridPosition(rowIndex,i)).childCount==0)  return false;
        }
        return true;
    }
    bool CheckIfColumnFull(int columnIndex)
    {
        for(int i=0; i<rows;i++)
        {
            if(transform.GetChild(GetIndexFromGridPosition(i,columnIndex)).childCount==0)  return false;
        }
        return true;
    }
    bool CheckIfRowCanChange(int rowIndex)
    {
        for(int i=0; i<columns-1; i++)//we already know that the row is full
        {
            if(CanMerge(transform.GetChild(GetIndexFromGridPosition(rowIndex,i)),transform.GetChild(GetIndexFromGridPosition(rowIndex,i+1)))) return true;
            if(CanCombine(transform.GetChild(GetIndexFromGridPosition(rowIndex,i)),transform.GetChild(GetIndexFromGridPosition(rowIndex,i+1)))) return true;
        }
        return false;
    }
    bool CheckIfColumnCanChange(int columnIndex)
    {
        for(int i=0; i<rows-1; i++)//we already know that the column is full
        {
            if(CanMerge(transform.GetChild(GetIndexFromGridPosition(i,columnIndex)),transform.GetChild(GetIndexFromGridPosition(i+1,columnIndex)))) return true;
            if(CanCombine(transform.GetChild(GetIndexFromGridPosition(i,columnIndex)),transform.GetChild(GetIndexFromGridPosition(i+1,columnIndex)))) return true;
        }
        return false;
    }
    bool CheckIfHorizontalMovementPossible()
    {
        //return if all rows that would get changed only have 3 or less spots filled
        List<int> fullRowIndexes = new List<int>();
        for(int i=0; i<rows; i++)
        {
            if(!omitTileIndexesHorizontal[i])
            {
                if(CheckIfRowFull(i)) fullRowIndexes.Add(i);
            }
        }
        if(fullRowIndexes.Count==0)
        {
            //print("horizontalMovementPossible");
            return true;
        }

        //return if from those rows there can be at least one merge/disappearance
        for(int i=0; i<fullRowIndexes.Count; i++)
        {
            if(!CheckIfRowCanChange(fullRowIndexes[i])) return false;
        }
        //print("horizontalMovementPossible");
        return true;
    }
    bool CheckIfVerticalMovementPossible()
    {
        //return if all rows that would get changed only have 3 or less spots filled
        List<int> fullColumnIndexes = new List<int>();
        for(int i=0; i<columns; i++)
        {
            if(!omitTileIndexesVertical[i])
            {
                if(CheckIfColumnFull(i)) fullColumnIndexes.Add(i);
            }
        }
        if(fullColumnIndexes.Count==0)
        {
            //print("verticalMovementPossible");
            return true;
        }

        //return if from those rows there can be at least one merge/disappearance
        for(int i=0; i<fullColumnIndexes.Count; i++)
        {
            if(!CheckIfColumnCanChange(fullColumnIndexes[i])) return false;
        }
        //print("verticalMovementPossible");
        return true;
    }

}
