using UnityEngine;

namespace Modules.LipSync
{
    [CreateAssetMenu(menuName = nameof(MouthShapeIndexMap), fileName = nameof(MouthShapeIndexMap), order = 0)]
    public class MouthShapeIndexMap : ScriptableObject
    {
        [Range(0, 30)] public int
            ClosedMouth,
            SlightlyOpenMouth,
            OpenMouth,
            WideOpenMouth,
            SlightlyRoundMouth,
            PuckeredLips,
            UpperTeethTouchingLowerLip,
            LSoundShape,
            IdlePosition;

        public int GetIndex(string value)
        {
            switch (value)
            {
                case "A": return ClosedMouth;
                case "B": return SlightlyOpenMouth;
                case "C": return OpenMouth;
                case "D": return WideOpenMouth;
                case "E": return SlightlyRoundMouth;
                case "F": return PuckeredLips;
                case "G": return UpperTeethTouchingLowerLip;
                case "H": return LSoundShape;
                case "X": return IdlePosition;
            }

            return IdlePosition;
        }
    }
}