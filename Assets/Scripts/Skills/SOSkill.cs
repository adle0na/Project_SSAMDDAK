using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu]
public class SOSkill : ScriptableObject
{
    [LabelText("스킬 타입")] public SkillType skillType;
    [LabelText("잠금여부")] public bool isLocked;
    [LabelText("파생기 해금")] public bool isUpgrade;
    [LabelText("히든 스킬 해금")] public bool hiddenFind;
    
    [LabelText("데미지")] public float damage;
    
    [LabelText("애니메이션")] public string animationName;

    [LabelText("이펙트")] public GameObject effectObj;
    [LabelText("방향")] public SkillDirection skillDirection;
    
    [LabelText("스킬 아이콘")] public Sprite icon;
}
