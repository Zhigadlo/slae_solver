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
            var testSlaeData = GetTestSlaeData($"M{testNumber}.txt", $"V{testNumber}.txt");
            var expectedAnswer = ReadVector($"X{testNumber}.txt");
            var actualAnswer = GaussSolver.Solve(testSlaeData.Matrix, testSlaeData.Vector);

            for (int i = 0; i < actualAnswer.Length; i++)
            {
                Assert.Equal(expectedAnswer[i], actualAnswer[i]);
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
                    var elements = reader.ReadLine().Split(' ').Select(float.Parse).ToArray();
                    matrix.Add(elements);
                }

                return matrix;
            }
        }

        private float[] ReadVector(string filename)
        {
            string path = Directory.GetParent(Environment.CurrentDirectory)?.Parent?.Parent?.FullName;
            var vectorPath = Path.Combine(path, $"Files/{filename}");
            var elements = File.ReadAllLines(vectorPath);
            var vector = new float[elements.Length];

            for (int i = 0; i < elements.Length; i++)
            {
                vector[i] = Convert.ToSingle(elements[i]);
            }

            return vector;
        }
    }
}