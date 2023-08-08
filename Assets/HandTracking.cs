using System.Collections;
using System.Collections.Generic;
using Leap;
using Leap.Unity;
using TMPro;
using UnityEngine;
public class HandTracking : MonoBehaviour
{
    LeapProvider provider;
    [SerializeField] float Yepsilon = 0.1f;
    [SerializeField] float gestureEpsilon = 0.55f;
    [SerializeField] float HorizontalDiff = 0.1f;
    [SerializeField] TextMeshProUGUI text;
    float[] lastX = new float[100];
    int lastXIndex = 0; 
    [SerializeField] BirdController bc;
    bool jumping = false;
    //[SerializeField] int jumpCounterMax = 10;
    void Start(){
        provider = GetComponent<LeapProvider>();
    }
    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < provider.CurrentFrame.Hands.Count; i++){
            Hand _hand = provider.CurrentFrame.Hands[i];
            Debug.Log(_hand.PalmPosition.y);
            if (_hand.PalmVelocity.y < Yepsilon){
                if (!jumping){
                    bc.Jump();
                    jumping = true;
                }
            }
            else if (_hand.PalmVelocity.y > 0) jumping = false;
            // float currentDiff = 0;
            // if (!bc.IsChangingLanes){
            //     switch(bc.CurrentLane){
            //         case 0: {
            //             if (_hand.PalmPosition.x > GetAverageX() - HorizontalDiff) bc.MoveRight();
            //             currentDiff = 0;
            //         } break;
            //         case 1: {
            //             if (_hand.PalmPosition.x > GetAverageX() + HorizontalDiff) bc.MoveRight();
            //             else if (_hand.PalmPosition.x < GetAverageX() - HorizontalDiff) bc.MoveLeft();
            //             currentDiff = 1;
            //         } break;
            //         case 2: {
            //             if (_hand.PalmPosition.x < GetAverageX() + HorizontalDiff) bc.MoveLeft();
            //             currentDiff = 0;
            //         } break;
            //         default: break;
            //     }
            // }
            // if (bc.CurrentLane == 1) AddX(_hand.PalmPosition.x);
            // text.text = GetAverageX().ToString();
            if (!bc.IsChangingLanes){
                switch(bc.CurrentLane){
                    case 0: {
                        if (_hand.PalmPosition.x > -1*HorizontalDiff) bc.MoveRight();
                    } break;
                    case 1: {
                        if (_hand.PalmPosition.x > 1.2*HorizontalDiff) bc.MoveRight();
                        else if (_hand.PalmPosition.x < -1.2* HorizontalDiff) bc.MoveLeft();
                    } break;
                    case 2: {
                        if (_hand.PalmPosition.x < HorizontalDiff) bc.MoveLeft();
                    } break;
                    default: break;
                }
            }
            if (Vector3.Distance(_hand.Fingers[0].TipPosition, _hand.Fingers[4].TipPosition) < gestureEpsilon){
                bc.Restart();
            }
        }
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            bc.Jump();
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
        {
            bc.MoveLeft();
        }
        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
        {
            bc.MoveRight();
        }
        if(Input.GetKeyDown(KeyCode.R)){
            bc.Restart();
        }
    }

    void AddX(float f){
        lastX[lastXIndex] = f;
        lastXIndex++;
        if (lastXIndex >= lastX.Length) lastXIndex = 0;
    }
    float GetAverageX(){
        float sum = 0;
        for (int i = 0; i < lastX.Length; i++){
            sum += lastX[i];
        }
        return sum / lastX.Length;
    }
}
