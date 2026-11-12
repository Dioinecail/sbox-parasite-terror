using Sandbox;
using static Sandbox.Engine.BindCollection;

public sealed class TestComponent : Component
{
	[Property] public PlayerController Controller { get; set; }
	[Property] public SkinnedModelRenderer Model { get; set; }

	[Property] public float BackwardDotThreshold { get; set; } = 0.55f;

	[Property] public Vector3 CameraOffsetNormal { get; set; }
	[Property] public Vector3 CameraOffsetAiming { get; set; }

	[Property, InputAction] public string AimAction { get; set; }
	[Property] public float AimBlendSpeed = 5f;
	[Property] public GameObject LeftHandTransformTarget { get; set; }
	[Property] public GameObject LeftHandTransformTargetOffsetted { get; set; }


	protected override void OnUpdate()
	{
		var wishVel = Controller.WishVelocity.Normal;
		var eyesBackwards = -Controller.EyeTransform.Rotation.Forward.WithZ(0f).Normal;

		var dot = Vector3.Dot(wishVel, eyesBackwards);

		// Log.Info(dot);

		if (dot > BackwardDotThreshold)
		{
			var targetForward = -wishVel.WithZ(0f).Normal;
			var targetRotation = Rotation.LookAt(targetForward, Vector3.Up);

			// Model.WorldRotation = targetRotation;

			Model.WorldRotation = Rotation.Slerp( Model.WorldRotation, targetRotation, Time.Delta * 2.0f * Controller.RotationSpeed);
		}

		var targetCameraOffset = Input.Down(AimAction) 
			? CameraOffsetAiming
			: CameraOffsetNormal;

		Controller.CameraOffset = Vector3.Lerp(Controller.CameraOffset, targetCameraOffset, AimBlendSpeed * Time.Delta);

		var leftHandIkWeight = Input.Down(AimAction)
			? 1f
			: 0f;

		var eyesCenterPosition =  Controller.EyeTransform.Position + Controller.EyeTransform.Forward * 150f;
		var handPosition = LeftHandTransformTarget.WorldTransform.Position;

		var dirFromHandToEyePoint = (eyesCenterPosition - handPosition).Normal;

		LeftHandTransformTarget.WorldRotation = Rotation.LookAt(dirFromHandToEyePoint, Vector3.Up);

		Model.Animation.Parameters.TrySet("LeftHandTarget", LeftHandTransformTargetOffsetted.WorldTransform);
		Model.Animation.Parameters.TrySet("LeftHandIK_Weight", leftHandIkWeight);

		// var targetAngle = Rotation.FromYaw( eyeAngles.yaw );
		// var velocity = Controller.WishVelocity.WithZ( 0 );

		// var rotateDifference = Model.WorldRotation.Distance( targetAngle );
		// var oldRotation = Model.WorldRotation;

		// Model.WorldRotation = rot;
	}
}
