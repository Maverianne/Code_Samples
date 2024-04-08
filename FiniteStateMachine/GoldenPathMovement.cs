using GameControl;
using Support;
using UnityEngine;

namespace Player.MovementStateMachine.States
{
    public class GoldenPathMovement : ReinaMovementState
    {
		private const string Emission = "_EmissionColor";

		private bool _reinaHasDisappeared;
		private bool _restorePhaseReached;

		private float _overlayStartTime;

		private float _respawnTimer;
		private static readonly int EmissionColor = Shader.PropertyToID(Emission);

		public GoldenPathMovement (ReinaDirector reina) {
			//When Reina is returning to the entrance of a room
			Reina = reina;
			CanReorient = true;
			CanTransitionToSelf = true;
		}

		public override void Enter() {
			//Which disappearance are we doing?  Default (Grab regret device), Discard keys (from golden path), or return to golden path (discard jacket), or instant?
			_reinaHasDisappeared = false;
			_restorePhaseReached = false;
			_respawnTimer = 0f;
			
			_overlayStartTime = Reina.ReinaSettings.RegretDeviceOverlayStartTime + Reina.ReinaSettings.RegretDeviceReinaDisappearTimeShort;

			TargetVelocity = Vector3.zero;
			Reina.Rigidbody.velocity = Vector3.zero;

			Reina.Status.Animator.SetBool(Constants.AnimationFields.Booleans.CustomAnimationActive, true);
			Reina.Status.Animator.SetInteger(Constants.AnimationFields.Integers.CustomAnimationID, Reina.CurrentWorldVariables.HasKeys ? Constants.Animation.Custom.Reina.DiscardCard : Constants.Animation.Custom.Reina.RegretDeviceLocalDepart);
			if (!Reina.Status.CurrentRoomEntranceState.KeysScrambled)
			{
				Reina.CurrentWorldVariables.GeneticKeysScrambled = Reina.Status.CurrentRoomEntranceState.KeysScrambled;
				Reina.CurrentWorldVariables.GoldenPath = Reina.Status.CurrentRoomEntranceState.GoldenPath;
			}
			Reina.Status.Animator.SetTrigger(Constants.AnimationFields.Triggers.CustomAnimation);
			
			Reina.Status.SetFacingDirection(Constants.Direction.Right);
			GameplayManager.Instance.Camera.UpdateFov(Reina.ReinaSettings.RegretDeviceCameraZoomInFieldOfViewFinal, Reina.ReinaSettings.RegretDeviceCameraZoomInDuration);

			GeneralManager.Instance.Audio.SoundEffects.RegretDeviceTravelBack.PlaySoundEffect();
		}

		public override void Exit() {
			Reina.Status.Animator.SetBool(Constants.AnimationFields.Booleans.CustomAnimationActive, false);
			
			Reina.Body.ReinaRenderer.materials[ReinaEffectDirector.MainMaterialIndex].SetColor(EmissionColor, Reina.Effects.BaseEmission);
			Reina.Body.ReinaRenderer.materials[ReinaEffectDirector.EyeMaterialIndex].SetColor(EmissionColor, Reina.EffectSettings.EyeDefaultEmission);
			Reina.Gear.RegretDevice.material.SetColor(EmissionColor, Reina.EffectSettings.RegretDeviceBaseEmission);
			EventManager.TriggerEvent(Constants.Events.WorldVariable.GoldenPathRestarted);
		}

		public override void ExecuteUpdate() {
			OrientReina();
			ProcessPhases();

			Reina.Status.Animator.SetFloat(Constants.AnimationFields.Floats.StandardSpeed, Reina.Rigidbody.velocity.x, 0.1f, Time.deltaTime);
			Reina.Status.Animator.SetFloat(Constants.AnimationFields.Floats.VelocityY, Reina.Rigidbody.velocity.y, 0.1f, Time.deltaTime);

			if (Reina.Movement.CurrentMovementState != this) return;
		}

		public override void ExecuteFixedUpdate()
		{
			//Status Check
			if (_reinaHasDisappeared || GeneralManager.Instance.Input.Gameplay.HoldingSpecial) return;
			GeneralManager.Instance.Audio.SoundEffects.RegretDeviceTravelBack.StopSoundEffect(0.5f);
			GeneralManager.Instance.Audio.SoundEffects.RegretDeviceTravelBackCancel.PlaySoundEffect();
			Reina.Status.Animator.SetBool(Constants.AnimationFields.Booleans.CustomAnimationActive, false);
		}

		private void ProcessPhases() {
			var eyeFlareProgress = Mathf.Clamp01((Timer - Reina.ReinaSettings.RegretDeviceEyeFlareStartTime) / Reina.ReinaSettings.RegretDeviceFlareStartTransitionDurations);
			var gemFlareProgress = Mathf.Clamp01((Timer - Reina.ReinaSettings.RegretDeviceGemFlareStartTime) / Reina.ReinaSettings.RegretDeviceFlareStartTransitionDurations);
			var bodyFlareProgress = Mathf.Clamp01((Timer - Reina.ReinaSettings.RegretDeviceBodyFlareStartTime) / Reina.ReinaSettings.RegretDeviceFlareStartTransitionDurations);
			
			Reina.Effects.EyeMaterial.SetColor(ReinaEffectDirector.EmissionColor, Color.Lerp(Reina.EffectSettings.EyeDefaultEmission, Reina.EffectSettings.EyeGlowEmission, eyeFlareProgress));
			Reina.Effects.RegretDeviceMainMaterial.SetColor(ReinaEffectDirector.EmissionColor, Color.Lerp(Reina.EffectSettings.RegretDeviceBaseEmission, Reina.EffectSettings.RegretDeviceFlareEmission, gemFlareProgress));
			Reina.Effects.ReinaMainMaterial.SetColor(ReinaEffectDirector.EmissionColor, Color.Lerp(Reina.Effects.BaseEmission, Reina.EffectSettings.RegretDeviceBodyFlareEmission, bodyFlareProgress));

			if (Timer > Reina.ReinaSettings.RegretDeviceReinaDisappearTimeShort && !_reinaHasDisappeared) {
				var remnantPosition = new Vector3(Reina.transform.position.x, Reina.transform.position.y, Reina.transform.position.z);
				_reinaHasDisappeared = true;
				GameplayManager.Instance.ObjectPool.SpawnFromPool(Constants.ObjectPool.Reina.RespawnRemnants, remnantPosition, Quaternion.identity, 1f);
				Reina.Body.ReinaMainMeshWrapper.SetActive(false);
				GeneralManager.Instance.Audio.SoundEffects.Impact.PlaySoundEffect();
			}

			if (Timer > _overlayStartTime && !_restorePhaseReached) {
				//START RESTORE PHASE
				GameplayManager.Instance.Camera.UpdateFov(Reina.Status.CurrentRoom.LocalCameraFieldOfView, Reina.ReinaSettings.RestoreDuration);
				
				Reina.transform.position = Reina.Status.GoldenPathReinaPosition;
				Reina.Status.GoldenPathRoom.gameObject.SetActive(true);
				GeneralManager.Instance.Data.ApplyWorldMemoryState(Reina.Status.GoldenPathWorldState);


				Vector3 position = Reina.transform.position + Reina.GeneralCollider.center;
				GameplayManager.Instance.ObjectPool.SpawnFromPool(Constants.ObjectPool.General.DistortionRipple, position, Quaternion.identity, 1f);
				Reina.Body.ReinaMainMeshWrapper.SetActive(true);
				_respawnTimer = 0f;
				_restorePhaseReached = true;

				Vector3 position1 = Reina.Status.RoomEntrancePosition + Reina.GeneralCollider.center;
				GameplayManager.Instance.ObjectPool.SpawnFromPool(Constants.ObjectPool.Reina.RespawnPortal, position1, Quaternion.identity, 1f);
				GeneralManager.Instance.Audio.SoundEffects.RegretDeviceArrive.PlaySoundEffect();
				
				GeneralManager.Instance.Audio.StopSoundEffect(GeneralManager.Instance.Audio.SoundEffects.RegretDeviceTravelBack.AudioClip, 0.05f);
				GeneralManager.Instance.Audio.SoundEffects.RegretDeviceTravelBackCancel.PlaySoundEffect();

				Reina.Status.Animator.SetBool(Constants.AnimationFields.Booleans.CustomAnimationActive, true);
				Reina.Status.Animator.SetInteger(Constants.AnimationFields.Integers.CustomAnimationID, Constants.Animation.Custom.Reina.RegretDeviceLocalRespawn);
				Reina.Status.Animator.SetTrigger(Constants.AnimationFields.Triggers.CustomAnimation);
				return;
			}

			if (_restorePhaseReached) {
				_respawnTimer += Time.deltaTime;
				PerformRestorePhase();
			}

			if (_respawnTimer > Reina.ReinaSettings.RestoreDuration) {
				EventManager.TriggerEvent(Constants.Events.Reina.RespawnFinished);
			}
		}
		
		private void PerformRestorePhase() {
			var percentComplete = Mathf.Clamp01(_respawnTimer / Reina.ReinaSettings.RestoreDuration);
			var quickPercentComplete = Mathf.Clamp01(percentComplete * 2);
		
			Reina.transform.position = Reina.Status.GoldenPathReinaPosition;
			
			Reina.Body.ReinaRenderer.materials[ReinaEffectDirector.MainMaterialIndex].SetColor(EmissionColor, Color.Lerp(Reina.EffectSettings.RegretDeviceBodyFlareEmission, Reina.Effects.BaseEmission, quickPercentComplete));
			Reina.Body.ReinaRenderer.materials[ReinaEffectDirector.EyeMaterialIndex].SetColor(EmissionColor, Color.Lerp(Reina.EffectSettings.EyeGlowEmission, Reina.EffectSettings.EyeDefaultEmission, percentComplete));
			Reina.Gear.RegretDevice.material.SetColor(EmissionColor, Color.Lerp(Reina.EffectSettings.RegretDeviceFlareEmission, Reina.EffectSettings.RegretDeviceBaseEmission, percentComplete));
		}

		public override void Spring(float upwardsVelocity, float minAscentTime = 0.5f) { }
    }
}