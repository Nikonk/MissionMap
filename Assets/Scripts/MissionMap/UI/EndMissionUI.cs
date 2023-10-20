using System;
using System.Collections.Generic;
using System.Linq;
using MissionMap.Hero;
using TMPro;
using Unity.VisualScripting;
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

        private HeroesHandler _heroesHandler;

        public event Action OnCompleteMission;

        public void Initialize(string missionName,
                               string missionText,
                               IReadOnlyList<IReadOnlyList<HeroType>> playingFor,
                               IReadOnlyList<IReadOnlyList<HeroType>> playingAgainst)
        {
            _missionName.text = missionName;
            _missionText.text = missionText;

            _playingFor.text = string.Empty;
            _playingAgainst.text = string.Empty;

            _heroesHandler = HeroesHandler.Instance;

            if (playingFor.Count > 1)
                EndMissionWithChoice(playingFor, playingAgainst);
            else
                EndMissionWithoutChoice(playingFor, playingAgainst);
        }

        private void EndMissionWithoutChoice(
            IReadOnlyList<IReadOnlyList<HeroType>> playingFor,
            IReadOnlyList<IReadOnlyList<HeroType>> playingAgainst)
        {
            if (playingFor.Count != 0)
                foreach (HeroType heroType in playingFor[0])
                    _playingFor.text += heroType + " ";

            if (playingAgainst.Count != 0)
                foreach (HeroType heroType in playingAgainst[0])
                    _playingAgainst.text += heroType + " ";
        }

        private void EndMissionWithChoice(
            IReadOnlyList<IReadOnlyList<HeroType>> playingFor,
            IReadOnlyList<IReadOnlyList<HeroType>> playingAgainst)
        {
            int lockHeroIndex = -1;

            foreach (IReadOnlyList<HeroType> heroTypes in playingFor)
                for (int i = 0; i < heroTypes.Count; i++)
                    if (_heroesHandler.IsLock(heroTypes[i]))
                    {
                        lockHeroIndex = i;
                        break;
                    }

            if (lockHeroIndex == -1)
                Debug.LogError("Bad mission playingFor and playingAgainst data");

            foreach (HeroType heroType in playingFor[lockHeroIndex])
                _playingFor.text += heroType + " ";

            foreach (HeroType heroType in playingAgainst[lockHeroIndex])
                _playingAgainst.text += heroType + " ";
        }

        private void Start() =>
            _completeMissionButton.onClick.AddListener(() => OnCompleteMission?.Invoke());
    }
}