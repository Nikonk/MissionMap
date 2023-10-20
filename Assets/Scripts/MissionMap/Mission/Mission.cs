using System;
using System.Collections.Generic;
using System.Linq;
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

        private List<List<Mission>> _parentMissions;
        private List<Mission> _childMissions;

        private List<MissionState> _states;

        private Color _initialBackgroundColor;

        public Mission Initialize(List<MissionMapNodeData> missionsData)
        {
            _missionsData = missionsData;
            _parentMissions = new List<List<Mission>>();
            _childMissions = new List<Mission>();
            _states = new List<MissionState>();
            _initialBackgroundColor = _backgroundImage.color;

            foreach (MissionMapNodeData missionData in _missionsData)
                _states.Add(missionData.IsActive ? MissionState.Active : MissionState.Block);

            CheckEnable();

            for (int i = 0; i < missionsData.Count; i++)
                _codeText[i].text = missionsData[i].Code;

            return this;
        }

        public event Action<Mission> OnMissionClick;

        public IReadOnlyList<MissionMapNodeData> MissionsData => _missionsData;

        public List<Mission> ChildMissions => _childMissions;
        public List<List<Mission>> ParentMissions => _parentMissions;

        public bool IsVisible => State != MissionState.Block;
        public bool IsComplete => State == MissionState.Complete;

        private MissionState State
        {
            get
            {
                if (_states.All(state => state == MissionState.Active))
                    return MissionState.Active;

                if (_states.All(state => state.HasFlag(MissionState.TemporarilyBlock)))
                    return MissionState.TemporarilyBlock;

                if (_states.Contains(MissionState.Complete))
                    return MissionState.Complete;

                return MissionState.Block;
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (State == MissionState.Active)
                OnMissionClick?.Invoke(this);
        }

        public void SetPosition() =>
            transform.position += new Vector3(_missionsData[0].NodePosition.X, _missionsData[0].NodePosition.Y);

        public void SetState(MissionState state)
        {
            for (var i = 0; i < _states.Count; i++)
                _states[i] = state;

            CheckEnable();
        }

        public void SetState(MissionState state, int missionChoiceIndex)
        {
            if (_states[missionChoiceIndex] == state)
                return;

            _states[missionChoiceIndex] = state;

            CheckEnable();
        }

        public void AddState(MissionState state)
        {
            for (var i = 0; i < _states.Count; i++)
                _states[i] |= state;

            CheckEnable();
        }

        public bool TryUnlock()
        {
            for (int i = 0; i < _missionsData[0].ParentMissionCodes.Count; i++)
            {
                bool isOneMissionComplete = false;

                for (int j = 0; j < _missionsData[0].ParentMissionCodes[i].Count; j++)
                {
                    for (var missionIndex = 0; missionIndex < _parentMissions[i][j].MissionsData.Count; missionIndex++)
                    {
                        var missionData = _parentMissions[i][j].MissionsData[missionIndex];
                        if (_missionsData[0].ParentMissionCodes[i][j] == missionData.Code
                            && _parentMissions[i][j]._states[missionIndex] == MissionState.Complete)
                        {
                            isOneMissionComplete = true;
                            break;
                        }
                    }
                }

                if (isOneMissionComplete == false)
                    return false;
            }

            SetState(MissionState.Active);
            return true;
        }

        private void CheckEnable()
        {
            switch (State)
            {
                case MissionState.Block:
                    gameObject.SetActive(false);
                    break;

                case MissionState.Complete:
                case MissionState.TemporarilyBlock:
                    gameObject.SetActive(true);
                    _backgroundImage.color = _blockColor;
                    break;

                case MissionState.Active:
                    gameObject.SetActive(true);
                    _backgroundImage.color = _initialBackgroundColor;
                    break;

                default:
                    Debug.LogError("MissionState enum has more cases than checked");
                    break;
            }
        }
    }
}