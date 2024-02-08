[System.Serializable]
public class MinMax2D
{
	public float MinX;
	public float MinY;
	public float MaxX;
	public float MaxY;

    public MinMax2D(float minX, float minY, float maxX, float maxY)
    {
		MinX = minX;
		MinY = minY;
		MaxX = maxX;
		MaxY = maxY;
    }
}