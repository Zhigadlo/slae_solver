namespace Domain
{
    public class SlaeData
    {
        public SlaeData(List<float[]> matrix, float[] vector)
        {
            Matrix = matrix;
            Vector = vector;
        }
        public float[] Vector { get; set; }
        public List<float[]> Matrix { get; set; }
    }
}
