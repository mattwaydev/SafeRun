using UnityEngine;

namespace SafeRun.Entities
{
    [DisallowMultipleComponent]
    public class CharacterSwitcher : MonoBehaviour
    {
        [System.Serializable]
        public class Skin
        {
            public string id = "skin";
            public RuntimeAnimatorController controller;
            public Sprite defaultSprite;
        }

        public const string PrefsKey = "SafeRun.PersonajeIndex";

        [SerializeField] private Skin[] skins;
        [SerializeField] private int initialIndex = 0;
        [SerializeField] private bool useSavedSelection = true;

        [SerializeField] private Animator animator;
        [SerializeField] private SpriteRenderer spriteRenderer;

        private int _currentIndex = -1;

        public int CurrentIndex => _currentIndex;
        public string CurrentId => (_currentIndex >= 0 && _currentIndex < skins.Length) ? skins[_currentIndex].id : null;

        private void Reset()
        {
            animator = GetComponent<Animator>();
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        private void Awake()
        {
            if (animator == null) animator = GetComponent<Animator>();
            if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();

            var idx = initialIndex;
            if (useSavedSelection && PlayerPrefs.HasKey(PrefsKey))
                idx = PlayerPrefs.GetInt(PrefsKey, initialIndex);
            Apply(idx);
        }

        public static void GuardarSeleccion(int index)
        {
            PlayerPrefs.SetInt(PrefsKey, index);
            PlayerPrefs.Save();
        }

        public static int CargarSeleccion(int fallback = 0)
        {
            return PlayerPrefs.GetInt(PrefsKey, fallback);
        }

        public void Apply(int index)
        {
            if (skins == null || skins.Length == 0) return;
            index = Mathf.Clamp(index, 0, skins.Length - 1);
            if (index == _currentIndex) return;

            var skin = skins[index];
            if (animator != null && skin.controller != null)
                animator.runtimeAnimatorController = skin.controller;
            if (spriteRenderer != null && skin.defaultSprite != null)
                spriteRenderer.sprite = skin.defaultSprite;

            _currentIndex = index;
        }

        public bool Apply(string id)
        {
            if (skins == null) return false;
            for (var i = 0; i < skins.Length; i++)
            {
                if (skins[i].id == id)
                {
                    Apply(i);
                    return true;
                }
            }
            return false;
        }

        public void Next() => Apply((_currentIndex + 1) % Mathf.Max(1, skins.Length));
    }
}
