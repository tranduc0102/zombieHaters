public interface IDataInitializer<T, U>
{
	T Initialize(U type);

	U GetInitializedType();

	bool HasType(U type);
}
