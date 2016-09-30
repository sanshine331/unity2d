﻿using System;

namespace Game.Level
{
	public interface IBuildable
	{
		void Build();

		Action<Type> Built { get; set; }
	}
}