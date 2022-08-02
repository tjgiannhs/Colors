using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoardScript : MonoBehaviour
{
    GridLayoutGroup myGrid;
    [SerializeField] GameObject garbageCan;
    [SerializeField] GameObject nextTilesPreviewParent;
    [SerializeField] GameObject[] tiles;
    int[] nextTileIndexes = new int[5];
    int movesPlayed = 0;
    bool gameEnded = false;    
    int rows = 4;
    int columns = 4;

    // Start is called before the first frame update
    void Start()
    {
        myGrid = GetComponent<GridLayoutGroup>();
        SetUpBoard();
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

    void SetUpBoard()
    {
        ClearBoard();
        RandomizeNextTiles();
        movesPlayed = 0;
        gameEnded = false;
    }

    void ClearBoard()
    {
        for(int i=15; i>=0; i--)
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
    void RandomizeNextTiles()
    {
        nextTileIndexes[0] = Random.Range(0,6);
        nextTileIndexes[1] = Random.Range(0,6);
        nextTileIndexes[2] = Random.Range(0,6);
        nextTileIndexes[3] = Random.Range(0,6);

        nextTileIndexes[4] = Random.Range(0,4);//which space to ommit

        VisualizeNextTiles();
    }
    void VisualizeNextTiles()
    {
        for(int i=0; i<4; i++)
        {
            for(int j=0; j<4; j++)
            {
                if(j!=nextTileIndexes[4])
                {
                    //nextTilesPreviewParent.transform.GetChild(i).GetChild(j).GetComponent<Image>().enabled = true;
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
        for(int i=0; i<16; i++)
        {
            if(transform.GetChild(i).childCount>0)
            transform.GetChild(i).GetChild(0).GetComponent<TileScript>().justChangedState = false;
        }
    }
    int GetIndexFromGridPosition(int row, int column) { return row*4 + column; }
    void MoveToNewPosition(int previousRow, int previousCol, int newRow, int newCol)
    {
        GameObject tileToMoveParent = transform.GetChild(GetIndexFromGridPosition(previousRow, previousCol)).gameObject;
        GameObject tileToMove = tileToMoveParent.transform.GetChild(tileToMoveParent.transform.childCount-1).gameObject;
        tileToMove.transform.SetParent(transform.GetChild(GetIndexFromGridPosition(newRow,newCol)));
        tileToMove.transform.localPosition = new Vector3(0,0,0);
    }
    bool CanMerge(int colorCode1, int colorCode2)
    {
        print(colorCode1+" - "+colorCode2);
        if(colorCode1 + colorCode2 == 111)
            return true;
        return false;
    }
    bool CanCombine(int colorCode1, int colorCode2)
    {
        if((colorCode1==1 || colorCode1==10 || colorCode1==100) && (colorCode2==1 || colorCode2==10 || colorCode2==100))
            return true;
        return false;
    }
    bool CanDivide(int colorCode1, int colorCode2)
    {
        if(Mathf.Abs(colorCode1-colorCode2)==1 || Mathf.Abs(colorCode1-colorCode2)==10 || Mathf.Abs(colorCode1-colorCode2)==100)
            return true;
        return false;
    }
    int CheckFreeSpaceInRow(int rowIndex, bool rightSwipe)
    {
        int freeSpaceIndex = -1;
        if(rightSwipe)
        {
            for(int i=3; i>=0; i--)
            {
                if(transform.GetChild(GetIndexFromGridPosition(rowIndex,i)).childCount==0)
                {
                    freeSpaceIndex = i;
                    break;
                }
            }
        }else
        {
            for(int i=0; i<4; i++)
            {
                if(transform.GetChild(GetIndexFromGridPosition(rowIndex,i)).childCount==0)
                {
                    freeSpaceIndex = i;
                    break;
                }
            }
        }
        return freeSpaceIndex;
    }
    int CheckFreeSpaceInColumn(int columnIndex, bool upSwipe)
    {
        int freeSpaceIndex = -1;
        if(upSwipe)
        {
            for(int i=3; i>=0; i--)
            {
                if(transform.GetChild(GetIndexFromGridPosition(i,columnIndex)).childCount==0)
                {
                    freeSpaceIndex = i;
                    break;
                }
            }            
        }else
        {
            for(int i=0; i<4; i++)
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
    bool CheckIfMoveAllowed(bool horizontal)
    {
        
        for(int i=0; i<4; i++)
        {
            for(int j=0; j<4; j++)
            {

            }
        }
        return true;
    }
    void SwipeRight()
    {
        if(!CheckIfMoveAllowed(true)) return;

        TileScript objectToBeMoved;
        Transform currentTile;

        Transform examinedTile;

        for(int i=0; i<rows; i++)
        {
            for(int j=columns-1; j>=0; j--)
            {
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

                            if(CanMerge(examinedTile.GetChild(0).GetComponent<TileScript>().GetColor(),objectToBeMoved.GetColor()))
                            {
                                MoveToGarbageCan(examinedTile.GetChild(0).gameObject);
                                MoveToGarbageCan(objectToBeMoved.gameObject);
                                break;
                            }else if(CanCombine(examinedTile.GetChild(0).GetComponent<TileScript>().GetColor(),objectToBeMoved.GetColor()))
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
                    if(nextTileIndexes[4]!=i)
                    {
                        if(freeIndex !=-1)
                        {
                            Instantiate(tiles[nextTileIndexes[i]],transform.GetChild(GetIndexFromGridPosition(i,freeIndex)));
                        }else{
                            gameEnded = true;
                        }
                    }   
                }
            }
        }
        movesPlayed++;
        ResetJustChangedStates();
        RandomizeNextTiles();
        ClearGarbageCan();
    }
    void SwipeLeft()
    {
        if(!CheckIfMoveAllowed(true)) return;
        
        TileScript objectToBeMoved;
        Transform currentTile;

        Transform examinedTile;

        for(int i=0; i<rows; i++)
        {
            for(int j=0; j<columns; j++)
            {
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

                            if(CanMerge(examinedTile.GetChild(0).GetComponent<TileScript>().GetColor(),objectToBeMoved.GetColor()))
                            {
                                MoveToGarbageCan(examinedTile.GetChild(0).gameObject);
                                MoveToGarbageCan(objectToBeMoved.gameObject);
                                break;
                            }else if(CanCombine(examinedTile.GetChild(0).GetComponent<TileScript>().GetColor(),objectToBeMoved.GetColor()))
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
                    if(nextTileIndexes[4]!=i)
                    {
                        if(freeIndex !=-1)
                        {
                            Instantiate(tiles[nextTileIndexes[i]],transform.GetChild(GetIndexFromGridPosition(i,freeIndex)));
                        }else{
                            gameEnded = true;
                        }
                    }   
                }
            }
        }
        movesPlayed++;
        ResetJustChangedStates();
        RandomizeNextTiles();
        ClearGarbageCan();
    }
    void SwipeUp()
    {
        if(!CheckIfMoveAllowed(false)) return;

        TileScript objectToBeMoved;
        Transform currentTile;

        Transform examinedTile;

        for(int j=0; j<columns; j++)
        {
            for(int i=rows-1; i>=0; i--)
            {
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

                            if(CanMerge(examinedTile.GetChild(0).GetComponent<TileScript>().GetColor(),objectToBeMoved.GetColor()))
                            {
                                MoveToGarbageCan(examinedTile.GetChild(0).gameObject);
                                MoveToGarbageCan(objectToBeMoved.gameObject);
                                break;
                            }else if(CanCombine(examinedTile.GetChild(0).GetComponent<TileScript>().GetColor(),objectToBeMoved.GetColor()))
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
                    if(nextTileIndexes[4]!=j)
                    {
                        if(freeIndex !=-1)
                        {
                            Instantiate(tiles[nextTileIndexes[j]],transform.GetChild(GetIndexFromGridPosition(freeIndex,j)));
                        }else{
                            gameEnded = true;
                        }
                    }   
                }
            }
        }
        movesPlayed++;
        ResetJustChangedStates();
        RandomizeNextTiles();
        ClearGarbageCan();
    }
    void SwipeDown()
    {
        if(!CheckIfMoveAllowed(false)) return;

        TileScript objectToBeMoved;
        Transform currentTile;

        Transform examinedTile;

        for(int j=0; j<columns; j++)
        {
            for(int i=0; i<rows; i++)
            {
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

                            if(CanMerge(examinedTile.GetChild(0).GetComponent<TileScript>().GetColor(),objectToBeMoved.GetColor()))
                            {
                                MoveToGarbageCan(examinedTile.GetChild(0).gameObject);
                                MoveToGarbageCan(objectToBeMoved.gameObject);
                                break;
                            }else if(CanCombine(examinedTile.GetChild(0).GetComponent<TileScript>().GetColor(),objectToBeMoved.GetColor()))
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
                    if(nextTileIndexes[4]!=j)
                    {
                        if(freeIndex !=-1)
                        {
                            Instantiate(tiles[nextTileIndexes[j]],transform.GetChild(GetIndexFromGridPosition(freeIndex,j)));
                        }else{
                            gameEnded = true;
                        }
                    }   
                }
            }
        }
        movesPlayed++;
        ResetJustChangedStates();
        RandomizeNextTiles();
        ClearGarbageCan();
    }

}
