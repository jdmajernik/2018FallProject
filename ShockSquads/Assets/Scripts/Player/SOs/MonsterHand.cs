using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BP
{
    public class MonsterHand : BodyPart
    {

        BodyPartType thisBodyPartType = BodyPartType.RightArm;

        public override void Constructor()
        {
            bodyPartType = thisBodyPartType;
        }
    }
}
