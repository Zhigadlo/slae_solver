using Domain;

namespace SlaeSolverTests
{
    public class GaussSolverTests
    {
        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        public void GaussTests(int testNumber)
        {
            var testSlaeData = GetTestSlaeData($"A{testNumber}.txt", $"B{testNumber}.txt");
            var expectedAnswer = ReadVector($"X{testNumber}.txt");
            var actualAnswer = GaussSolver.Solve(testSlaeData.Matrix, testSlaeData.Vector);

            for (int i = 0; i < actualAnswer.Length; i++)
            {
                Assert.Equal(expectedAnswer[i], actualAnswer[i], 0.01f);
            }
        }

        private SlaeData GetTestSlaeData(string matrixFilename, string vectorFilename)
        {
            var matrix = ReadMatrix(matrixFilename);
            var vector = ReadVector(vectorFilename);

            return new SlaeData(matrix, vector);
        }

        private List<float[]> ReadMatrix(string filename)
        {
            string path = Directory.GetParent(Environment.CurrentDirectory)?.Parent?.Parent?.FullName;
            var matrixPath = Path.Combine(path, $"Files/{filename}");
            using (var reader = new StreamReader(matrixPath))
            {
                var matrix = new List<float[]>();

                while (!reader.EndOfStream)
                {
                    var elements = reader.ReadLine().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    matrix.Add(new float[elements.Length]);

                    for (int i = 0; i < elements.Length; i++)
                    {
                        matrix[matrix.Count - 1][i] = Convert.ToSingle(elements[i]);
                    }
                }

                return matrix;
            }
        }
        private float[] ReadVector(string filename)
        {
            string path = Directory.GetParent(Environment.CurrentDirectory)?.Parent?.Parent?.FullName;
            var vectorPath = Path.Combine(path, $"Files/{filename}");
            using (var reader = new StreamReader(vectorPath))
            {
                var elements = reader.ReadToEnd().Split(new char[] { '\n', '\r', ' ' }, StringSplitOptions.RemoveEmptyEntries);
                var vector = new float[elements.Length];

                for (int i = 0; i < elements.Length; i++)
                    vector[i] = Convert.ToSingle(elements[i]);

                return vector;
            }
        }
    }
}