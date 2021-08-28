using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameTable : MonoBehaviour
{
    public GameObject whiteStone;
    public GameObject blackStone;
    public GameObject pickupAreaWhiteStone;
    public GameObject pickupAreaBlackStone;

    public GameObject[] PlaceToCheckFrom_LeftTop;
    public GameObject[] PlaceToCheckFrom_RightBottom;
    public GameObject[] PlaceToCheckFrom_InnerCenters;

    public GameObject WhiteParticles;
    public GameObject BlackParticles;
    public GameObject VictoryMessage;

    public const int NUMBER_OF_STONES_STARTING = 9;

    private readonly List<GameObject> whiteStones = new List<GameObject>();
    private readonly List<GameObject> blackStones = new List<GameObject>();

    public enum State { Placing, Moving, Removing, Jumping, Finished }
    private State state = State.Placing;

    public enum WhoseMove { White, Black };
    private WhoseMove whoseMove;

    void Start()
    {
        whoseMove = WhoseMove.White;
        for (int i = 0; i < NUMBER_OF_STONES_STARTING; i++)
        {
            whiteStones.Add(Instantiate(whiteStone, new Vector3(0, 0, 0), Quaternion.identity));
            whiteStones[i].transform.SetParent(pickupAreaWhiteStone.transform, false);
        }
        for (int i = 0; i < NUMBER_OF_STONES_STARTING; i++)
        {
            blackStones.Add(Instantiate(blackStone, new Vector3(0, 0, 0), Quaternion.identity));
            blackStones[i].transform.SetParent(pickupAreaBlackStone.transform, false);
        }
    }

    //Public Methods
    public void TableChanged(GameObject stone)
    {
        if (IsMühle(stone) && state != State.Removing)
        {
            state = State.Removing;
            ShowAllRemovable();
        }
        else
        {
            SwitchWhoseMove();
            if (!IsPickUpAreaEmpty())
                state = State.Placing;
            else
            {
                int count;
                if (whoseMove == WhoseMove.White)
                    count = whiteStones.Count;
                else
                    count = blackStones.Count;


                if (count > 3)
                    state = State.Moving;
                else if (count < 3)
                {
                    state = State.Finished;
                    GameFinished(false);
                }
                else if ((whiteStones.Count == 3 && blackStones.Count == 3))
                {
                    GameFinished(true);
                }
                else if (count == 3)
                    state = State.Jumping;
            }

        }

    }

    public void ClickedStone(GameObject stoneGameObject)
    {
        if (state == State.Removing)
        {
            if (whoseMove == WhoseMove.White && stoneGameObject.GetComponent<Stone>().color == WhoseMove.Black && IsRemovable(stoneGameObject))
            {
                blackStones.Remove(stoneGameObject);
                Destroy(stoneGameObject);
                HideAllRemovable();
                TableChanged(stoneGameObject);
            }
            else if (whoseMove == WhoseMove.Black && stoneGameObject.GetComponent<Stone>().color == WhoseMove.White && IsRemovable(stoneGameObject))
            {
                whiteStones.Remove(stoneGameObject);
                Destroy(stoneGameObject);
                HideAllRemovable();
                TableChanged(stoneGameObject);
            }
        }
    }

    public bool CanPlace(GameObject activePlace, GameObject hoveringPlace)
    {
        if (state == State.Placing)
        {
            return (activePlace == pickupAreaBlackStone || activePlace == pickupAreaWhiteStone) && IsPlaceFree(hoveringPlace);
            /**
            if ((activePlace == pickupAreaBlackStone || activePlace == pickupAreaWhiteStone) && IsPlaceFree(hoveringPlace))
                return true;
            else
                return false;
            */
        }
        else if (state == State.Moving)
        {
            return IsPossiblePlace(hoveringPlace, activePlace.GetComponent<Place>().GetPlaces()) && IsPlaceFree(hoveringPlace);
            /**
            if (IsPossiblePlace(hoveringPlace, activePlace.GetComponent<Place>().GetPlaces()))
            {
                if (IsPlaceFree(hoveringPlace))
                {
                    return true;
                }
                else
                    return false;
            }
            */
        }
        else if (state == State.Jumping)
        {
            return IsPlaceFree(hoveringPlace);
        }

        throw new Exception("Stone could not be placed. Unknown reason.");
    }

    public bool CanMove(WhoseMove color, GameObject activePlace)
    {
        if (color == whoseMove)
        {
            if (state == State.Placing)
            {
                if (activePlace == pickupAreaBlackStone || activePlace == pickupAreaWhiteStone)
                    return true;
            }
            else if (state != State.Removing)
                return true;
        }
        return false;
    }

    public void ShowAllPossibleMoves(GameObject stone)
    {
        for (int count = 0; count < 24; count++)
        {
            GameObject placeAtCount = transform.GetChild(0).GetChild(count).gameObject;
            if (CanPlace(stone.transform.parent.gameObject, placeAtCount))
            {
                placeAtCount.GetComponent<Place>().EnablePossibleMoveImage();
            }
        }
    }

    public void HideAllPossibleMoves()
    {
        for (int count = 0; count < 24; count++)
        {
            transform.GetChild(0).GetChild(count).gameObject.GetComponent<Place>().DisablePossibleMoveImage();
        }
    }


    //Private Methods
    private void ShowAllRemovable()
    {
        if (whoseMove == WhoseMove.White)
        {
            foreach (GameObject stone in blackStones)
            {
                if (IsRemovable(stone))
                    stone.transform.GetChild(0).GetComponent<Image>().enabled = true;
                else
                    stone.transform.GetChild(0).GetComponent<Image>().enabled = false;
            }

        }
        else
        {
            foreach (GameObject stone in whiteStones)
            {
                if (IsRemovable(stone))
                    stone.transform.GetChild(0).GetComponent<Image>().enabled = true;
                else
                    stone.transform.GetChild(0).GetComponent<Image>().enabled = false;
            }
        }
    }

    private void HideAllRemovable()
    {
        foreach (GameObject stone in blackStones)
        {
            stone.transform.GetChild(0).GetComponent<Image>().enabled = false;
        }
        foreach (GameObject stone in whiteStones)
        {
            stone.transform.GetChild(0).GetComponent<Image>().enabled = false;
        }
    }

    private bool IsPickUpAreaEmpty()
    {
        return pickupAreaWhiteStone.transform.childCount == 0 && pickupAreaBlackStone.transform.childCount == 0;
    }

    private void SwitchWhoseMove()
    {
        if (whoseMove == WhoseMove.White)
        {
            whoseMove = WhoseMove.Black;
            BlackParticles.SetActive(true);
            WhiteParticles.SetActive(false);
        }
        else
        {
            whoseMove = WhoseMove.White;
            WhiteParticles.SetActive(true);
            BlackParticles.SetActive(false);
        }
    }

    private bool IsPossiblePlace(GameObject hoveringPlace, GameObject[] places)
    {
        foreach (GameObject place in places)
            if (hoveringPlace == place)
                return true;
        return false;
    }

    private bool IsPlaceFree(GameObject hoveringPlace)
    {
        return hoveringPlace.transform.childCount == 0;
    }

    private bool IsMühle(GameObject stone)
    {
        Stone.VerticalPosition verticalPosition = stone.GetComponent<Stone>().GetVerticalPosition();
        if (verticalPosition == Stone.VerticalPosition.Up)
        {
            if (stone.transform.parent.GetComponent<Place>().down.transform.childCount == 1 && stone.transform.parent.GetComponent<Place>().down.GetComponent<Place>().down.transform.childCount == 1)
            {
                WhoseMove one = stone.transform.parent.GetComponent<Place>().down.transform.GetChild(0).transform.GetComponent<Stone>().color;
                WhoseMove two = stone.transform.parent.GetComponent<Place>().down.GetComponent<Place>().down.transform.GetChild(0).transform.GetComponent<Stone>().color;
                if (stone.GetComponent<Stone>().color == one && one == two)
                    return true;
            }
        }
        else if (verticalPosition == Stone.VerticalPosition.Down)
        {
            if (stone.transform.parent.GetComponent<Place>().up.transform.childCount == 1 && stone.transform.parent.GetComponent<Place>().up.GetComponent<Place>().up.transform.childCount == 1)
            {
                WhoseMove one = stone.transform.parent.GetComponent<Place>().up.transform.GetChild(0).transform.GetComponent<Stone>().color;
                WhoseMove two = stone.transform.parent.GetComponent<Place>().up.GetComponent<Place>().up.transform.GetChild(0).transform.GetComponent<Stone>().color;
                if (stone.GetComponent<Stone>().color == one && one == two)
                    return true;
            }
        }
        else if (verticalPosition == Stone.VerticalPosition.Middle)
        {
            if (stone.transform.parent.GetComponent<Place>().up.transform.childCount == 1 && stone.transform.parent.GetComponent<Place>().down.transform.childCount == 1)
            {
                WhoseMove one = stone.transform.parent.GetComponent<Place>().up.transform.GetChild(0).transform.GetComponent<Stone>().color;
                WhoseMove two = stone.transform.parent.GetComponent<Place>().down.transform.GetChild(0).transform.GetComponent<Stone>().color;
                if (stone.GetComponent<Stone>().color == one && one == two)
                    return true;
            }
        }

        Stone.HorizontalPosition horizontalPosition = stone.GetComponent<Stone>().GetHorizontalPosition();
        if (horizontalPosition == Stone.HorizontalPosition.Left)
        {
            if (stone.transform.parent.GetComponent<Place>().right.transform.childCount == 1 && stone.transform.parent.GetComponent<Place>().right.GetComponent<Place>().right.transform.childCount == 1)
            {
                WhoseMove one = stone.transform.parent.GetComponent<Place>().right.transform.GetChild(0).transform.GetComponent<Stone>().color;
                WhoseMove two = stone.transform.parent.GetComponent<Place>().right.GetComponent<Place>().right.transform.GetChild(0).transform.GetComponent<Stone>().color;
                if (stone.GetComponent<Stone>().color == one && one == two)
                    return true;
            }
        }
        else if (horizontalPosition == Stone.HorizontalPosition.Right)
        {
            if (stone.transform.parent.GetComponent<Place>().left.transform.childCount == 1 && stone.transform.parent.GetComponent<Place>().left.GetComponent<Place>().left.transform.childCount == 1)
            {
                WhoseMove one = stone.transform.parent.GetComponent<Place>().left.transform.GetChild(0).transform.GetComponent<Stone>().color;
                WhoseMove two = stone.transform.parent.GetComponent<Place>().left.GetComponent<Place>().left.transform.GetChild(0).transform.GetComponent<Stone>().color;
                if (stone.GetComponent<Stone>().color == one && one == two)
                    return true;
            }
        }
        else if (horizontalPosition == Stone.HorizontalPosition.Middle)
        {
            if (stone.transform.parent.GetComponent<Place>().left.transform.childCount == 1 && stone.transform.parent.GetComponent<Place>().right.transform.childCount == 1)
            {
                WhoseMove one = stone.transform.parent.GetComponent<Place>().left.transform.GetChild(0).transform.GetComponent<Stone>().color;
                WhoseMove two = stone.transform.parent.GetComponent<Place>().right.transform.GetChild(0).transform.GetComponent<Stone>().color;
                if (stone.GetComponent<Stone>().color == one && one == two)
                    return true;
            }
        }

        return false;
    }

    private bool IsOnTable(GameObject stone)
    {
        return stone.transform.parent.gameObject != pickupAreaBlackStone && stone.transform.parent.gameObject != pickupAreaWhiteStone;
    }

    private bool IsRemovable(GameObject stoneGameObject)
    {
        return IsOnTable(stoneGameObject) && stoneGameObject.GetComponent<Stone>().color != whoseMove && !(IsMühle(stoneGameObject) && AreStoneWithoutMühle(stoneGameObject.GetComponent<Stone>().color));
    }

    private bool AreStoneWithoutMühle(WhoseMove ofColor)
    {
        bool returnValue = false;
        if (ofColor == WhoseMove.White)
        {
            foreach (GameObject stone in whiteStones)
                if (IsOnTable(stone) && !IsMühle(stone))
                    returnValue = true;
        }
        else
        {
            foreach (GameObject stone in blackStones)
                if (IsOnTable(stone) && !IsMühle(stone))
                    returnValue = true;
        }

        return returnValue;
    }

    //other reference is on button "Menu" in the PausedMenu => its used
    private void GoToMenu()
    {
        SceneManager.LoadScene(0);
    }

    private void GameFinished(bool isDraw)
    {
        if (isDraw)
        {
            VictoryMessage.GetComponent<VictoryMessage>().textMeshPro.text = "Game is a draw!";
        }
        else
        {
            if (whoseMove != WhoseMove.White)
            {
                VictoryMessage.GetComponent<VictoryMessage>().textMeshPro.text = "White has won!";
            }
            else
            {
                VictoryMessage.GetComponent<VictoryMessage>().textMeshPro.text = "Black has won!";
            }
        }
        VictoryMessage.SetActive(true);
    }
}