using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MissionMap.Hero
{
    public class Hero : MonoBehaviour, IPointerDownHandler
    {
        [Header("Hero info")]
        [SerializeField] private string _name;
        [SerializeField] private int _point;
        [SerializeField] private HeroType _type;
        [SerializeField] private bool _isEnable;


        [Header("Hero UI")]
        [SerializeField] private TMP_Text _nameText;
        [SerializeField] private TMP_Text _pointText;


        [Header("Selection")]
        [SerializeField] private Image _heroInfoBackground;
        [SerializeField] private Color _selectionColor;

        private bool _isChosen;

        private Color _defaultHeroInfoBackgroundColor;

        public event Action<Hero> OnHeroClicked;

        public HeroType Type => _type;

        public bool IsEnable
        {
            get => _isEnable;
            set
            {
                _isEnable = value;
                gameObject.SetActive(_isEnable);
            }
        }

        public bool IsChosen
        {
            set
            {
                _isChosen = value;

                _heroInfoBackground.color = _isChosen ? _selectionColor : _defaultHeroInfoBackgroundColor;
            }
        }

        public void OnPointerDown(PointerEventData eventData) => OnHeroClicked?.Invoke(this);

        private void Start()
        {
            _nameText.text = _name;
            _pointText.text = _point > 0 ? $"+{_point.ToString()}" : _point.ToString();

            if (_isEnable == false)
                gameObject.SetActive(false);

            _defaultHeroInfoBackgroundColor = _heroInfoBackground.color;
        }

        public void ChangePoint(int point)
        {
            _point += point;
            _pointText.text = _point > 0 ? $"+{_point.ToString()}" : _point.ToString();
        }
    }
}