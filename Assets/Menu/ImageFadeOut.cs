using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageFadeOut : MonoBehaviour
{
    public float FadeRate;
    private Image image;
    // Use this for initialization
    void Start()
    {
        image = GetComponent<Image>();
        if (image == null)
        {
            Debug.LogError("Error: No image on " + this.name);
        }
    }

    // Update is called once per frame
    void Update()
    {
        Color curColor = image.color;
        float alphaDiff = Mathf.Abs(curColor.a - 0.0f);
        if (alphaDiff > 0.0001f)
        {
            curColor.a = Mathf.Lerp(curColor.a, 0.0f, FadeRate * Time.deltaTime);
            image.color = curColor;
        }

        if (curColor.a < 0.0001f)
            gameObject.SetActive(false);
    }
}
