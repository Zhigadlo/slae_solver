using Domain;

namespace Client
{
    public class SlaeDataReader
    {
        public SlaeData ReadSlaeData()
        {
            var matrixPath = "C:\\Users\\vladi\\OneDrive\\Рабочий стол\\matrix.txt";
            var matrix = ReadMatrix(matrixPath);
            var vectorPath = "C:\\Users\\vladi\\OneDrive\\Рабочий стол\\vector.txt";
            Console.ResetColor();
            var vector = ReadVector(vectorPath);
            var slaeData = new SlaeData(matrix, vector);
            Console.WriteLine("Slae data:");
            SlaeDataOutput(slaeData);
            return slaeData;
        }

        private List<float[]> ReadMatrix(string filename)
        {
            using (var reader = new StreamReader(filename))
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
            var elements = File.ReadAllLines(filename);
            var vector = new float[elements.Length];

            for (int i = 0; i < elements.Length; i++)
            {
                vector[i] = Convert.ToSingle(elements[i]);
            }

            return vector;
        }

        public void MatrixTest()
        {
            var matrixPath = "C:\\Users\\vladi\\OneDrive\\Рабочий стол\\matrix.txt";
            var matrix = ReadMatrix(matrixPath);
            Console.WriteLine("Matrix:");
            MatrixOutput(matrix);
            var vectorPath = "C:\\Users\\vladi\\OneDrive\\Рабочий стол\\vector.txt";
            Console.WriteLine("Vector:");
            var vector = ReadVector(vectorPath);
            VectorOutput(vector);

        }

        private void MatrixOutput(List<float[]> matrix)
        {
            foreach (var element in matrix)
            {
                for (int i = 0; i < element.Length; i++)
                {
                    Console.Write(element[i] + " ");
                }
                Console.WriteLine();
            }
        }
        private void VectorOutput(float[] vector)
        {
            for(int i = 0; i < vector.Length; i++)
            {
                Console.WriteLine(vector[i]);
            }
        }

        private void SlaeDataOutput(SlaeData data)
        {
            for(int i = 0; i < data.Vector.Length; i++)
            {
                Console.WriteLine(string.Join(" ", data.Matrix[i].Select(p => p.ToString()).ToArray()) + " " + data.Vector[i]);
            }
        }
    }
}
