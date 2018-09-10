using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BodyPartFactory {

    private static Dictionary<string, BodyPart> bodyParts = new Dictionary<string, BodyPart>();

    public static BodyPart Lookup(string s)
    {
        // add dictionary if it doesn't exist
        if (bodyParts.Count == 0)
        {
            // dictionary is empty, load in all the scriptable objects
            BodyPart[] bodyPartsArray = Resources.LoadAll<BodyPart>("");
            foreach (BodyPart thisBodyPart in bodyPartsArray)
            {
                bodyParts.Add(thisBodyPart.name, thisBodyPart);
            }
            Debug.Log("Created dictionary");
        }

        BodyPart bodyPart = null;
        bodyParts.TryGetValue(s, out bodyPart);
        return bodyPart;
    }

}
