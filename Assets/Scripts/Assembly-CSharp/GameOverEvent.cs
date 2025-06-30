using System;
using System.Collections.Generic;

public class GameOverEvent
{
	private readonly List<Action<int>> _callbacks = new List<Action<int>>();

	public void Subscribe(Action<int> callback)
	{
		_callbacks.Add(callback);
	}

	public void Publish(int value)
	{
		foreach (Action<int> callback in _callbacks)
		{
			callback(value);
		}
	}

	public void UnSubscribe(Action<int> callback)
	{
		_callbacks.Remove(callback);
	}
}
