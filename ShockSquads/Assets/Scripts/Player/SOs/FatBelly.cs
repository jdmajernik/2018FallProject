using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BP
{
    public class FatBelly : BodyPart
    {

        BodyPartType thisBodyPartType = BodyPartType.Torso;

        public override void Constructor()
        {
            bodyPartType = thisBodyPartType;
        }

    }
}
