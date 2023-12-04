namespace Domain
{
    public class ClientData
    {
        public float[] MatrixRow { get; set; }
        public float[] Previous { get; set; }
        public int StartIter { get; set; }
        public int EndIter { get; set; }
        public int Iteration { get; set; }

        public bool IsSlaeSolved { get; set; } = false;
    }
}
