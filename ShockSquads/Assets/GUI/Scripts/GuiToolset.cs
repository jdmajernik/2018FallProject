using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuiToolset : MonoBehaviour
{
    //Written by John Majernik
    //This exists as a toolset for the GUI functions that enables use of MonoBehaviour scripts and coroutines
    //This also contains all the animations for the GUI, which are procedural

    //Exists to utilize the MonoBehaviour specific functions (cough, cough, Instantiate, cough) while being able to have a constructor
    //Whats that? Have my cake? Eat it too??? Don't mind if I do!!!
    public GameObject CreateObject(GameObject newObject, GameObject parent)
    {
        return Instantiate(newObject, parent.transform);
    }
    public void RemoveObject(GameObject removedObject)
    {
        Destroy(removedObject);
    }
    public void ReloadAnimation(GameObject removedClip)
    {
        StartCoroutine(RemoveAnimation(removedClip));
    }
    IEnumerator RemoveAnimation(GameObject removedClip)
    {
        //the "remove" animation that removes the object before throwing it away
        float duration = 0.05f; // the duration of the animation
        float endTime = Time.time + duration;
        float pullDistance = 15f; //the distance to move the ammoClip
        //The points needed to do the transistion
        Vector3 startPoint = removedClip.transform.position;
        Vector3 endPoint = new Vector3(pullDistance,0,0) + removedClip.transform.position;
        Vector3 moveDistance = endPoint - startPoint;

        bool isRemoved = false;//simple boolean latch to keep the animation going until it's done
        while (!isRemoved)
        {
            float deltaTime = ((Time.time - endTime) / duration) + 1;//returns 0->1 as time progresses over duration
            removedClip.transform.position = startPoint + (moveDistance * deltaTime);//adds distance over time deltaTime

            if(Time.time >= endTime)
            {
                isRemoved = true;//ends the animation
            }
            yield return null;
        }
        yield return new WaitForSeconds(0.05f); //waits just a second before moving to the throw animation
        StartCoroutine(ThrowAnimation(removedClip));
        yield return null;
    }
    IEnumerator ThrowAnimation(GameObject removedClip)
    {
        float thrownTime = .4f; //the duration, in seconds that the throwing animation takes
        float endTime = Time.time + thrownTime; //the end time of the animation
        float endRotation = 180; //the end rotation for the animation
        //setting up the bezier variables
        float thrownDistance = 200; //the distance on the x-axis the clip is thrown
        float thrownHeight = 70; //the height on the y-axis the clip reaches at its peak
        float landHeight = 50; //the height at which the clip lands

        //RectTransform
        //the start positon, end position and middle position needed for the bezier calculations
        Vector3 clipPos = removedClip.transform.position;
        Vector3 LookPoint = clipPos + new Vector3(thrownDistance/2, 0, 0);
        Vector3 clipEndPos = clipPos + new Vector3(thrownDistance, landHeight, 0);
        Vector3 clipHighPoint = clipPos + new Vector3((clipEndPos.x - clipPos.x) / 2, thrownHeight, 0);
        BezierCurve bezier = new BezierCurve(clipPos, clipEndPos, clipHighPoint); //the bezier class that stores all the calculations

        bool isDone = false; //simple bool to find if the animation is completed or not
        while (!isDone)
        {
            float deltaTime = ((Time.time - endTime) / thrownTime) + 1; // returns a value from 0 to 1 based on the animation length and current time
            if(Time.time >= endTime)
            {
                isDone = true;
            }
            removedClip.transform.localScale = new Vector2 (1 - deltaTime, 1 - deltaTime); //scales down as it gets thrown
            removedClip.transform.eulerAngles = new Vector3(0, 0, (1-deltaTime)*endRotation);
            removedClip.transform.position = bezier.pointOnCurve(deltaTime); //gets the point on the projected curve
            yield return null;
        }
        Destroy(removedClip);
        yield return null;
    }
}
public class BezierCurve
{
    Vector3 startPoint, endPoint, middlePoint;
    public BezierCurve (Vector2 _startPoint, Vector2 _endPoint, Vector2 _middlePoint)
    {
        startPoint = _startPoint;
        endPoint = _endPoint;
        middlePoint = _middlePoint;

        //TODO - DELETE this is for debugging and can be removed once done debugging (so, never)
        Debug.DrawLine(startPoint, endPoint, Color.red, 30f);
        Debug.DrawLine(startPoint, middlePoint, Color.gray, 30f);
        Debug.DrawLine(middlePoint, endPoint, Color.gray, 30f);
        for(float i = 0; i < 1; i+=0.1f)
        {
            
            Debug.DrawLine(pointOnCurve(i), pointOnCurve(i + 0.1f), Color.blue, 30f);
            Debug.Log("DRAWING LINE!!!!");
        }
    }
    public Vector3 pointOnCurve (float t)
    {
        Vector3 returnedPoint;
        float n = 1 - t; //n = 1-t, it pops up a lot in the formula, so I stored it on a variable
        returnedPoint = ((n * n) * startPoint) + (2 * n * t * middlePoint) + ((t * t) * endPoint);
        return returnedPoint;//rotates 90 degrees, because that's how it returns, for some reason
    }
}
