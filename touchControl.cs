using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class touchControl : MonoBehaviour {

    public float touchHoldTime = 80;

    //camera position variables
    public Transform locator;
    public float damping = 1.0f;
    private Vector3 vCameraSlideTo;

    //camera orbit variables
    public GameObject camPos;
    public Vector2 touchOrbitSpeed = new Vector2(-0.25f, 0.25f);
    public Vector2 mouseOrbitSpeed = new Vector2(-3f, 3f);
    public Vector2 tiltRange = new Vector2(-89f, 89f);
    private float Xtilt = 20f;
    private float Yorbit = 0f;
    private float tempRotX;
    private float tempRotY;
    private float tempZoom;

    //camera zoom variables
    public GameObject camOffset;
    public float touchZoomSpeed = 0.25f;
    public float mouseZoomSpeed = 20f;
    public Vector2 zoomRange = new Vector2(1f, 300f);
    private float offsetZ = 100f;
    public Text camOffsetTxt;

    //event management
    public delegate void clickAction();
    public static event clickAction onClicked;

    //private variables used internally for the script
    private bool touchActive = false;
    private bool touchEnabled = false;
    private float touchHolding = 0;
    private Vector2 touchZeroPrevPos = new Vector2(0f, 0f);
    private float touchDeltaMag = 0f;
    private float touchDuration = 0f;
    private Touch touchZero;
    //private int clickCount = 0;

    public Text dragTxt;

    // Use this for initialization
    void Start () {
        //check to see if touch suppported
        if (Input.touchSupported)
        {
            touchEnabled = true;
        }
        else
        {
            touchEnabled = false;
            StartCoroutine(HandleMouse());
        }


    }

    // Update is called once per frame
    void Update()
    {
        if(touchEnabled == true)
        {
            HandleTouch();
        }

    }

    private void LateUpdate()
    {
        tempZoom = camOffset.transform.localPosition.z * -1f;
        tempRotX = ClampAngle(camPos.transform.localEulerAngles.x, tiltRange.x, tiltRange.y);
        tempRotY = camPos.transform.localEulerAngles.y;

        float finalX = Mathf.LerpAngle(tempRotX, Xtilt, 2f * Time.deltaTime);
        
        float finalY = Mathf.LerpAngle(tempRotY, Yorbit, 2f * Time.deltaTime);
        float finalZ = Mathf.Lerp(tempZoom, offsetZ, 2f * Time.deltaTime);

        //clamp to user specified ranges for tilting and zooming
        finalZ = Mathf.Clamp(finalZ, zoomRange.x, zoomRange.y);
        finalX = ClampAngle(finalX, tiltRange.x, tiltRange.y);
        

        camPos.transform.localEulerAngles = new Vector3(finalX, finalY, 0f);
        camOffset.transform.localPosition = new Vector3(0f, 0f, finalZ * -1f);
        Debug.Log("rotateX = " + finalX + "  " + "transform = " + camPos.transform.localEulerAngles.x + "  " + "temp X = " + tempRotX);
    }

    // touch control
    public void HandleTouch()
    {
        // tap count section
        if(Input.touchCount > 0)
        {
            touchDuration += Time.deltaTime;
            touchZero = Input.GetTouch(0);
            touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
            touchDeltaMag = (touchZero.position - touchZeroPrevPos).magnitude;

            //duration between taps before considering a double tap
            if(touchZero.phase == TouchPhase.Ended && touchDuration < 0.33f)
            {
                if (touchZero.tapCount == 2)
                {
                    CoCamMove();
                }
            }
            else
            {
                touchDuration = 0f;
            }
        }
        //check for a single touch and if it is holding
        if (Input.touchCount == 1 && touchZero.phase == TouchPhase.Stationary)
        {
            touchHolding += 1;
            if(touchHolding >= touchHoldTime)
            {
                if (onClicked != null)
                    onClicked();
                touchHolding = 0;
            }
            if (touchZero.phase == TouchPhase.Ended)
            {
                touchHolding = 0;
            }
        }
        //check for a single touch - this is point/click
        if (Input.touchCount == 1 && touchDeltaMag < 1.0f)
        {

        }
        //check for single touch hold and drag - used for camera orbit
        if (Input.touchCount == 1 && touchDeltaMag > 1.5f)
        {
            touchActive = true;
            touchHolding = 0;

            StartCoroutine(CoCamDrag());
        }
        //check for two touch inputs - this is pinch-to-zoom
        if (Input.touchCount == 2)
        {
            touchActive = true;
            touchHolding = 0;
            StartCoroutine(CoPinchZoom());
        }
        //ensure no touches are active
        if (Input.touchCount < 1)
        {
            touchHolding = 0;
            touchActive = false;
        }
        camOffsetTxt.text = touchHolding.ToString();
    }

    //move to new location on terrain
    public void CoCamMove()
    {
        Ray rMouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit rMouseRayHit;

        if (Physics.Raycast(rMouseRay, out rMouseRayHit, 5000.0f))
        {
            Debug.DrawLine(rMouseRay.origin, rMouseRayHit.point);
            Debug.Log(rMouseRayHit.point);

            //vCameraSlideFrom = transform.position; //Camera.main.gameObject.transform.position;
            vCameraSlideTo = rMouseRayHit.point;
        }
        locator.transform.position = vCameraSlideTo;
    }

    //camera orbit and tilt drag
    public IEnumerator CoCamDrag()
    {
        Touch touchZero = Input.GetTouch(0);
        Vector2 touchDeltaPosition = new Vector2(0f, 0f);

        if (touchZero.phase == TouchPhase.Moved)
        {
            
            float touchPrevPositionY = touchZero.position.y - touchZero.deltaPosition.y;
            float touchPrevPositionX = touchZero.position.x - touchZero.deltaPosition.x;

            // Find the magnitude of the vector (the distance) between the touches in each frame.
            float touchPositionYMag = touchZero.position.y - touchPrevPositionY;
            float touchPositionXMag = touchZero.position.x - touchPrevPositionX;

            // Find the difference in the distances between each frame.
            //float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;

            Yorbit += touchPositionXMag * touchOrbitSpeed.x;
            Xtilt += touchPositionYMag * touchOrbitSpeed.y;
            //Xtilt = Mathf.Clamp(Xtilt, tiltRange.x, tiltRange.y);
            touchDeltaPosition = new Vector2(touchPositionXMag, touchPositionYMag);
            //camPos.transform.localEulerAngles = new Vector3(Xtilt, Yorbit, 0);

            dragTxt.text = touchDeltaPosition.ToString();
        }
        if (Input.touchCount == 0)
        {
            yield return new WaitForSeconds(0.3f);
            touchActive = false;
        }
        yield return null;
    }

    public IEnumerator CoPinchZoom()
    {
        // Store both touches.
        Touch touchZero = Input.GetTouch(0);
        Touch touchOne = Input.GetTouch(1);

        if (touchZero.phase == TouchPhase.Moved && touchOne.phase == TouchPhase.Moved)
        {
            // Find the position in the previous frame of each touch.
            Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
            Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

            // Find the magnitude of the vector (the distance) between the touches in each frame.
            float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
            float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

            // Find the difference in the distances between each frame.
            float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;

            //add values to zoom. Clamp to stay within boundaries
            offsetZ += deltaMagnitudeDiff * touchZoomSpeed;
            offsetZ = Mathf.Clamp(offsetZ, zoomRange.x, zoomRange.y);

            //transform camera offset node, debug numbers to UI
            //camOffset.transform.localPosition = new Vector3(0, 0, offsetZ * -1.0f);
            //Debug.Log(offsetZ);
        }
        
        if(Input.touchCount == 0)
        {
            yield return new WaitForSeconds(0.3f);
            touchActive = false;
        }

        yield return null;
    }

    //---------------------------------------------------------------------------------------------------------
    //Mouse controls
    public IEnumerator HandleMouse()
    {
        while (true)
        {
            float duration = 0;
            bool doubleClicked = false;
            if (Input.GetMouseButtonDown(0))
            {
                while (duration < 0.25f)
                {
                    duration += Time.deltaTime;
                    yield return new WaitForSeconds(0.005f);
                    if (Input.GetMouseButtonDown(0))
                    {
                        doubleClicked = true;
                        duration = 0.3f;
                        // Double click/tap
                        //Debug.Log("Double Click detected");
                        CoCamMove();
                    }
                }
                if (!doubleClicked)
                {
                    // Single click/tap
                    //Debug.Log("Single Click detected");
                }
               
            }
            //mouse orbit and tilt while holding button down
            while (Input.GetMouseButton(0))
            {
                Xtilt += Input.GetAxis("Mouse Y") * mouseOrbitSpeed.x;
                Yorbit += Input.GetAxis("Mouse X") * mouseOrbitSpeed.y;
                //Xtilt = Mathf.(Xtilt, tiltRange.x, tiltRange.y);
                //Debug.Log(Xtilt);
                //camPos.transform.localEulerAngles = new Vector3(Xtilt, Yorbit, 0);
                yield return null;
            }
            // mouse zoom scroll wheel
            while(Input.GetAxis("Mouse ScrollWheel") != 0)
            {
                offsetZ += (Input.GetAxis("Mouse ScrollWheel") * -1f) * mouseZoomSpeed;
                //offsetZ = Mathf.Clamp(offsetZ, 15f, 300f);
                //camOffset.transform.localPosition = new Vector3(0, 0, offsetZ * -1f);
                
                yield return null;
            }
            if (Input.GetMouseButtonDown(1))
            {
                if (onClicked != null)
                    onClicked();
                //Debug.Log("mouse button 1");
            }
            yield return null;
        }
    }

    float ClampAngle(float angle, float from, float to)
    {
        // accepts e.g. -80, 80
        if (angle < 0f)
        {
            angle = 360f + angle;
        }
        if (angle > 180f)
        {
            return Mathf.Max(angle, 360f + from);
        }
        return Mathf.Min(angle, to);
    }
}
