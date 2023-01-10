using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;

namespace QuickRuleTileEditor
{
    public interface IRuleTilePatternsProvider
    {
        string FirstDefaultPatternId { get; }

        List<string> GetAllPatternsIds();
        void LoadPattern(string id, IObjectPickerHost objectPicker, System.Action<RuleTile> callback);
        string GetPatternName(string id);
    }

    public class RuleTilePatternsProvider : IRuleTilePatternsProvider
    {
        private static Dictionary<string, string> DefaultPatterns { get; } = new()
        {
            { "Pattern-16", "16" },
            { "Pattern-15", "15" },
            { "Pattern-47", "47" },
        };
        private static string CustomPatternId { get; } = "Custom";

        public string FirstDefaultPatternId => DefaultPatterns.First().Key;


        public List<string> GetAllPatternsIds()
        {
            var patternsList = new List<string>();
            patternsList.AddRange(DefaultPatterns.Keys);
            patternsList.Add(CustomPatternId);
            return patternsList;
        }

        public void LoadPattern(string id, IObjectPickerHost objectPicker, System.Action<RuleTile> callback)
        {
            if (DefaultPatterns.ContainsKey(id))
            {
                callback(LoadDefaultPattern(id));
            }
            else if (id == CustomPatternId)
            {
                objectPicker.ShowObjectPicker<RuleTile>(null, false, null, customPattern =>
                {
                    if (customPattern == null)
                    {
                        callback(null);
                        return;
                    }

                    callback((RuleTile)customPattern);
                });
            }
            else
            {
                // Unsupported id somehow
                Debug.LogError($"Pattern with id '{id}' not found");
                callback(null);
            }
        }

        public string GetPatternName(string id)
        {
            if (DefaultPatterns.TryGetValue(id, out var name))
            {
                return name;
            }

            return id;
        }


        private static RuleTile LoadDefaultPattern(string id)
        {
            return Resources.Load<RuleTile>(id);
        }
    }
}
