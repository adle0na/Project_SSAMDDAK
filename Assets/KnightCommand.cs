using System.Collections.Generic;
using UnityEngine;

public class KnightCommandInput : CommandInput
{
    [Header("콤보 설정")]
    public int   maxCombo          = 3;
    public float comboInputWindow  = 0.25f;
    public float lungeDistance     = 0.25f;
    
    protected override List<(string, List<HashSet<string>>)> SkillCommands => new List<(string, List<HashSet<string>>)>
    {
        // ➡️⬇️↘️ K — 초승달 베기
        ("초승달 베기", new List<HashSet<string>> {
            new HashSet<string>{ "Right" },
            new HashSet<string>{ "Down" },
            new HashSet<string>{ "Down-Right", "Right", "Down" },
            new HashSet<string>{ "K" }
        }),
        
        ("초승달 베기", new List<HashSet<string>> {
            new HashSet<string>{ "Left" },
            new HashSet<string>{ "Down" },
            new HashSet<string>{ "Down-Left", "Left", "Down" },
            new HashSet<string>{ "K" }
        }),
        
        // ➡️➡️ K — 돌격 강습
        ("돌격 강습", new List<HashSet<string>> {
            new HashSet<string>{ "Right" },
            new HashSet<string>{ "Right" },
            new HashSet<string>{ "K" }
        }),
        ("돌격 강습", new List<HashSet<string>> {
            new HashSet<string>{ "Left" },
            new HashSet<string>{ "Left" },
            new HashSet<string>{ "K" }
        }),
        
        // ⬇️⬇️ K — 기사의 맹세 (버프)
        ("기사의 맹세", new List<HashSet<string>> {
            new HashSet<string>{ "Up" },
            new HashSet<string>{ "Up" },
            new HashSet<string>{ "K" }
        }),
        
        // ⬅️↙️⬇️↘️➡️ K — 맹세의 검풍
        ("맹세의 검풍", new List<HashSet<string>> {
            new HashSet<string> { "Left", "Down-Left" },
            new HashSet<string> { "Down", "Down-Left", "Down-Right" },
            new HashSet<string> { "Right", "Down-Right" },
            new HashSet<string> { "K" }
        }),
        
        ("맹세의 검풍", new List<HashSet<string>> {
            new HashSet<string> { "Right", "Down-Right" },
            new HashSet<string> { "Down", "Down-Right", "Down-Left" },
            new HashSet<string> { "Left", "Down-Left" },
            new HashSet<string> { "K" }
        }),

        // // ⬅️↙️⬇️↘️➡️ K홀드 — 진 맹세의 검풍
        // ("진 맹세의 검풍", new List<HashSet<string>> {
        //     new HashSet<string>{ "Left", "Down-Left" },
        //     new HashSet<string>{ "Down", "Down-Left", "Down-Right" },
        //     new HashSet<string>{ "Down", "Down-Left", "Down-Right" },
        //     new HashSet<string>{ "Down-Right", "Right" },
        //     new HashSet<string>{ "Right" },
        //     new HashSet<string>{ "K_Hold" }
        // }),
        // ("진 맹세의 검풍", new List<HashSet<string>> {
        //     new HashSet<string>{ "Right", "Down-Right" },
        //     new HashSet<string>{ "Down", "Down-Left", "Down-Right" },
        //     new HashSet<string>{ "Down", "Down-Left", "Down-Right" },
        //     new HashSet<string>{ "Down-Left", "Left" },
        //     new HashSet<string>{ "Left" },
        //     new HashSet<string>{ "K_Hold" }
        // }),
    };
}
