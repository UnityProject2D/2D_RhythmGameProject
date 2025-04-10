using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ButtonEnablerAfterDelay : MonoBehaviour
{
    [SerializeField] private float _delay = 1f;

    private Button _button;

    private void Awake()
    {
        _button = GetComponent<Button>();
        if (_button == null)
        {
            Debug.LogError("Button component not found on this GameObject.");
            return;
        }
    }

    private void Start()
    {
        _button.interactable = false;
        StartCoroutine(EnableButtonAfterDelay());
    }

    private IEnumerator EnableButtonAfterDelay()
    {
        yield return new WaitForSeconds(_delay);
        _button.interactable = true;
    }
}
