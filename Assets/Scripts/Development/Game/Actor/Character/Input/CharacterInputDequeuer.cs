﻿using Game.Input;
using UnityEngine;

namespace Game.Actor
{
	[RequireComponent(
	typeof(Character)
	)]
	public class CharacterInputDequeuer : AInputDequeuer
	{
		[SerializeField]
		private Character character;

		public Character Character { get { return character; } }

		private void Awake()
		{
			character = GetComponent<Character>();

			character.MovementHalted += OnCharacterMovementHalt;
		}

		private void OnCharacterMovementHalt()
		{
			foreach (var inputEnqueuer in inputEnqueuers)
			{
				inputEnqueuer.Inputs.Clear();
			}
		}

		public override void OnInputsEnqueued(AInputEnqueuer inputEnqueuer)
		{
			if (character.State == CharacterState.Idle)
			{
				if (inputEnqueuer.HasInputs)
				{
					Vector2 direction = Vector2.zero;
					KeyCode input = inputEnqueuer.Inputs.Dequeue();
					switch (input)
					{
						case KeyCode.UpArrow:
							direction = Vector2.up;
							break;

						case KeyCode.DownArrow:
							direction = Vector2.down;
							break;

						case KeyCode.LeftArrow:
							direction = Vector2.left;
							break;

						case KeyCode.RightArrow:
							direction = Vector2.right;
							break;
					}

					character.SetDestination(direction);
				}

				Debug.LogWarning(name + " InputDequeuer dequeueing from " + inputEnqueuer.name + " | Total InputEnqueuers: " + inputEnqueuers.Count);
			}
		}

		public override void Dispose()
		{
			character.MovementHalted -= OnCharacterMovementHalt;
		}
	}
}