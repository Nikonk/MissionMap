using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MissionMap.Core
{
    public class Mission : MonoBehaviour, IPointerDownHandler
    {
        [SerializeField] private Color _blockColor;
        [SerializeField] private Image _backgroundImage;
        [SerializeField] private List<TMP_Text> _codeText;

        private List<MissionMapNodeData> _missionsData;

        private List<Mission> _parentMissions;
        private List<Mission> _childMissions;

        private List<MissionState> _states;

        private Color _initialBackgroundColor;

        public Mission Initialize(List<MissionMapNodeData> missionsData)
        {
            _missionsData = missionsData;
            _parentMissions = new List<Mission>();
            _childMissions = new List<Mission>();
            _states = new List<MissionState>();

            foreach (MissionMapNodeData missionData in _missionsData)
                _states.Add(missionData.IsActive ? MissionState.Active : MissionState.Block);

            for (int i = 0; i < missionsData.Count; i++)
                _codeText[i].text = missionsData[i].Code;

            return this;
        }

        public event Action<Mission> OnMissionClick;

        public IReadOnlyList<MissionMapNodeData> MissionsData => _missionsData;

        public List<Mission> ChildMissions => _childMissions;
        public List<Mission> ParentMissions => _parentMissions;

        public MissionState State
        {
            get
            {
                var resultState = MissionState.Block;

                if (_states.All(state => state == MissionState.Active))
                    resultState = MissionState.Active;

                if (_states.All(state => state == MissionState.TemporarilyBlock))
                    resultState = MissionState.TemporarilyBlock;

                if (_states.Contains(MissionState.Complete))
                    resultState = MissionState.Complete;

                return resultState;
            }
        }

        private List<MissionState> States => _states;

        public void OnPointerDown(PointerEventData eventData)
        {
            if (_states.All(state => state == MissionState.Active))
                OnMissionClick?.Invoke(this);
        }

        private void Start() => _initialBackgroundColor = _backgroundImage.color;

        public void SetState(MissionState state, int missionChoiceIndex)
        {
            if (_states[missionChoiceIndex] == state)
                return;

            _states[missionChoiceIndex] = state;

            switch (State)
            {
                case MissionState.Active:
                    gameObject.SetActive(true);
                    _backgroundImage.color = _initialBackgroundColor;
                    break;

                case MissionState.Block:
                    gameObject.SetActive(false);
                    break;

                case MissionState.Complete:
                case MissionState.TemporarilyBlock:
                    _backgroundImage.color = _blockColor;
                    break;

                default:
                    Debug.LogError("MissionState enum has more cases than checked");
                    break;
            }
        }

        [CanBeNull]
        public List<Mission> TryUnlockChildAndReturnUnlocked() =>
            _childMissions.Where(childMission => childMission.TryUnlock()).ToList();

        private bool TryUnlock()
        {
            foreach (string missionParentCode in _missionsData[0].ParentMissionCode)
                foreach (Mission parentMission in _parentMissions)
                    for (var i = 0; i < parentMission.MissionsData.Count; i++)
                        if (missionParentCode == parentMission.MissionsData[i].Code)
                            if (parentMission._states[i] != MissionState.Complete)
                                return false;

            for (var i = 0; i < _states.Count; i++)
                _states[i] = MissionState.Active;

            return true;
        }
    }
}