using MoreMountains.Tools;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody2D))]
public class ShopPlayerController : MonoBehaviour
{
    public float moveSpeed = 2f;
    public float jumpForce = 10f;
    public Transform groundCheck;
    public LayerMask groundLayer;
    public Animator anim;
    private Rigidbody2D rb;
    private @PlayerInputAction inputActions;
    private Vector2 moveInput;
    private bool isJumpPressed;
    private bool isGrounded;
    private bool ShopActive = false;
    public GameObject ShopUI;

    public string sceneToLoad;
    public ShopHelper ShopHelper;
    public DummyDoorController DummyController;
    private void Awake()
    {

        inputActions = new @PlayerInputAction();
        rb = GetComponent<Rigidbody2D>();

        // Move 입력
        inputActions.Shop_Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        inputActions.Shop_Player.Move.canceled += ctx => moveInput = Vector2.zero;

        // Jump 입력
        inputActions.Shop_Player.Jump.performed += ctx => isJumpPressed = true;

        inputActions.Shop_Player.UseShop.performed += PerformedE;
    }

    private void PerformedE(InputAction.CallbackContext callbackContext)
    {
        if (ShopHelper.IsShop)
        {
            ShopActive = !ShopActive;
            ShopUI.SetActive(ShopActive);
            if (ShopActive == false)
            {
                TooltipUI.Instance.Hide();
            }
        }
        else if (DummyController.IsExit)
        {
            GameSceneManager.Instance.ChangeScene(sceneToLoad);
        }
    }

    private void OnEnable()
    {
        inputActions.Shop_Player.Enable();
    }

    private void OnDisable()
    {
        inputActions.Shop_Player.Disable();
    }

    private void Update()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.05f, groundLayer);

        if (isJumpPressed && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            isJumpPressed = false;
        }
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(moveInput.x * moveSpeed, rb.linearVelocity.y);
        anim.SetBool("Moving", Mathf.Abs(moveInput.x) > 0.01f);
        anim.SetBool("Jump", !isGrounded);
        // 캐릭터 좌우 반전
        if (moveInput.x != 0)
            transform.localScale = new Vector3(Mathf.Sign(moveInput.x), transform.localScale.y, transform.localScale.z);
    }
}
