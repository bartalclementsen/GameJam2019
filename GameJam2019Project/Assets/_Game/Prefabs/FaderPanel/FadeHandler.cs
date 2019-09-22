using Core.Loggers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Fading
{
    public enum FadeDirection
    {
        FromBlack, ToBlack, Toggle
    }

    public interface IFadeService
    {
        void SetFadeTime(float newTime);

        void DoFade(FadeDirection direction, float delaySeconds = 0f);
    }

    public class FadeService : IFadeService
    {
        private readonly Core.Loggers.ILogger _logger;

        internal static FadeHandler ActiveHandler { get; set; } = null;
        internal static float? FadeTime { get; private set; } = null;

        public FadeService(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.Create(this);
        }

        public void SetFadeTime(float newTime)
        {
            FadeTime = newTime;
            if (ActiveHandler != null)
            {
                ActiveHandler.TimeToFade = newTime;
            }
        }

        public void DoFade(FadeDirection direction, float delaySeconds = 0f)
        {
            switch (direction)
            {
                case FadeDirection.FromBlack:
                    ActiveHandler?.Invoke(nameof(FadeHandler.FromBlack), delaySeconds);
                    break;
                case FadeDirection.ToBlack:
                    ActiveHandler?.Invoke(nameof(FadeHandler.ToBlack), delaySeconds);
                    break;
                case FadeDirection.Toggle:
                    ActiveHandler?.Invoke(nameof(FadeHandler.ToBlack), delaySeconds);
                    break;
                default:
                    break;
            }
        }

        bool FadeToggle()
        {
            if (ActiveHandler == null)
            {
                return false;
            }

            _logger.Log("Toggling fade");

            ActiveHandler.FromBlack();
            return true;
        }

        bool FadeFromBlack()
        {
            if (ActiveHandler == null)
            {
                return false;
            }

            _logger.Log("Fading from black");

            ActiveHandler.FromBlack();
            return true;
        }

        bool FadeToBlack()
        {
            if (ActiveHandler == null)
            {
                return false;
            }

            _logger.Log("Fading to black");

            ActiveHandler.ToBlack();
            return true;
        }
    }

    public class FadeHandler : MonoBehaviour
    {
        [SerializeField]
        private bool _startsDark;

        public bool DoToggleFade;
        public bool DoFadeFromBlack;
        public bool DoFadeToBlack;
        public float TimeToFade;

        private float _transparency;
        private bool _fading;
        private bool _isDarkening;

        private Image _image;

        private void Awake()
        {
            Fading.FadeService.ActiveHandler = this;
        }

        // Start is called before the first frame update
        void Start()
        {
            _image = transform.GetComponent<Image>();
            Init();

            TimeToFade = FadeService.FadeTime ?? TimeToFade;
        }

        private void OnDestroy()
        {
            Fading.FadeService.ActiveHandler = null;
        }

        private void Init()
        {
            _isDarkening = !_startsDark;
            _transparency = _isDarkening ? 1f : 0f;

            _image.color = _startsDark ? Color.black : Color.clear;
        }

        public void Toggle()
        {
            _fading = true;
        }

        public void ToBlack()
        {
            DoToggleFade = false;
            _startsDark = false;
            Init();
            _fading = true;
        }

        public void FromBlack()
        {
            DoToggleFade = false;
            _startsDark = true;
            Init();
            _fading = true;
        }

        void CheckInspectorButtons()
        {
            if (!_fading)
            {
                if (DoToggleFade)
                {
                    _fading = true;
                }
                else if (DoFadeToBlack)
                {
                    ToBlack();
                }
                else if (DoFadeFromBlack)
                {
                    FromBlack();
                }
            }

            DoToggleFade = false;
            DoFadeFromBlack = false;
            DoFadeToBlack = false;
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            if (!_fading)
                CheckInspectorButtons();
            else
            {
                _transparency = !_isDarkening ?
                    _transparency + (TimeToFade * Time.fixedDeltaTime)
                    : _transparency - (TimeToFade * Time.fixedDeltaTime);

                _image.color = Color.Lerp(Color.black, Color.clear, _transparency);

                Debug.Log(_transparency);

                if (_transparency < 0f || _transparency > 1f)
                {
                    _isDarkening = _transparency >= 1f;

                    _transparency = Mathf.Clamp01(_transparency);

                    _fading = false;
                }
            }

        }

    }
}
