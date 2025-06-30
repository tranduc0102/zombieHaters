using UnityEngine;

namespace Spine.Unity.Examples
{
	[RequireComponent(typeof(CharacterController))]
	public class BasicPlatformerController : MonoBehaviour
	{
		[Header("Controls")]
		public string XAxis = "Horizontal";

		public string YAxis = "Vertical";

		public string JumpButton = "Jump";

		[Header("Moving")]
		public float walkSpeed = 1.5f;

		public float runSpeed = 7f;

		public float gravityScale = 6.6f;

		[Header("Jumping")]
		public float jumpSpeed = 25f;

		public float minimumJumpDuration = 0.5f;

		public float jumpInterruptFactor = 0.5f;

		public float forceCrouchVelocity = 25f;

		public float forceCrouchDuration = 0.5f;

		[Header("Visuals")]
		public SkeletonAnimation skeletonAnimation;

		[Header("Animation")]
		public TransitionDictionaryExample transitions;

		public AnimationReferenceAsset walk;

		public AnimationReferenceAsset run;

		public AnimationReferenceAsset idle;

		public AnimationReferenceAsset jump;

		public AnimationReferenceAsset fall;

		public AnimationReferenceAsset crouch;

		public AnimationReferenceAsset runFromFall;

		[Header("Effects")]
		public AudioSource jumpAudioSource;

		public AudioSource hardfallAudioSource;

		public ParticleSystem landParticles;

		public HandleEventWithAudioExample footstepHandler;

		private CharacterController controller;

		private Vector2 input = default(Vector2);

		private Vector3 velocity = default(Vector3);

		private float minimumJumpEndTime;

		private float forceCrouchEndTime;

		private bool wasGrounded;

		private AnimationReferenceAsset targetAnimation;

		private AnimationReferenceAsset previousTargetAnimation;

		private void Awake()
		{
			controller = GetComponent<CharacterController>();
		}

		private void Update()
		{
			float deltaTime = Time.deltaTime;
			bool isGrounded = controller.isGrounded;
			bool flag = !wasGrounded && isGrounded;
			input.x = Input.GetAxis(XAxis);
			input.y = Input.GetAxis(YAxis);
			bool buttonUp = Input.GetButtonUp(JumpButton);
			bool buttonDown = Input.GetButtonDown(JumpButton);
			bool flag2 = (isGrounded && input.y < -0.5f) || forceCrouchEndTime > Time.time;
			bool flag3 = false;
			bool flag4 = false;
			bool flag5 = false;
			if (flag && 0f - velocity.y > forceCrouchVelocity)
			{
				flag5 = true;
				flag2 = true;
				forceCrouchEndTime = Time.time + forceCrouchDuration;
			}
			if (!flag2)
			{
				if (isGrounded)
				{
					if (buttonDown)
					{
						flag4 = true;
					}
				}
				else
				{
					flag3 = buttonUp && Time.time < minimumJumpEndTime;
				}
			}
			Vector3 vector = Physics.gravity * gravityScale * deltaTime;
			if (flag4)
			{
				velocity.y = jumpSpeed;
				minimumJumpEndTime = Time.time + minimumJumpDuration;
			}
			else if (flag3 && velocity.y > 0f)
			{
				velocity.y *= jumpInterruptFactor;
			}
			velocity.x = 0f;
			if (!flag2 && input.x != 0f)
			{
				velocity.x = ((!(Mathf.Abs(input.x) > 0.6f)) ? walkSpeed : runSpeed);
				velocity.x *= Mathf.Sign(input.x);
			}
			if (!isGrounded)
			{
				if (wasGrounded)
				{
					if (velocity.y < 0f)
					{
						velocity.y = 0f;
					}
				}
				else
				{
					velocity += vector;
				}
			}
			controller.Move(velocity * deltaTime);
			if (isGrounded)
			{
				if (flag2)
				{
					targetAnimation = crouch;
				}
				else if (input.x == 0f)
				{
					targetAnimation = idle;
				}
				else
				{
					targetAnimation = ((!(Mathf.Abs(input.x) > 0.6f)) ? walk : run);
				}
			}
			else
			{
				targetAnimation = ((!(velocity.y > 0f)) ? fall : jump);
			}
			if (previousTargetAnimation != targetAnimation)
			{
				Animation animation = null;
				if (transitions != null && previousTargetAnimation != null)
				{
					animation = transitions.GetTransition(previousTargetAnimation, targetAnimation);
				}
				if (animation != null)
				{
					skeletonAnimation.AnimationState.SetAnimation(0, animation, false).MixDuration = 0.05f;
					skeletonAnimation.AnimationState.AddAnimation(0, targetAnimation, true, 0f);
				}
				else
				{
					skeletonAnimation.AnimationState.SetAnimation(0, targetAnimation, true);
				}
			}
			previousTargetAnimation = targetAnimation;
			if (input.x != 0f)
			{
				skeletonAnimation.Skeleton.FlipX = input.x < 0f;
			}
			if (flag4)
			{
				jumpAudioSource.Stop();
				jumpAudioSource.Play();
			}
			if (flag)
			{
				if (flag5)
				{
					hardfallAudioSource.Play();
				}
				else
				{
					footstepHandler.Play();
				}
				landParticles.Emit((int)(velocity.y / -9f) + 2);
			}
			wasGrounded = isGrounded;
		}
	}
}
