namespace Domain
{
    public static class GaussSolver
    {
        public static float Epsilon = 0.00001f;
        public static float[] Solve(List<float[]> matrix, float[] vector)
        {
            int size = vector.Length;
            float[] previous = new float[size];
            float[] current = new float[size];

            for (int i = 0; i < size; i++)
            {
                current[i] = vector[i] / matrix[i][i];
            }
            bool converge = false;
            do
            {
                float norm = 0f;
                Array.Copy(current, previous, size);

                Parallel.For(0, size, i =>
                {
                    float sum = 0f;

                    for (int j = 0; j < size; j++)
                    {
                        if (i != j)
                        {
                            sum += matrix[i][j] * previous[j];
                        }
                    }

                    current[i] = (vector[i] - sum) / matrix[i][i];
                    norm += (current[i] - previous[i]) * (current[i] - previous[i]);
                    
                });
                converge = Math.Sqrt(norm) < Epsilon;
            }
            while (!converge);

            return current;
        }
    }
}
