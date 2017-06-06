namespace Brainf_ck_sharp.Helpers
{
    internal struct Brainf_ckBinaryItem
    {
        public uint Offset { get; }

        public char Operator { get; }

        public Brainf_ckBinaryItem(uint offset, char @operator)
        {
            Offset = offset;
            Operator = @operator;
        }
    }
}