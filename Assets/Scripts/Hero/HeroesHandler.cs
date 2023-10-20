using System.Collections.Generic;
using MissionMap.Util;
using UnityEngine;

namespace MissionMap.Hero
{
    public class HeroesHandler : Singleton<HeroesHandler>
    {
        [SerializeField] private List<Hero> _heroes;

        public Hero CurrentSelected { get; private set; }

        private void Start()
        {
            foreach (Hero hero in _heroes)
            {
                hero.OnHeroClicked += HeroClicked;

                if (hero.IsEnable == false)
                    hero.gameObject.SetActive(false);
            }
        }

        public void UnlockHeroes(IEnumerable<HeroType> heroesType)
        {
            foreach (HeroType heroType in heroesType)
            {
                foreach (Hero hero in _heroes)
                    if (hero.Type == heroType)
                        hero.IsEnable = true;
            }
        }

        public void SetReward(IEnumerable<KeyValuePair<HeroType,int>> rewards)
        {
            foreach (KeyValuePair<HeroType,int> reward in rewards)
            {
                if (reward.Key == HeroType.Myself)
                    CurrentSelected.ChangePoint(reward.Value);

                foreach (Hero hero in _heroes)
                    if (hero.Type == reward.Key
                        && (reward.Value < 0 || hero.IsEnable))
                        hero.ChangePoint(reward.Value);
            }
        }

        private void HeroClicked(Hero hero)
        {
            if (CurrentSelected == hero)
            {
                CurrentSelected.IsChosen = false;
                CurrentSelected = null;
                return;
            }

            if (CurrentSelected != null)
                CurrentSelected.IsChosen = false;

            CurrentSelected = hero;
            CurrentSelected.IsChosen = true;
        }

        public bool IsLock(HeroType heroType)
        {
            foreach (Hero hero in _heroes)
                if (hero.Type == heroType
                    && hero.IsEnable == false)
                    return true;

            return false;
        }
    }
}