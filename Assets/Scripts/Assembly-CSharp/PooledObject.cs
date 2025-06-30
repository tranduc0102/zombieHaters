using System.Collections.Generic;

public class PooledObject<T>
{
	public T prefab;

	public List<T> list = new List<T>();
}
