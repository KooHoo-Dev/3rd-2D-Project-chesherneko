using UnityEngine;
using Sirenix.OdinInspector;

public class PlayerMove : MonoBehaviour
{
    #region COMPONENT FIELDS
    [Title("Components")]
    [SerializeField] private Rigidbody2D body;
    [SerializeField] private new Collider2D collider;
    [SerializeField] private Afterimage afterImage;
    #endregion

    #region SETTING FIELDS
    //Move Settings
    [SerializeField, TitleGroup("Move")] private float maxMoveSpeed = 6.5f;
    [SerializeField, TitleGroup("Move")] private float maxFallSpeed = -20f;
    [SerializeField, TitleGroup("Move")] private float acceleration = 20f;
    [SerializeField, TitleGroup("Move")] private float deceleration = 30f;

    //Gravity Settings
    [SerializeField, TitleGroup("Gravity")] private float gravity = -9.81f;
    [SerializeField, TitleGroup("Gravity")] private float groundStickGravity = -2f;

    [Space]
    [SerializeField, TitleGroup("Gravity")] private float fallGravityMult = 4.25f;
    [SerializeField, TitleGroup("Gravity")] private float lateAscentGravityMult = 5.25f;

    //Jump Settings
    [SerializeField, TitleGroup("Jump")] private float jumpPower = 13f;
    [SerializeField, TitleGroup("Jump")] private int maxJumpCount = 2;

    [Space]
    [SerializeField, TitleGroup("Jump")] private float maxJumpHoldTime = 0.15f;

    [Space]
    [SerializeField, TitleGroup("Jump")] private float airJumpPowerMult = 1f;
    [SerializeField, TitleGroup("Jump")] private float jumpStartMoveSpeedMult = 0.75f;
    [SerializeField, TitleGroup("Jump")] private float minJumpHoldGravityMult = 2.5f;

    //Extra Jump Settings
    [SerializeField, TitleGroup("Extra Jump")] private float coyoteTimeLimit = 0.15f;
    [SerializeField, TitleGroup("Extra Jump")] private float jumpBufferTimeLimit = 0.15f;

    //Bonus Peak Settings
    [SerializeField, TitleGroup("Bonus Peak")] private float jumpHangThreshold = 0.5f;
    [SerializeField, TitleGroup("Bonus Peak")] private float jumpHangGravityMult = 0.5f;
    [SerializeField, TitleGroup("Bonus Peak")] private float jumpHangSpeedMult = 1.25f;

    //Wall Cling Settings
    [SerializeField, TitleGroup("Wall Cling")] private LayerMask wallCollisionLayer;
    [SerializeField, TitleGroup("Wall Cling")] private float wallCheckDistance = 0.05f;
    [SerializeField, TitleGroup("Wall Cling")] private float wallDetachInputTimeLimit = 0.5f;

    [Space]
    [SerializeField, TitleGroup("Wall Cling")] private float wallClingGravityMult = 0.5f;
    [SerializeField, TitleGroup("Wall Cling")] private float wallJumpMoveSpeedMult = 0.5f;

    //Dash Settings
    [SerializeField, TitleGroup("Dash")] private float dashSpeed = 20f;
    [SerializeField, TitleGroup("Dash")] private float dashDuration = 0.15f;

    [Space]
    [SerializeField, TitleGroup("Dash")] private float dashCooldown = 0.5f;

    [Space]
    [SerializeField, TitleGroup("Dash")] private float dashEndSpeedMult = 0.75f; //대시 시 기존 속도 감소배율

    //Collision Settings
    [SerializeField, TitleGroup("Collision")] private float skinWidth = 0.02f;
    [SerializeField, TitleGroup("Collision")] private float groundCheckDistance = 0.2f;
    [SerializeField, TitleGroup("Collision")] private LayerMask collisionLayer;
    #endregion

    #region RUNTIME STATE FIELDS
    //Collision States
    [PropertySpace]
    [ShowInInspector, ReadOnly, TitleGroup("Collision")] public bool IsGrounded { get; private set; } = false;

    //Move States
    [Space]
    [ShowInInspector, ReadOnly, TitleGroup("Move")] private Vector2 moveInput;
    [ShowInInspector, ReadOnly, TitleGroup("Move")] public float MoveX { get; private set; } = 0f;
    [ShowInInspector, ReadOnly, TitleGroup("Move")] public float MoveY { get; private set; } = 0f;
    [ShowInInspector, ReadOnly, TitleGroup("Move")] public float LastMoveDirection { get; private set; } = 1f;

    //Jump States
    [Space]
    [ShowInInspector, ReadOnly, TitleGroup("Jump")] private bool isJumpRequested = false;

    [ShowInInspector, ReadOnly, TitleGroup("Jump")] private bool isJumping = false;
    [ShowInInspector, ReadOnly, TitleGroup("Jump")] private int jumpCount = 0;

    [ShowInInspector, ReadOnly, TitleGroup("Jump")] private bool isJumpHold = false;
    [ShowInInspector, ReadOnly, TitleGroup("Jump")] private float jumpHoldTime = 0f;

    [Space]
    [ShowInInspector, ReadOnly, TitleGroup("Extra Jump")] private float coyoteTime = float.PositiveInfinity;
    [ShowInInspector, ReadOnly, TitleGroup("Extra Jump")] private float jumpBufferTime = float.PositiveInfinity;

    //Wall Cling States
    [ShowInInspector, ReadOnly, TitleGroup("Wall Cling")] public bool IsClingToWall { get; private set; } = false;
    [ShowInInspector, ReadOnly, TitleGroup("Wall Cling")] private float wallDetachInputTime = 0f;

    //Bonus Peak States
    [PropertySpace]
    [ShowInInspector, ReadOnly, TitleGroup("Bonus Peak")] private bool IsBonusPeakSection => Mathf.Abs(MoveY) < jumpHangThreshold;

    //Dash States
    [Space]
    [ShowInInspector, ReadOnly, TitleGroup("Dash")] private bool isDashRequested = false;
    [ShowInInspector, ReadOnly, TitleGroup("Dash")] private bool isDashOnCooldown = false;
    [ShowInInspector, ReadOnly, TitleGroup("Dash")] private float dashCooldownTime = 0f;

    [ShowInInspector, ReadOnly, TitleGroup("Dash")] private bool isDashing = false;
    [ShowInInspector, ReadOnly, TitleGroup("Dash")] private float dashTime = 0f;
    [ShowInInspector, ReadOnly, TitleGroup("Dash")] private float dashStartX = 0f;
    [ShowInInspector, ReadOnly, TitleGroup("Dash")] private float dashEndX = 0f;
    [ShowInInspector, ReadOnly, TitleGroup("Dash")] private float dashPreviousX = 0f;
    #endregion

    #region UNITY METHOD
    private void Update()
    {
        UpdateJumpBufferTime();
        UpdateCoyoteTime();

        UpdateJumpHoldTime();

        UpdateDashCooltime();

        UpdateWallDetachInputTime();
    }

    private void FixedUpdate()
    {
        IsGrounded = CheckGrounded();
        UpdateGroundedState();

        if (isDashRequested)
        {
            isDashRequested = false;
            ReadyDash();
        }

        if (isDashing)
        {
            ApplyDash();
            return;
        }

        if (IsClingToWall) ApplyWallCling();
        else ApplyMove();

        var (desiredMoveX, canMoveXAsDesired) = ApplyActualMove();

        if (IsClingToWall == false)
            TryEnterWallCling(desiredMoveX, canMoveXAsDesired);
    }
    #endregion

    #region INPUT/TIME UPDATE METHOD
    private void UpdateJumpBufferTime()
    {
        if (isJumpRequested == false) return;

        jumpBufferTime += Time.deltaTime;

        if (jumpBufferTime >= jumpBufferTimeLimit)
        {
            isJumpRequested = false;
            jumpBufferTime = float.PositiveInfinity;
        }
    }

    private void UpdateCoyoteTime()
    {
        if (isJumping) return;
        if (IsGrounded) return;

        coyoteTime += Time.deltaTime;
    }

    private void UpdateJumpHoldTime()
    {
        if (isJumping == false) return;
        if (isJumpHold == false) return;

        jumpHoldTime += Time.deltaTime;

        if (jumpHoldTime >= maxJumpHoldTime)
        {
            isJumpHold = false;
            jumpHoldTime = maxJumpHoldTime;
        }
    }

    private void UpdateDashCooltime()
    {
        if (isDashOnCooldown == false) return;
        if (isDashing) return;
        if (IsGrounded == false) return;

        dashCooldownTime += Time.deltaTime;

        if (dashCooldownTime >= dashCooldown)
        {
            dashCooldownTime = 0f;
            isDashOnCooldown = false;
        }
    }

    private void UpdateWallDetachInputTime()
    {
        if (IsClingToWall == false) return;

        float input = moveInput.x;
        bool isHoldingAwayFromWall = input == LastMoveDirection * -1f;

        if (isHoldingAwayFromWall == false)
        {
            wallDetachInputTime = 0f;
            return;
        }

        wallDetachInputTime += Time.deltaTime;

        if (wallDetachInputTime >= wallDetachInputTimeLimit)
            IsClingToWall = false;
    }
    #endregion

    #region STATE UPDATE METHOD
    private void UpdateGroundedState()
    {
        if (IsGrounded == false) return;
        if (MoveY > 0f) return;

        IsClingToWall = false;

        jumpCount = 0;
        isJumping = false;

        isJumpHold = false;
        jumpHoldTime = 0f;

        coyoteTime = 0f;
        jumpBufferTime = float.PositiveInfinity;

        if (isDashOnCooldown == false && dashCooldownTime > 0f)
            dashCooldownTime = 0f;
    }
    #endregion

    #region CORE MOVE METHOD
    private void ApplyMove()
    {
        ApplyHorizontalMove();

        float gravityMult = GetGravityMultiplier();
        ApplyGravity(gravityMult);

        TryApplyJump();
    }

    private (float desiredMoveX, bool canMoveXAsDesired) ApplyActualMove()
    {
        Vector2 desireMove = new Vector2(MoveX, MoveY) * Time.fixedDeltaTime;

        Bounds box = GetCastBox();

        float moveX = ResolveAxisMovement(desireMove.x, Vector2.right, box, out bool canMoveXAsDesired);
        box.center += Vector3.right * moveX;

        float moveY = ResolveAxisMovement(desireMove.y, Vector2.up, box, out bool canMoveYAsDesired);

        Vector2 move = new(moveX, moveY);

        if (canMoveYAsDesired == false) MoveY = 0f;

        body.MovePosition(body.position + move);

        return (desireMove.x, canMoveXAsDesired);
    }

    private float ResolveAxisMovement(float move, Vector2 axis, Bounds box, out bool canMoveAsDesired)
    {
        canMoveAsDesired = true;

        if (move == 0f) return 0f;

        Vector3 dir = move > 0f ? axis : -axis;
        float dist = Mathf.Abs(move);

        RaycastHit2D hit = Physics2D.BoxCast(box.center, box.size, 0f, dir, dist, collisionLayer);

        if (hit.collider == null) return move;

        canMoveAsDesired = false;
        float resolvedMove = Mathf.Max(0f, hit.distance - skinWidth);

        return Mathf.Sign(move) * resolvedMove;
    }
    #endregion

    #region HORIZONTAL MOVE METHOD
    private void ApplyHorizontalMove()
    {
        float dir = moveInput.x;
        bool isMoving = dir != 0f;

        float targetSpeed;

        float speedChangeRate = isMoving ? acceleration : deceleration;
        float maxDelta = speedChangeRate * Time.fixedDeltaTime;

        if (isMoving == false) targetSpeed = 0f;
        else
        {
            LastMoveDirection = dir;

            targetSpeed = maxMoveSpeed * dir;

            bool isTurnDir = MoveX != 0f && Mathf.Sign(MoveX) != dir;
            if (IsGrounded && isTurnDir) MoveX = 0f;

            if (isJumping && IsBonusPeakSection)
                targetSpeed *= jumpHangSpeedMult;
        }

        MoveX = Mathf.MoveTowards(MoveX, targetSpeed, maxDelta);
    }
    #endregion

    #region GRAVITY METHOD
    private void ApplyGravity(float gravityMult)
    {
        if (IsGrounded && MoveY <= 0f)
        {
            MoveY = groundStickGravity;
            return;
        }

        float gravity = this.gravity * gravityMult;

        MoveY += gravity * Time.fixedDeltaTime;
        MoveY = Mathf.Max(MoveY, maxFallSpeed);
    }

    private float GetGravityMultiplier()
    {
        if (isJumping == false)
            return MoveY > 0f ? 1f : fallGravityMult;

        float mult;

        if (isJumpHold) mult = 1f;
        else
        {
            if (MoveY > 0f)
            {
                float holdTime = Mathf.Min(jumpHoldTime, maxJumpHoldTime);

                float t = Mathf.InverseLerp(0f, maxJumpHoldTime, holdTime);
                float gravityMult = Mathf.Lerp(minJumpHoldGravityMult, 1f, t);

                mult = lateAscentGravityMult * gravityMult;
            }
            else mult = fallGravityMult;
        }

        if (IsBonusPeakSection)
            mult *= jumpHangGravityMult;

        return mult;
    }
    #endregion

    #region JUMP METHOD
    private void TryApplyJump()
    {
        if (CanJump() == false) return;

        coyoteTime = float.PositiveInfinity;
        if (jumpCount == 0)
        {
            dashCooldownTime = dashCooldown;
            isDashOnCooldown = false;
        }

        float moveSpeed = MoveX * jumpStartMoveSpeedMult;
        PerformJump(moveSpeed, true);
    }

    private void ApplyWallJump()
    {
        IsClingToWall = false;

        float dir = LastMoveDirection * -1f;
        float input = moveInput.x;

        float moveSpeedMult = input == dir ? jumpStartMoveSpeedMult : wallJumpMoveSpeedMult;
        float moveSpeed = maxMoveSpeed * moveSpeedMult * dir;

        PerformJump(moveSpeed, false);
    }

    private void PerformJump(float moveSpeed, bool canHoldJump)
    {
        isJumpRequested = false;
        isJumping = true;

        isJumpHold = canHoldJump && jumpCount == 0;

        if (isJumpHold == false)
            jumpHoldTime = maxJumpHoldTime;

        float power = jumpPower;
        float powerMult = jumpCount == 0 ? 1f : airJumpPowerMult;

        power *= powerMult;

        MoveX = moveSpeed;
        MoveY = power;

        jumpCount += 1;
    }

    private bool CanJump()
    {
        if (isJumpRequested == false) return false;
        if (IsGrounded) return true;
        if (coyoteTime <= coyoteTimeLimit) return true;
        if (isJumping == false) return false;

        return jumpCount < maxJumpCount;
    }
    #endregion

    #region WALL CLING METHOD
    private void TryEnterWallCling(float desiredMoveX, bool canMoveXAsDesired)
    {
        if (IsGrounded) return;
        if (canMoveXAsDesired) return;
        if (desiredMoveX == 0f) return;

        LastMoveDirection = Mathf.Sign(desiredMoveX);

        if (CheckWallClingSurface() == false) return;

        MoveX = 0f;
        MoveY = 0f;

        jumpCount = 0;

        dashCooldownTime = dashCooldown;
        isDashOnCooldown = false;

        IsClingToWall = true;
        wallDetachInputTime = 0f;
    }

    private void ApplyWallCling()
    {
        if (CheckWallClingSurface() == false)
        {
            IsClingToWall = false;
            return;
        }

        MoveX = 0f;

        if (isJumpRequested)
        {
            ApplyWallJump();
            return;
        }

        Vector2 moveAxis = moveInput;

        bool shouldIgnoreGravity =
            moveAxis.y >= 0f &&
            moveAxis != Vector2.zero;

        if (shouldIgnoreGravity) MoveY = 0f;
        else ApplyGravity(wallClingGravityMult);
    }

    private bool CheckWallClingSurface()
    {
        Bounds box = GetCastBox();

        Vector2 origin = Vector2.zero;
        origin.x = LastMoveDirection > 0f ? box.max.x : box.min.x;
        origin.y = box.center.y;

        Vector2 dir = Vector2.right * LastMoveDirection;

        RaycastHit2D hit = Physics2D.Raycast(
            origin,
            dir,
            wallCheckDistance,
            wallCollisionLayer
        );

        return hit.collider != null;
    }
    #endregion

    #region DASH METHOD
    private void ReadyDash()
    {
        isDashing = true;
        isDashOnCooldown = true;

        if (IsGrounded) coyoteTime = float.PositiveInfinity;

        float dir = moveInput.x;
        LastMoveDirection = dir != 0f ? dir : LastMoveDirection;

        float distance = dashSpeed * dashDuration;
        float delta = distance * LastMoveDirection;

        dashStartX = body.position.x;
        dashEndX = dashStartX + delta;
        dashPreviousX = dashStartX;

        MoveY = 0f; //대시 중에는 y축 이동량 제거

        afterImage.Execute(distance);
    }

    private void ApplyDash()
    {
        dashTime += Time.fixedDeltaTime;

        float progress = Mathf.Clamp01(dashTime / dashDuration);

        float easedMove = EasingFunction.EaseInOutQuart(dashStartX, dashEndX, progress);
        float desiredMove = easedMove - dashPreviousX;
        float actualMove = ResolveAxisMovement(desiredMove, Vector2.right, GetCastBox(), out bool canMoveAsDesired);

        body.MovePosition(body.position + new Vector2(actualMove, 0f));

        dashPreviousX += desiredMove;

        if (progress >= 1f || canMoveAsDesired == false)
        {
            isDashing = false;
            dashTime = 0f;

            //대쉬한 방향으로 속도값 조정
            MoveX = Mathf.Abs(MoveX) *
                dashEndSpeedMult *
                LastMoveDirection;

            afterImage.Stop();
        }
    }
    #endregion

    #region COLLISION METHOD
    private bool CheckGrounded()
    {
        Bounds box = GetCastBox();

        RaycastHit2D hit = Physics2D.BoxCast(box.center, box.size, 0f,
            Vector2.down, groundCheckDistance,
            collisionLayer);

        return hit.collider != null;
    }

    private Bounds GetCastBox()
    {
        Bounds box = collider.bounds;

        Vector3 size = box.size;
        size.x -= skinWidth * 2;
        size.y -= skinWidth * 2;
        box.size = size;

        return box;
    }
    #endregion

    #region PUBLIC METHOD
    public void SetMoveInput(Vector2 input)
    {
        float x = Mathf.Clamp(input.x, -1f, 1f);
        float y = Mathf.Clamp(input.y, -1f, 1f);
        moveInput = new Vector2(x, y);
    }

    public void RequestJump()
    {
        if (isDashing) return;

        isJumpRequested = true;
        jumpBufferTime = 0f;
    }

    public void ReleaseJump()
    {
        if (isJumpHold)
            isJumpHold = false;
    }

    public bool RequestDash()
    {
        if (isDashing) return false;
        if (isDashOnCooldown) return false;

        isDashRequested = true;
        return true;
    }

    public void SwitchMoveDirection() 
        => LastMoveDirection *= -1f;
    #endregion
}
