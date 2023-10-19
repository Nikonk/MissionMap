using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MissionMap.Core
{
    public class SingleMissionStartUI : MonoBehaviour
    {
        [SerializeField] private TMP_Text _missionName;
        [SerializeField] private TMP_Text _preMissionText;
        [SerializeField] private Button _startMissionButton;

        public event Action OnStartMission;

        public void Initialize(string missionName, string preMissionText)
        {
            _missionName.text = missionName;
            _preMissionText.text = preMissionText;
        }

        private void Start() =>
            _startMissionButton.onClick.AddListener(() => OnStartMission?.Invoke());
    }
}