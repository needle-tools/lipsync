using UnityEngine;

namespace LipSync
{
    [CreateAssetMenu(menuName = "Lipsync/" + nameof(EyeDirectionIndexMap), fileName = nameof(EyeDirectionIndexMap), order = 0)]
    public class EyeDirectionIndexMap : ScriptableObject
    {
        public int TopLeft, TopCenter, TopRight, MiddleLeft, MiddleCenter, MiddleRight, BottomLeft, BottomCenter, BottomRight;

        public int GetIndex(Vector3 eyePosition, Vector3 targetPosition)
        {
            var dir = targetPosition - eyePosition;
            var lookDir = dir.normalized;

            var lr = lookDir.x;
            var ud = lookDir.y;

            // CENTER
            if (lr >= -.5f && lr < .5f)
            {
                // MIDDLE
                if (ud >= -.5f && ud <= .5f)
                {
                    return MiddleCenter;
                }
                // DOWN
                else if (ud < -.5f)
                {
                    return BottomCenter;
                }
                // UP
                else
                {
                    return TopCenter;
                }
            }
            // LEFT
            else if (lr <= -.5f)
            {
                // MIDDLE
                if (ud >= -.5f && ud <= .5f)
                {
                    return MiddleRight;
                }
                // DOWN
                else if (ud < -.5f)
                {
                    return BottomRight;
                }
                // UP
                else
                {
                    return TopRight;
                }
            }
            // RIGHT
            else
            {
                // MIDDLE
                if (ud >= -.5f && ud <= .5f)
                {
                    return MiddleLeft;
                }
                // DOWN
                else if (ud < -.5f)
                {
                    return BottomLeft;
                }
                // UP
                else
                {
                    return TopLeft;
                }
            }
            
            return 0;
        }
    }
}