namespace Domain
{
    public class SlaeData
    {
        public SlaeData(IReadOnlyList<float[]> matrix, float[] vector)
        {
            Matrix = matrix;
            Vector = vector;
        }
        public float[] Vector { get; set; }
        public IReadOnlyList<float[]> Matrix { get; set; }
    }
}
