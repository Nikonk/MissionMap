using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MissionMap.Core
{
    public class DoubleMissionStartUI : MonoBehaviour
    {
        private const int _firstMissionIndex = 0;
        private const int _secondMissionIndex = 1;

        [SerializeField] private TMP_Text _firstMissionName;
        [SerializeField] private TMP_Text _firstPreMissionText;
        [SerializeField] private Button _startFirstMissionButton;

        [SerializeField] private TMP_Text _secondMissionName;
        [SerializeField] private TMP_Text _secondPreMissionText;
        [SerializeField] private Button _startSecondMissionButton;

        public event Action<int> OnStartMission;

        public void Initialize(string firstMissionName, string firstPreMissionText,
                               string secondMissionName, string secondPreMissionText)
        {
            _firstMissionName.text = firstMissionName;
            _firstPreMissionText.text = firstPreMissionText;

            _secondMissionName.text = secondMissionName;
            _secondPreMissionText.text = secondPreMissionText;
        }

        private void Start()
        {
            _startFirstMissionButton.onClick.AddListener(() => OnStartMission?.Invoke(_firstMissionIndex));
            _startSecondMissionButton.onClick.AddListener(() => OnStartMission?.Invoke(_secondMissionIndex));
        }
    }
}