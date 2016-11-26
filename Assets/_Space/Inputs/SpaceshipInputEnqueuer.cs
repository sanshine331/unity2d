﻿using System.Collections.Generic;
using UnityEngine;

public enum SpaceshipAIState
{
	Idle,
	Moving,
}

[RequireComponent(
	typeof(Spaceship)
)]
public class SpaceshipInputEnqueuer : AInputEnqueuer
{
	[SerializeField]
	private SpaceshipAIState state;

	private List<KeyCode> lastInputsReceived = new List<KeyCode>();

	private HashSet<Collider2D> otherColliders = new HashSet<Collider2D>();

	[SerializeField]
	private Spaceship character;

	private SpaceshipMovement movement;

	private CircleCollider2D collider;

	private int escapeCounter = 0;

	private Vector2 inputDirection = Vector2.zero;

	private Vector2 EscapeDirection
	{
		get
		{
			var newDirection = Vector2.zero;
			foreach (var otherCollider in otherColliders)
			{
				newDirection += movement.Position - GetColliderPosition(otherCollider);
			}

			return newDirection.normalized;
		}
	}

	public bool CanEscape { get { return EscapeDirection == Vector2.zero && escapeCounter < 2; } }

	protected override void Awake()
	{
		base.Awake();

		character = GetComponent<Spaceship>();
		movement = character.GetComponent<SpaceshipMovement>();
		collider = character.GetComponent<CircleCollider2D>();

		var dequeuerInstance = character.GetComponent<AInputDequeuer>();
		Add(ref dequeuerInstance);
		(dequeuerInstance as SpaceshipInputDequeuer).InputsDequeued += OnInputsDequeued;
	}

	protected override void EnqueueInputs()
	{
		if (state == SpaceshipAIState.Idle)
		{
			lastInputsReceived.Clear();
			var inputsGenerated = Random.Range(0, maximumInputsPerUpdate);
			if (inputsGenerated > 0)
			{
				for (int i = 0; i < inputsGenerated; i++)
				{
					var generatedInput = Random.Range(0, 4);
					var input = KeyCode.None;
					if (generatedInput == 0)
					{
						input = KeyCode.UpArrow;
					}

					if (generatedInput == 1)
					{
						input = KeyCode.DownArrow;
					}

					if (generatedInput == 2)
					{
						input = KeyCode.LeftArrow;
					}

					if (generatedInput == 3)
					{
						input = KeyCode.RightArrow;
					}
					lastInputsReceived.Add(input);
					Enqueue(input);
				}
				return;
			}
			Enqueue(KeyCode.None);
			return;
		}

		if (state == SpaceshipAIState.Moving)
		{
			foreach (var input in lastInputsReceived)
			{
				Enqueue(input);
			}
			if (lastInputsReceived.Count == 0)
			{
				Enqueue(KeyCode.None);
			}
			return;
		}
	}

	protected void OnInputsDequeued(Vector2 direction)
	{
		if (!enabled)
		{
			return;
		}

		if (state == SpaceshipAIState.Idle)
		{
			if (direction != Vector2.zero)
			{
				inputDirection = direction;
				state = SpaceshipAIState.Moving;
				return;
			}
		}

		if (state == SpaceshipAIState.Moving)
		{
			if (inputDirection == Vector2.zero)
			{
				state = SpaceshipAIState.Idle;
				return;
			}
		}

		movement.Move(inputDirection);
		DrawDebugRays(direction, ref inputDirection);
		return;
	}

	private void DrawDebugRays(Vector2 direction, ref Vector2 inputDirection)
	{
		if (inputDirection != direction)
		{
			Debug.DrawRay(movement.Position, inputDirection.normalized * collider.radius, Color.cyan);
			return;
		}
		Debug.DrawRay(movement.Position, direction.normalized * collider.radius, Color.magenta);
		return;
	}

	private void OnCollisionEnter2D(Collision2D other)
	{
		lastInputsReceived.Clear();
		otherColliders.Add(other.collider);
		inputDirection = EscapeDirection;
	}

	private void OnCollisionStay2D()
	{
		inputDirection = EscapeDirection;
	}

	private void OnCollisionExit2D(Collision2D other)
	{
		otherColliders.Remove(other.collider);

		if (CanEscape && TryToEscape(other, ref inputDirection, ref escapeCounter))
		{
			return;
		}

		StopEscaping(out inputDirection, out escapeCounter);
		return;
	}

	private bool TryToEscape(Collision2D other, ref Vector2 inputDirection, ref int escapeCounter)
	{
		var direction = (movement.Position - GetColliderPosition(other.collider)).normalized * collider.radius;
		var something = Physics2D.Raycast(movement.Position, direction, collider.radius * 2f);
		if (!something.collider)
		{
			++escapeCounter;
			inputDirection = direction.normalized;

			Debug.DrawRay(movement.Position, direction, Color.white, 1f);
			return true;
		}

		Debug.DrawRay(GetColliderPosition(other.collider), direction * Vector2.Distance(movement.Position, GetColliderPosition(other.collider)), Color.blue, 1f);
		return false;
	}

	private static void StopEscaping(out Vector2 inputDirection, out int escapeCounter)
	{
		escapeCounter = 0;
		inputDirection = Vector2.zero;
	}

	private static Vector2 GetColliderPosition(Collider2D collider)
	{
		return (Vector2)collider.transform.position + collider.offset;
	}

	protected override void OnDequeuerDestroyed(IDestroyable destroyedComponent)
	{
		base.OnDequeuerDestroyed(destroyedComponent);
		(destroyedComponent as SpaceshipInputDequeuer).InputsDequeued -= OnInputsDequeued;
	}
}