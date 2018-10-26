using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIslideCtrl : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private Slider UIslider;
    private bool sliderEnabled;
    public GameObject asset;
    private Animator assetAnim;

    void Start()
    {
        UIslider = this.GetComponent<Slider>();
        assetAnim = asset.GetComponent<Animator>();
    }

    public void OnPointerDown(PointerEventData data)
    {
        sliderEnabled = true;
        //Debug.Log("holding down!");
    }
    public void OnPointerUp(PointerEventData data)
    {
        sliderEnabled = false;
        //Debug.Log("holding up!");
    }

    void Update()
    {
        float animationTime = assetAnim.GetCurrentAnimatorStateInfo(0).normalizedTime;
        animationTime = animationTime % 1f;
        if(sliderEnabled == true)
        {
            assetAnim.speed = 0f;
            assetAnim.Play(0, -1, UIslider.value);
            
            animationTime = UIslider.value;
        }
        else
        {
            UIslider.value = animationTime;
        }
    }

}
