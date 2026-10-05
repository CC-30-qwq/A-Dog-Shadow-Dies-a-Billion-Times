using System.Collections.Generic;
using UnityEngine;

public class SkillSystem : MonoBehaviour
{
    public List<GameObject> skillIcons;
    BehaviourStatus status;

    void Start()
    {
        status = GetComponent<BehaviourStatus>();
    }

    void Update()
    {
        if (status.Skill.currentSkillHash >= 0 && status.Skill.currentSkillHash < skillIcons.Count)
        {
            for (int i = 0; i < skillIcons.Count; i++)
            {
                if (i == status.Skill.currentSkillHash)
                {
                    skillIcons[i].SetActive(true);
                }
                else
                {
                    skillIcons[i].SetActive(false);
                }
            }
        }
    }
}
