using System.Collections.Generic;
using MissionMap.Core;
using MissionMap.Util;

namespace MissionMap.Bootstrap
{
    public class DataHandler : Singleton<DataHandler>
    {
        public List<List<MissionMapNodeData>> MissionsData { get; private set; }

        public void Initialize(List<List<MissionMapNodeData>> missionsData)
        {
            MissionsData = missionsData;
        }

        protected override DataHandler Initialize()
        {
            IsDontDestroyOnLoad = true;
            return this;
        }
    }
}