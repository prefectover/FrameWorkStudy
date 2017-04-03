using System;
using UnityEngine;

using System.Collections;
using System.Collections.Generic;

namespace QFramework
{
    public interface ISkill
    {
        SkillInfo skillInfo { get; set; }
        AbstractSkillSystem skillSystem { get; }
        ISkillReleaser skillReleaser { get; }
        void DoSkillRelease(AbstractSkillSystem system, ISkillReleaser releaser);
        void DoSkillRemove();
        void DoSkillUpdate(float deltaTime);
    }
}
