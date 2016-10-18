﻿using System;
using UnityEngine;

[RequireComponent(
	typeof(CharacterMovement),
	typeof(SpriteRenderer)
)]
public class Character : AActor
{
	[SerializeField]
	private int steps = 0;

	public int Steps { get { return steps; } }

	[SerializeField]
	private SpriteRenderer spriteRenderer;

	[SerializeField]
	private CharacterMovement characterMovement;

	public Action StepTaken = delegate { };

	private void Awake()
	{
		spriteRenderer = GetComponent<SpriteRenderer>();

		characterMovement = GetComponent<CharacterMovement>();
		characterMovement.DestinationSet += OnDestinationSet;
		characterMovement.DestinationReached += OnDestinationReached;
	}

	private void OnDestinationSet(Vector2 destination)
	{
		spriteRenderer.flipX = (destination - characterMovement.Position).x > 0f;
	}

	private void OnDestinationReached()
	{
		if (characterMovement.PreviousDestination != characterMovement.Position)
		{
			steps++;
			StepTaken();
		}
	}

	public override void Enable()
	{
		base.Enable();
		gameObject.SetActive(true);
	}

	public override void Disable()
	{
		base.Disable();
		gameObject.SetActive(false);
	}

	public override void Dispose()
	{
		characterMovement.DestinationSet -= OnDestinationSet;
		characterMovement.DestinationReached -= OnDestinationReached;
	}
}