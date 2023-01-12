using UnityEngine;

namespace QuickRuleTileEditor
{
    [System.Serializable]
    public struct RuleTilePattern
    {
        [SerializeField] private string id;
        [SerializeField] private RuleTile ruleTile;

        public string Id
        {
            get => id;
            set => id = value;
        }

        public RuleTile RuleTile
        {
            get => ruleTile;
            set => ruleTile = value;
        }

        public RuleTilePattern(string id, RuleTile ruleTile)
        {
            this.id = id;
            this.ruleTile = ruleTile;
        }
    }
}
