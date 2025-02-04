using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIDebugWindowA : MonoBehaviour
{
    public Text text_comp;

    private void Awake()
    {
        text_comp = GetComponent<Text>();
    }
}
