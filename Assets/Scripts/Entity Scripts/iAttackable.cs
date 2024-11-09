using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Entities
{
    public interface iAttackable
    {
        public Attackable DamageableTo { get; }

    }

    public enum Attackable
    {
        PLAYER = 1,
        ENEMIES = 2,
        EVERYTHING = 4
    }
}
