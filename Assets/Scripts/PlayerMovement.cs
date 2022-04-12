using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    const string FLIP_ANIMATION = "flipAnimation";
    const string JUMP_ANIMATION_WHITE = "JumpAnimationWhite";
    const string JUMP_ANIMATION_BLACK = "JumpAnimationBlack";
    const string DEATH_ANIMATION_WHITE = "DeathAnimationWhite";
    const string DEATH_ANIMATION_BLACK = "DeathAnimationBlack";
    const string TURN_ON_COLLIDER = "turnOnCollider";
    const string FLIP_PLAYER = "FlipPlayer";
    const string FLIP_PARAMETER = "changeDirection";
    const float SWITCH_TIME = 1.167f / 2;
    const float GRAVITY = 30f;
    const float SPEED = .14f;
    const float JUMP_FORCE = 11f;

    float SWITCH_COOLDOWN = 1.5f;
    float DEATH_COOLDOWN = 0.1f;
    float WALK_COOLDOWN = 0.3f;

    float switchCurrentCoolDown = 0f;
    float deathCurrentCheckCoolDown = 0f;
    float walkCurrentCoolDown = 1f;

    bool isRightFootstep;

    public AudioClip flipAudio;
    public AudioClip[] walkAudios;
    public AudioClip[] jumpAudios;
    public AudioClip dieAudio;
    public AudioClip usePortal;

    Vector2 lastPosition;

    bool isDownwards = false;
    public bool isFliping = false;
    bool isPlayerDeath;

    public enum JumpingStates
    {
        GROUNDED,
        ONE_JUMP,
        DOUBLE_JUMP
    }
    public JumpingStates jumpState = JumpingStates.ONE_JUMP;

    public SpriteRenderer playerSpriteRenderer;
    public BoxCollider2D playerCollider;
    public Rigidbody2D playerRigidbody;
    public Animator playerAnimator;
    public AnimationClip switchClip;
    public AudioSource playerAudioSource;
    
    private Gear currentGear;
    public LevelGenerator levelGenerator;

    void Start()
    {
        lastPosition = transform.position;
    }

    void Update()
    {
        if (!isPlayerDeath)
        {
            UpdateCooldowns();
            HandleInput();
            StartCoroutine(CheckDeath());
        }
    }

    void FixedUpdate()
    {
        if (!isPlayerDeath)
        {
            HandleRotation();
            HandleMovement();
            HandleGravity();
        }
    }

    public void ResetPlayer()
    {
        playerAnimator.SetBool(FLIP_PARAMETER, false);
        isDownwards = false;
        isFliping = false;
        jumpState = JumpingStates.ONE_JUMP;
        isPlayerDeath = false;
        playerSpriteRenderer.flipY = false;
        transform.position = new Vector3(0, 50, 0); 
        playerAnimator.Play("Walk");
    }
    
    void UpdateCooldowns()
    {
        switchCurrentCoolDown -= Time.deltaTime;
        deathCurrentCheckCoolDown -= Time.deltaTime;
        walkCurrentCoolDown -= Time.deltaTime;
    }

    public void AssignGear(Gear newGear)
    {
        currentGear = newGear;
    }

    void HandleMovement()
    {
        transform.position += transform.right * SPEED;

        if (walkCurrentCoolDown <= 0 && jumpState == JumpingStates.GROUNDED && !isFliping)
        {
            if (isRightFootstep)
            {
                PlayAudio(walkAudios[Random.Range(0,2)]);
            }
            else
            {
                PlayAudio(walkAudios[Random.Range(2,4)]);           
            }
            walkCurrentCoolDown = WALK_COOLDOWN;
            isRightFootstep = !isRightFootstep;
        }
    }

    void HandleGravity()
    {
        if (isDownwards)
        {
            playerRigidbody.AddForce(transform.up * GRAVITY);
        }
        else
        {
            playerRigidbody.AddForce(-transform.up * GRAVITY);
        }
    }

    void HandleRotation()
    {
        float xdif = -currentGear.transform.parent.position.x + transform.position.x;
        float ydif = -currentGear.transform.parent.position.y + transform.position.y;
        float angle = Mathf.Atan2(xdif, ydif) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, -angle);
    }

    void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            Vector3 playerVelocityV3 = new Vector3(playerRigidbody.velocity.x, playerRigidbody.velocity.y);

            if (jumpState != JumpingStates.DOUBLE_JUMP)
            {
                if (isDownwards)
                {
                    playerRigidbody.AddForce(-transform.up * JUMP_FORCE - playerVelocityV3, ForceMode2D.Impulse);
                    playerAnimator.Play(JUMP_ANIMATION_WHITE, 0, 0f);
                }
                else
                {
                    playerRigidbody.AddForce(transform.up * JUMP_FORCE - playerVelocityV3, ForceMode2D.Impulse);
                    playerAnimator.Play(JUMP_ANIMATION_BLACK, 0, 0f);
                }
                jumpState++;
                if (jumpState == JumpingStates.ONE_JUMP)
                {
                    PlayAudio(jumpAudios[Random.Range(0,2)]);
                }
                else
                {
                    PlayAudio(jumpAudios[Random.Range(2,4)]);
                }
            }
        }
        if (Input.GetKeyDown(KeyCode.DownArrow) && switchCurrentCoolDown <= 0 && jumpState == JumpingStates.GROUNDED)
        {
            StartCoroutine(FlipPlayer());
            switchCurrentCoolDown = SWITCH_COOLDOWN;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.gameObject == currentGear.gameObject)
        {
            jumpState = JumpingStates.GROUNDED;
        }
    }

    IEnumerator FlipPlayer()
    {
        isFliping = true;
        playerAnimator.SetBool(FLIP_PARAMETER, true); 
        PlayAudio(flipAudio);
        yield return new WaitForSeconds(SWITCH_TIME);
        if (!isPlayerDeath)
        {
            currentGear.GetComponent<EdgeCollider2D>().enabled = false;
            if (isDownwards)
            {
                transform.position = Vector3.MoveTowards(transform.position, (transform.position - currentGear.transform.parent.position) * 100f, 3f);
            }
            else
            {
                transform.position = Vector3.MoveTowards(transform.position, currentGear.transform.parent.position, 3f);
            }
            playerSpriteRenderer.flipY = !playerSpriteRenderer.flipY;   
            isDownwards = !isDownwards;
            currentGear.GetComponent<EdgeCollider2D>().enabled = true;
            playerAnimator.SetBool(FLIP_PARAMETER, false);
            isFliping = false;
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if(col.gameObject.name == "Portal")
        {
            AssignGear(levelGenerator.NextGear());
            col.gameObject.name = "UsedPortal";
            PlayAudio(usePortal);
        }
    }

    IEnumerator CheckDeath()
    {
        if (deathCurrentCheckCoolDown <= 0)
        {
            float distanceBetweenPositions = Vector2.Distance(transform.position, lastPosition);
            if (distanceBetweenPositions < 0.075f || distanceBetweenPositions > 15f)
            {
                if (isDownwards)
                {
                    playerAnimator.Play(DEATH_ANIMATION_BLACK);
                }
                else
                {
                    playerAnimator.Play(DEATH_ANIMATION_WHITE);
                }
                isPlayerDeath = true;
                PlayAudio(dieAudio);
                yield return new WaitForSeconds(3f);
                levelGenerator.Start();
            }
            lastPosition = transform.position;
            deathCurrentCheckCoolDown = DEATH_COOLDOWN;
        }
    }

    void PlayAudio (AudioClip audioClip)
    {
        playerAudioSource.clip = audioClip;
        playerAudioSource.Play();
    }
}