using UnityEngine;
using UnityEngine.UI;
using TMPro;

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

    [Header("재화 아이콘 모음")]
    [SerializeField] private Sprite[] sprites;

    private bool _isVisible;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        _tooltipRoot.SetActive(false);
    }
    void Update()
    {
        if (_isVisible)
        {
            Vector2 mousePos = Input.mousePosition;
            _tooltipRoot.transform.position = mousePos + new Vector2(200f, -16f);
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
