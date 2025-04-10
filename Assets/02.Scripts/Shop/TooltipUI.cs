using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;

public class TooltipUI : MonoBehaviour
{
    public static TooltipUI Instance { get; private set; }

    [Header("UI 요소")]
    [SerializeField] private GameObject _tooltipRoot;
    [SerializeField] private TMP_Text _nameText;
    [SerializeField] private TMP_Text _descriptionText;
    [SerializeField] private TMP_Text _categoryText;
    [SerializeField] private TMP_Text _priceText;
    [SerializeField] private Image _currencyIcon;
    [SerializeField] private Image _iconImage;
    [SerializeField] private GameObject _priceTextWrapper;

    [Header("재화 아이콘 모음")]
    [SerializeField] private Sprite[] sprites;

    private bool _isVisible;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);

        _tooltipRoot.SetActive(false);
    }
    private void OnEnable()
    {
        SceneManager.sceneLoaded += DestroyOnRestart; // 추후 SceneCleanupHandler로 분리 예정
    }
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= DestroyOnRestart;
    }

    private void DestroyOnRestart(Scene scene, LoadSceneMode loadSceneMode)
    {
        if (scene.name == "GameTitle")
        {
            Destroy(gameObject);
        }
    }
    void Update()
    {
        if (_isVisible)
        {
            Vector2 mousePos = Input.mousePosition;
            _tooltipRoot.transform.position = mousePos + new Vector2(200f, 50f);
        }
    }
    public void Show(ShopItemSO item)
    {
        _nameText.text = item.itemSO.itemName;
        _descriptionText.text = item.itemSO.description;
        _categoryText.text = item.itemSO.category != null ? item.itemSO.category.categoryName : "";
        _priceText.text = item.price.ToString();
        _currencyIcon.sprite = sprites[(int)item.currencyType];
        _iconImage.sprite = item.itemSO.icon;
        _priceText.gameObject.SetActive(true);
        _priceTextWrapper.SetActive(true);
        _currencyIcon.enabled = true;
        _tooltipRoot.SetActive(true);
        _isVisible = true;
    }
    public void Show(ItemSO item)
    {
        _nameText.text = item.itemName;
        _descriptionText.text = item.EffectDescription;
        _categoryText.text = item.category != null ? item.category.categoryName : "";
        _currencyIcon.enabled = false;
        _iconImage.sprite = item.icon;

        _priceText.gameObject.SetActive(false);
        _priceTextWrapper.SetActive(false);
        _tooltipRoot.SetActive(true);
        _isVisible = true;
    }

    public void Hide()
    {
        _tooltipRoot.SetActive(false);
        _isVisible = false;
    }

    public void SetPosition(Vector2 screenPosition)
    {
        _tooltipRoot.transform.position = screenPosition;
    }
}
