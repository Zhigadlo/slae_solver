namespace Server
{
    public static class GaussSolver
    {
        public static float[] Solve(IReadOnlyList<float[]> matrix, float[] b)
        {
            int rowCount = matrix.Count;
            float[] x = new float[rowCount];

            for (int i = 0; i < rowCount; i++)
            {
                float maxElement = Math.Abs(matrix[i][i]);
                int maxRow = i;
                for (int k = i + 1; k < rowCount; k++)
                {
                    if (Math.Abs(matrix[k][i]) > maxElement)
                    {
                        maxElement = Math.Abs(matrix[k][i]);
                        maxRow = k;
                    }
                }

                for (int k = i; k < rowCount; k++)
                {
                    float tmp = matrix[maxRow][k];
                    matrix[maxRow][k] = matrix[i][k];
                    matrix[i][k] = tmp;
                }

                float tmp2 = b[maxRow];
                b[maxRow] = b[i];
                b[i] = tmp2;

                for (int k = i + 1; k < rowCount; k++)
                {
                    float c = -matrix[k][i] / matrix[i][i];
                    for (int j = i; j < rowCount; j++)
                    {
                        if (i == j)
                        {
                            matrix[k][j] = 0;
                        }
                        else
                        {
                            matrix[k][j] += c * matrix[i][j];
                        }
                    }
                    b[k] += c * b[i];
                }
            }

            for (int i = rowCount - 1; i >= 0; i--)
            {
                x[i] = b[i] / matrix[i][i];
                for (int k = i - 1; k >= 0; k--)
                {
                    b[k] -= matrix[k][i] * x[i];
                }
            }

            return x;
        }
    }
}
