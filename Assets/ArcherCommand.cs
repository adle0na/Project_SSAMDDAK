using System.Collections.Generic;
using UnityEngine;

public class ArcherCommandInput : CommandInput
{
    protected override List<(string, List<HashSet<string>>)> SkillCommands => new List<(string, List<HashSet<string>>)>
    {
        // ➡️➡️ K — front flip
        ("Front Flip", new List<HashSet<string>> {
            new HashSet<string>{ "Right" },
            new HashSet<string>{ "Right" },
            new HashSet<string>{ "K" }
        }),

        ("Front Flip ←", new List<HashSet<string>> {
            new HashSet<string>{ "Left" },
            new HashSet<string>{ "Left" },
            new HashSet<string>{ "K" }
        }),

        // ⬇️⬇️ K — cast spell
        ("Cast Spell", new List<HashSet<string>> {
            new HashSet<string>{ "Down" },
            new HashSet<string>{ "Down" },
            new HashSet<string>{ "K" }
        }),

        // ⬅️ 홀드 ➡️ K — 조준 사격
        ("Charge Shot", new List<HashSet<string>> {
            new HashSet<string>{ "Left_Hold" },
            new HashSet<string>{ "Right" },
            new HashSet<string>{ "K" }
        }),

        ("Charge Shot ←", new List<HashSet<string>> {
            new HashSet<string>{ "Right_Hold" },
            new HashSet<string>{ "Left" },
            new HashSet<string>{ "K" }
        }),

        // ⬅️ ↙️ ➡️ ⬇️ ↘️ K — 조준 후 점프 후 사격
        ("Flip Shot", new List<HashSet<string>> {
            new HashSet<string>{ "Left" },
            new HashSet<string>{ "Down-Left", "Left", "Down" },
            new HashSet<string>{ "Right" },
            new HashSet<string>{ "Down" },
            new HashSet<string>{ "Down-Right", "Right", "Down" },
            new HashSet<string>{ "K" }
        }),

        ("Flip Shot ←", new List<HashSet<string>> {
            new HashSet<string>{ "Right" },
            new HashSet<string>{ "Down-Right", "Right", "Down" },
            new HashSet<string>{ "Left" },
            new HashSet<string>{ "Down" },
            new HashSet<string>{ "Down-Left", "Left", "Down" },
            new HashSet<string>{ "K" }
        })
    };
}
