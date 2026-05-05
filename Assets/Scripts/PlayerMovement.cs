using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerMovement : MonoBehaviour {
    [Header("Object refrences")]
    //A refrence to the player's rigidbody
    private Rigidbody rb;
    //A refrence to the player input component
    [SerializeField]
    private PlayerInput playerInput;
    //A refrence to the player's model, used to show rotation
    [SerializeField]
    private GameObject model;
    //A refrence to the camera's transform, used for camera-relative movement
    [SerializeField]
    private Transform cameraTransform;

    [Header("Movement")] 
    //The speed at which the player travels at
    [SerializeField] 
    private float moveSpeed = 2f; 
    //The rotation speed of the model
    [SerializeField] 
    private float rotationSpeed = 5f;

    [Header("Jumping")] 
    //The force at which the player jumps at
    [SerializeField] 
    private float jumpForce = 1f; 
    //The cooldown of the player
    [SerializeField] 
    private float jumpCooldown = 0.6f; 
    //A multiplier that reduces air floatynes
    [SerializeField] 
    private float airMultiplier = 0.4f; 
    //The coyote time, used to give a splitsecond window to jump once leaving ground
    [SerializeField] 
    private float coyoteTime = 0.2f; 
    //The counter used to tell how much time has passed for the coyote counter
    private float coyoteTimeCounter; 
    //A bool to check if the player can jump
    private bool canJump = true; 
    
    //A refrence to the ground, used to determine if the player is standing on ground or not
    public LayerMask ground; 
    //A bool to check if the player is grounded
    private bool isGrounded = false; 
    //A value used to create slight drag when the player is on the ground
    [SerializeField] 
    private float groundDrag = 0; 
    //The height of the player, used to calculate the whether the player is touching the ground
    [SerializeField] 
    private float playerHeight = 2f; 
    //A refrence to a vector used to know which keys the player is pressing
    private Vector2 input; 
    public bool hasKey = false;

    private void Awake() 
    { 
        //If the rigidbody isn't assigned, assign it
        if (this.rb == null) 
        { 
            rb = GetComponent<Rigidbody>(); 
        } 
        //If the player input isn't assigned, assign it
        if (this.playerInput == null) 
        { 
            this.playerInput = GetComponent<PlayerInput>(); 
        } 
        //If the camera isn't assigned, get the AR camera and assign it
        if(this.cameraTransform == null) 
        { 
            cameraTransform = GameObject.Find("ARThing").GetComponent<XROrigin>().Camera.gameObject.transform; 
        } 
    } 

    //Subscribe to all the input actions
    private void OnEnable() 
    { 
        playerInput.actions["Move"].performed += OnMoveAction; 
        playerInput.actions["Move"].canceled += OnMoveAction; 
        playerInput.actions["Jump"].performed += OnJumpAction; 
    } 
    //Unsubscribe from all the input actions
    private void OnDisable() 
    { 
        playerInput.actions["Move"].performed -= OnMoveAction; 
        playerInput.actions["Move"].canceled -= OnMoveAction; 
        playerInput.actions["Jump"].performed -= OnJumpAction; 
    } 
    private void Update() 
    { 
        //Use a raycast to determine if the player is grounded
        isGrounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, ground); 
        //Add coyote time
        if (isGrounded) 
        { 
            coyoteTimeCounter = coyoteTime; 
        } 
        else 
        { 
            coyoteTimeCounter -= Time.deltaTime; 
        } 
    } 
    private void FixedUpdate() 
    { 
        //Apply ground drag if the player is grounded
        rb.linearDamping = isGrounded ? groundDrag : 0f; 

        //If the player isn't performing a movement action, the movement code will not be executed
        if (input == Vector2.zero) return; 
        //Calculates the movement direction based on the camera's relative forward
        Vector3 moveDirection = GetCameraRelativeDirection(); 
        //Method for moving the player
        MovePlayer(moveDirection); 
        //Method for rotating the player model
        UpdateRotation(moveDirection); 
        //The method used to make sure the player doesn't go too fast
        SpeedControl(); 
    } 
    //Reading the input value of the movement
    private void OnMoveAction(InputAction.CallbackContext context) 
    { 
        this.input = context.ReadValue<Vector2>(); 
    } 
    //Method for moving the player
    private void MovePlayer(Vector3 moveDirection) 
    {
        if (!isGrounded) return;
        //Calculates a multiplier that is used to slow down the player while in the air
        float multiplier = isGrounded ? 1f : airMultiplier; 
        //Calculates a vector which the player will move in at a certain speed
        Vector3 force = moveDirection * moveSpeed * multiplier; 
        //Moves the player
        rb.AddForce(force, ForceMode.Force); 
    } 
         
    //Rotating the player
    private void UpdateRotation(Vector3 moveDirection) 
    { 
        //If the player is not performing a movement action, they will not rotate. This is to prevent the player's rotation from being locked to four axises
        if (moveDirection == Vector3.zero) return; 
        //Calculates a rotation that the model is supposed to be rotated towards
        Quaternion targetRotation = Quaternion.LookRotation(moveDirection); 
        //Rotates the model into the right direction
        model.transform.rotation = Quaternion.Slerp(model.transform.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime); 
    } 
    //A function that prvents the player from going too fast
    private void SpeedControl() 
    { 
        //Calculates the X and Z velocity that the player is moving in
        Vector3 flatVel = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z); 
        //Limit the velocity if needed
        if (flatVel.magnitude > moveSpeed) 
        { 
            Vector3 limitedVelocity = flatVel.normalized * moveSpeed; 
            rb.linearVelocity = new Vector3(limitedVelocity.x, rb.linearVelocity.y, limitedVelocity.z); 
        } 
    } 
    //The jumping input action
    private void OnJumpAction(InputAction.CallbackContext context) 
    { 
        //If the player still can jump, perform jump
        if(coyoteTimeCounter > 0f && canJump) 
        { 
            canJump = false; 
            Jump(); 
            //Stop the player from double jumping
            coyoteTimeCounter = 0f; 
            //Start the cooldown of the jump
            Invoke(nameof(ResetJump), jumpCooldown); 
        } 
    } 
    //Method for jumping
    private void Jump() 
    { 
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z); 
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse); 
    } 
    //Method for reseting the jump
    private void ResetJump() 
    { 
        canJump = true; 
    } 
    private Vector3 GetCameraRelativeDirection() 
    { 
        //Get the camera's orientations
        Vector3 camForward = cameraTransform.forward; 
        Vector3 camRight = cameraTransform.right;

        //Make sure the player doesn't follow the camera's y movement
        camForward.y = 0f; 
        camRight.y = 0f; 
        
        //Normalize the values
        camForward.Normalize(); 
        camRight.Normalize(); 

        //Return the camera's relative forward direction
        return camForward * input.y + camRight * input.x; 
    } 
    private void OnTriggerEnter(Collider other) 
    { 
        if(other.CompareTag("Key")) 
        { 
            hasKey = true; 
            Destroy(other.gameObject); 
        } 
    } 
}