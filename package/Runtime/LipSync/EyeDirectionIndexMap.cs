using UnityEngine;

namespace LipSync
{
    [CreateAssetMenu(menuName = "Lipsync/" + nameof(EyeDirectionIndexMap), fileName = nameof(EyeDirectionIndexMap), order = 0)]
    public class EyeDirectionIndexMap : ScriptableObject
    {
        public int TopLeft, TopCenter, TopRight, MiddleLeft, MiddleCenter, MiddleRight, BottomLeft, BottomCenter, BottomRight;

        public int GetIndex(Vector3 lookDir, float horizontalSensitivity01 = .5f, float verticalSensitivity01 = .5f)
        {
            var lr = lookDir.x;
            var ud = lookDir.y;

            // CENTER
            if (lr >= -horizontalSensitivity01 && lr < horizontalSensitivity01)
            {
                // MIDDLE
                if (ud >= -verticalSensitivity01 && ud <= verticalSensitivity01)
                {
                    return MiddleCenter;
                }
                // DOWN
                else if (ud < -verticalSensitivity01)
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
            else if (lr <= -horizontalSensitivity01)
            {
                // MIDDLE
                if (ud >= -verticalSensitivity01 && ud <= verticalSensitivity01)
                {
                    return MiddleRight;
                }
                // DOWN
                else if (ud < -verticalSensitivity01)
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
                if (ud >= -verticalSensitivity01 && ud <= verticalSensitivity01)
                {
                    return MiddleLeft;
                }
                // DOWN
                else if (ud < -verticalSensitivity01)
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