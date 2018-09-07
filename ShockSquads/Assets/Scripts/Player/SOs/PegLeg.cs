using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BP
{
    public class PegLeg : BodyPart
    {

        BodyPartType thisBodyPartType = BodyPartType.LeftLeg;

        public override void Constructor()
        {
            bodyPartType = thisBodyPartType;
        }

    }
}
