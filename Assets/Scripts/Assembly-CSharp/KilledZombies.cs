public class KilledZombies
{
	public int id;

	public int count;

	public float power;

	public float CalculateTotalDamage()
	{
		return power * StaticConstants.ZombiePowerConst * (float)count;
	}
}
