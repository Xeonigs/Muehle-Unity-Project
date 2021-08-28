using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Place : MonoBehaviour
{
    public GameObject up;
    public GameObject right;
    public GameObject down;
    public GameObject left;

    private GameObject[] places;

    private void Start()
    {
        places = new GameObject[] { up, down, right, left };
    }

    public GameObject[] GetPlaces()
    {
        return places;
    }

    public void EnablePossibleMoveImage()
    {
        GetComponent<Image>().enabled = true;
    }

    public void DisablePossibleMoveImage()
    {
        GetComponent<Image>().enabled = false;
    }
}
