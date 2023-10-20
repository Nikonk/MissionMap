using System;
using System.Collections.Generic;
using MissionMap.Hero;

namespace MissionMap.Core
{
    [Serializable]
    public struct MissionMapNodeData
    {
        public string Code;

        public bool IsActive;

        public string MissionName;
        public string PreMissionText;
        public string EndMissionText;

        public List<List<HeroType>> PlayingFor;
        public List<List<HeroType>> PlayingAgainst;

        public List<HeroType> UnlockCharacter;
        public Dictionary<HeroType, int> Reward;

        public List<List<string>> ParentMissionCodes;
        public List<string> TemporarilyBlockMission;

        public MissionMapPosition NodePosition;

        public MissionMapNodeData(
            string code,
            string missionName, string preMissionText, string endMissionText,
            bool isActive,
            List<List<HeroType>> playingFor, List<List<HeroType>> playingAgainst,
            List<HeroType> unlockCharacter, Dictionary<HeroType, int> reward,
            List<List<string>> parentMissionCodes, List<string> temporarilyBlockMission,
            MissionMapPosition nodePosition)
        {
            Code = code;

            IsActive = isActive;

            MissionName = missionName;
            PreMissionText = preMissionText;
            EndMissionText = endMissionText;

            PlayingFor = playingFor;
            PlayingAgainst = playingAgainst;

            UnlockCharacter = unlockCharacter;
            Reward = reward;

            ParentMissionCodes = parentMissionCodes;
            TemporarilyBlockMission = temporarilyBlockMission;

            NodePosition = nodePosition;
        }
    }
}