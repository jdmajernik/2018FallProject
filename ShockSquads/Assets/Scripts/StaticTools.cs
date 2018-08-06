using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticTools : MonoBehaviour {

    [SerializeField] protected GameObject AIControllerPrefab;

    public static GameObject FindChildGameObjectWithTag(GameObject parent, string tag)
    {
        Transform t = parent.transform;
        foreach(Transform this_t in t)
        {
            if (this_t.tag == tag)
            {
                return this_t.gameObject;
            }
        }
        return null;
    }

    public static void CreateAIController()
    {
        //Instantiate(AIControllerPrefab);
    }
}
