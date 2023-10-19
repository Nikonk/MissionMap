using System;
using System.Collections.Generic;
using MissionMap.Hero;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MissionMap.Core
{
    public class EndMissionUI : MonoBehaviour
    {
        [SerializeField] private TMP_Text _missionName;
        [SerializeField] private TMP_Text _missionText;
        [SerializeField] private TMP_Text _playingFor;
        [SerializeField] private TMP_Text _playingAgainst;
        [SerializeField] private Button _completeMissionButton;

        public event Action OnCompleteMission;

        public void Initialize(string missionName,
                               string missionText,
                               IEnumerable<IEnumerable<HeroType>> playingFor,
                               IEnumerable<IEnumerable<HeroType>> playingAgainst)
        {
            _missionName.text = missionName;
            _missionText.text = missionText;

            _playingFor.text = string.Empty;
            _playingAgainst.text = string.Empty;

            foreach (IEnumerable<HeroType> characterType in playingFor)
            {
                foreach (HeroType type in characterType)
                {
                    _playingFor.text += type + " ";
                }
            }

            foreach (IEnumerable<HeroType> characterType in playingAgainst)
            {
                foreach (HeroType type in characterType)
                {
                    _playingAgainst.text += type + " ";
                }
            }
        }

        private void Start() =>
            _completeMissionButton.onClick.AddListener(() => OnCompleteMission?.Invoke());
    }
}