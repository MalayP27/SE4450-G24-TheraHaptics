using System.Net.Mime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Net.Http;
using System.Threading.Tasks;

public class Session : MonoBehaviour
{

    [SerializeField] private TMP_Text output;

    // Start is called before the first frame update
    void Start()
    {
        output.text = "helloooooo";
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
