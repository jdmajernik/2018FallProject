using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BP
{
    public class RabbitLeg : BodyPart
    {

        BodyPartType thisBodyPartType = BodyPartType.RightLeg;

        public override void Constructor()
        {
            bodyPartType = thisBodyPartType;

            AbilityDelegate bunnyJump = BunnyJump;
            myAbilities.Add(AbilityType.None, bunnyJump);
        }

        private void BunnyJump()
        {
            Debug.Log("bunny jump ability!");
        }

    }
}
