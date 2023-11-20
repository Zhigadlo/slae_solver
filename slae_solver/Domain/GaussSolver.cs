namespace Domain
{
    public static class GaussSolver
    {
        public static float[] Solve(List<float[]> matrix, float[] vector, int iterations = 10000)
        {
            int size = vector.Length;
            float[] previous = new float[size];
            float[] current = new float[size];

            for (int i = 0; i < size; i++)
            {
                current[i] = vector[i] / matrix[i][i];
            }

            for (int iteration = 0; iteration < iterations; iteration++)
            {
                Array.Copy(current, previous, size);

                for (int i = 0; i < size; i++)
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
                }
            }

            return current;
        }
    }
}
