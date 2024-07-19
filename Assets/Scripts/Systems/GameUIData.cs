using UnityEngine;

namespace Systems
{
    [CreateAssetMenu(menuName = "GameUIData")]
    public class GameUIData : ScriptableObject
    {
        public Sprite[] imgAvatar;
        public int[] foodNumCards;
    }
}