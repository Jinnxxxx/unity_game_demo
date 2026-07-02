using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;

public class Blackhole_Hotkey_Controller : MonoBehaviour
{
    private KeyCode myHotKey; //需要按下的快捷键
    private TextMeshProUGUI myText; //tmp组件

    public void SetupHotKey(KeyCode _myHotKey)
    {
        myText = GetComponentInChildren<TextMeshProUGUI>();

        myText.text = myHotKey.ToString();
        myHotKey = _myHotKey;
    }
 
    void Update()
    {
        if (Input.GetKeyDown(myHotKey))
        {
            Debug.Log("HOT KEY IS " + myHotKey);
        }
    }

}
