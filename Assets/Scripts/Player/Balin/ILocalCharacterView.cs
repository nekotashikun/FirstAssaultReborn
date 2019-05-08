using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;

namespace Scripts.Player.Balin
{
    public interface ILocalCharacterView
    {
        Vector3 Position { get; }
        Vector3 LookRotation { get; }
    }
}
