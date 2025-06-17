using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu]
public class SOSkill : ScriptableObject
{
    [LabelText("데미지")] public float damage;
    [LabelText("쿨타임")] public float coolTime;

    [LabelText("애니메이션")] public string animationName;
    [LabelText("아이콘")] public Sprite icon;
}
