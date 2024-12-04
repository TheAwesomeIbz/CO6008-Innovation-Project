using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Entities
{
    public enum Attackable
    {
        NONE = 0,
        PLAYER = 1,
        ENEMIES = 2,
        EVERYTHING = 4
    }

    public interface iDodgeable
    {
        public bool IsDodging { get; }
    }
}
