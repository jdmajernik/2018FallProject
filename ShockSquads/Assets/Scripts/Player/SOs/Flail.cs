using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BP
{
    public class Flail : BodyPart
    {

        BodyPartType thisBodyPartType = BodyPartType.LeftArm;

        public override void Constructor()
        {
            bodyPartType = thisBodyPartType;
        }

    }
}
