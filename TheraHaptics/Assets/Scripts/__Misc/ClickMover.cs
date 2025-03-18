using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public class ClickMover : MonoBehaviour
{
    [SerializeField] private static ClickMover painLocation;
    int x = 395;
    int y = 475;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0)){
            Debug.Log(Input.mousePosition.x + ", " + Input.mousePosition.y);
        }
        if (Input.GetMouseButtonDown(0) && Input.mousePosition.x > 210 && Input.mousePosition.x < 1000 && Input.mousePosition.y > 65 && Input.mousePosition.y < 1015){
            transform.position=new Vector3(Input.mousePosition.x, Input.mousePosition.y, transform.position.z);
            x = Convert.ToInt32(Input.mousePosition.x);
            y = Convert.ToInt32(Input.mousePosition.y);
            Debug.Log(x + ", " + y);
        }
    }
    public static int getX(){
        return painLocation.x;
    }
    public static int getY(){
        return painLocation.y;
    }
}
