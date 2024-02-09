using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crosshair : MonoBehaviour
{
    public float crosshairSize = 10f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnGUI()
    {
        float x = (Screen.width / 2) - crosshairSize/2;
        float y = Screen.height / 2 - crosshairSize/2;

        GUI.DrawTexture(new Rect(x, y, crosshairSize, crosshairSize), Texture2D.whiteTexture);
    }
}
