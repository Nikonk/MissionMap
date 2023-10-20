using System;

namespace MissionMap.Core
{
    [Flags]
    public enum MissionState
    {
        Active = 1 << 0,
        Block = 1 << 1,
        TemporarilyBlock = 1 << 2,
        Complete = 1 << 3
    }
}