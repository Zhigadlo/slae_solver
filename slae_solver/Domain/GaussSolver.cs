namespace Domain
{
    public static class GaussSolver
    {
        public static float Epsilon = 0.0001f;
        //public static float[] Solve(List<float[]> matrix, float[] vector)
        //{
        //    int size = vector.Length;
        //    float[] previous = new float[size];
        //    float[] current = new float[size];

        //    for (int i = 0; i < size; i++)
        //    {
        //        current[i] = vector[i] / matrix[i][i];
        //    }

        //    //float maxDifference;
        //    float loss;
        //    do
        //    {
        //        loss = 0f;
        //        Array.Copy(current, previous, size);

        //        Parallel.For(0, size, i =>
        //        {
        //            float sum = 0f;

        //            for (int j = 0; j < size; j++)
        //            {
        //                if (i != j)
        //                {
        //                    sum += matrix[i][j] * previous[j];
        //                }
        //            }

        //            current[i] = (vector[i] - sum) / matrix[i][i];
        //        });

        //        //maxDifference = 0f;
        //        for (int i = 0; i < size; i++)
        //        {

        //            loss += (float)Math.Pow(current[i] - previous[i], 2);
        //            //maxDifference = Math.Max(maxDifference, Math.Abs(current[i] - previous[i]));
        //        }
        //    }
        //    while (Math.Sqrt(loss) >= Epsilon);

        //    return current;
        //}
        public static float[] Solve(List<float[]> matrix, float[] b)
        {
            int size = b.Length;
            float[] prevX = new float[size];
            float[] currX = new float[size];
            do
            {
                for (int i = 0; i < size; i++)
                {
                    currX[i] = b[i];
                    for (int j = 0; j < size; j++)
                    {
                        if (i != j)
                        {
                            currX[i] -= matrix[i][j] * prevX[j];
                        }
                    }
                    currX[i] /= matrix[i][i];
                }
            }
            while (!Converged(prevX, currX));
            return currX;
        }

        static bool Converged(float[] prevX, float[] currX)
        {
            double norm = 0;
            for (int i = 0; i < prevX.Length; i++)
            {
                norm += (currX[i] - prevX[i]) * (currX[i] - prevX[i]);
            }
            return Math.Sqrt(norm) < Epsilon;
        }
    }
}
