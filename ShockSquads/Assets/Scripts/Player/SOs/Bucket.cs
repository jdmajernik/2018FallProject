using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BP
{
    public class Bucket : BodyPart
    {

        BodyPartType thisBodyPartType = BodyPartType.Head;

        public override void Constructor()
        {
            bodyPartType = thisBodyPartType;
        }

    }
}
