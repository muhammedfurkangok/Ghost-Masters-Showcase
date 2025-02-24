using Runtime.Data.ValueObject;
using UnityEngine;

namespace Runtime.Data.UnityObject
{
    [CreateAssetMenu(fileName = "CD_GameSound", menuName = "Furtle Game/Sound System/CD_GameSound", order = 0)]
    public class CD_GameSound : ScriptableObject
    {
        public GameSoundData[] gameSoundData;
    }
}