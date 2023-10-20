using System.Collections.Generic;
using MissionMap.Bootstrap;
using MissionMap.Hero;
using UnityEngine;

namespace MissionMap.Core
{
    public class MissionHandler : MonoBehaviour
    {
        [SerializeField] private SingleMissionStartUI _singleMissionStartUIPrefab;
        [SerializeField] private DoubleMissionStartUI _doubleMissionStartUIPrefab;
        [SerializeField] private EndMissionUI _endMissionUIPrefab;

        [SerializeField] private Mission _singleMissionPrefab;
        [SerializeField] private Mission _doubleMissionPrefab;

        [SerializeField] private Transform _uiParentTransform;
        [SerializeField] private Transform _scrollViewContentTransform;

        private SingleMissionStartUI _singleMissionStartUI;
        private DoubleMissionStartUI _doubleMissionStartUI;
        private EndMissionUI _endMissionUI;

        private readonly List<Mission> _missions = new();
        private Mission _currentMission;

        private HeroesHandler _heroesHandler;

        private void Awake()
        {
            _singleMissionStartUI = Instantiate(_singleMissionStartUIPrefab, _uiParentTransform);
            _singleMissionStartUI.gameObject.SetActive(false);
            _doubleMissionStartUI = Instantiate(_doubleMissionStartUIPrefab, _uiParentTransform);
            _doubleMissionStartUI.gameObject.SetActive(false);
            _endMissionUI = Instantiate(_endMissionUIPrefab, _uiParentTransform);
            _endMissionUI.gameObject.SetActive(false);
        }

        private void Start()
        {
            InitializeMissions();
            FillChildAndParentMissions();
            TemporarilyBlockVisibleMission();

            _heroesHandler = HeroesHandler.Instance;
        }

        private void OnEnable()
        {
            _singleMissionStartUI.OnStartMission += MissionStart;
            _doubleMissionStartUI.OnStartMission += MissionStart;
            _endMissionUI.OnCompleteMission += CompleteMission;
        }

        private void OnDisable()
        {
            _singleMissionStartUI.OnStartMission -= MissionStart;
            _doubleMissionStartUI.OnStartMission -= MissionStart;
            _endMissionUI.OnCompleteMission -= CompleteMission;
        }

        private void ShowMissionStartUI(Mission mission)
        {
            _currentMission = mission;

            if (mission.MissionsData.Count == 1)
                ShowCloseSingleMissionStartUI(mission.MissionsData[0]);
            else
                ShowCloseDoubleMissionStartUI(mission.MissionsData);
        }

        private void ShowCloseSingleMissionStartUI(MissionMapNodeData missionData)
        {
            _doubleMissionStartUI.gameObject.SetActive(false);

            _singleMissionStartUI.Initialize(missionData.MissionName, missionData.PreMissionText);

            _singleMissionStartUI.gameObject.SetActive(!_singleMissionStartUI.gameObject.activeInHierarchy);
        }

        private void ShowCloseDoubleMissionStartUI(IReadOnlyList<MissionMapNodeData> missionsData)
        {
            _singleMissionStartUI.gameObject.SetActive(false);

            _doubleMissionStartUI.Initialize(
                missionsData[0].MissionName, missionsData[0].PreMissionText,
                missionsData[1].MissionName, missionsData[1].PreMissionText);

            _doubleMissionStartUI.gameObject.SetActive(!_doubleMissionStartUI.gameObject.activeInHierarchy);
        }

        private void MissionStart() => MissionStart(0);

        private void MissionStart(int missionDataIndex)
        {
            if (_heroesHandler.CurrentSelected == null)
                return;

            _singleMissionStartUI.gameObject.SetActive(false);
            _doubleMissionStartUI.gameObject.SetActive(false);

            MissionMapNodeData currentMissionData = _currentMission.MissionsData[missionDataIndex];
            _endMissionUI.Initialize(
                currentMissionData.MissionName,
                currentMissionData.EndMissionText,
                currentMissionData.PlayingFor,
                currentMissionData.PlayingAgainst);

            _currentMission.SetState(MissionState.Complete, missionDataIndex);
            RewardHeroes(missionDataIndex);
            UnlockChildMissions();
            TemporarilyUnblockMission(_currentMission.MissionsData[missionDataIndex]);
            TemporarilyBlockVisibleMission();

            _endMissionUI.gameObject.SetActive(true);
        }

        private void CompleteMission()
        {
            _endMissionUI.gameObject.SetActive(false);
        }

        private void TemporarilyBlockVisibleMission()
        {
            foreach (Mission mission in _missions)
            {
                if (mission.IsVisible == false
                    || mission.IsComplete)
                    continue;

                foreach (MissionMapNodeData missionData in mission.MissionsData)
                {
                    if (missionData.TemporarilyBlockMission == null)
                        continue;

                    foreach (string temporarilyBlockMissionCode in missionData.TemporarilyBlockMission)
                    {
                        Mission findMission = _missions.Find(findMission =>
                            findMission.MissionsData[0].Code == temporarilyBlockMissionCode);

                        if (findMission != null && findMission.IsVisible)
                            findMission.AddState(MissionState.TemporarilyBlock);
                    }
                }
            }
        }

        private void TemporarilyUnblockMission(MissionMapNodeData missionData)
        {
            if (missionData.TemporarilyBlockMission == null)
                return;

            foreach (string temporarilyBlockMissionCode in missionData.TemporarilyBlockMission)
            {
                Mission findMission = _missions.Find(findMission =>
                    findMission.MissionsData[0].Code == temporarilyBlockMissionCode);

                if (findMission != null
                    && findMission.IsVisible
                    && findMission.IsComplete == false)
                    findMission.SetState(MissionState.Active);
            }
        }

        private void RewardHeroes(int missionDataIndex)
        {
            MissionMapNodeData currentMissionData = _currentMission.MissionsData[missionDataIndex];

            if (currentMissionData.UnlockCharacter.Count != 0)
                _heroesHandler.UnlockHeroes(currentMissionData.UnlockCharacter);

            if (currentMissionData.Reward.Count != 0)
                _heroesHandler.SetReward(currentMissionData.Reward);
        }

        private void UnlockChildMissions()
        {
            foreach (Mission childMission in _currentMission.ChildMissions)
                childMission.TryUnlock();
        }

        private void InitializeMissions()
        {
            for (var i = 0; i < DataHandler.Instance.MissionsData.Count; i++)
            {
                List<MissionMapNodeData> missionData = DataHandler.Instance.MissionsData[i];
                Mission mission;

                if (missionData.Count == 1)
                    mission = Instantiate(_singleMissionPrefab, _scrollViewContentTransform)
                        .Initialize(missionData);
                else
                    mission = Instantiate(_doubleMissionPrefab, _scrollViewContentTransform)
                        .Initialize(missionData);

                mission.SetPosition();

                mission.OnMissionClick += ShowMissionStartUI;

                _missions.Add(mission);
            }
        }

        private void FillChildAndParentMissions()
        {
            foreach (Mission mission in _missions)
            foreach (MissionMapNodeData missionsData in mission.MissionsData)
                if (missionsData.ParentMissionCode != null)
                    foreach (string parentMissionCode in missionsData.ParentMissionCode)
                    {
                        Mission parentMission =
                            _missions.Find(
                                findMission =>
                                {
                                    bool result = false;

                                    foreach (MissionMapNodeData findMissionData in findMission.MissionsData)
                                        result = result || findMissionData.Code == parentMissionCode;

                                    return result;
                                });

                        if (parentMission.ChildMissions.Contains(mission) == false)
                            parentMission.ChildMissions.Add(mission);

                        if (mission.ParentMissions.Contains(parentMission) == false)
                            mission.ParentMissions.Add(parentMission);
                    }
        }
    }
}