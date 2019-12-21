namespace Brainf_ck_sharp_UWP.DataModels.Misc
{
    /// <summary>
    /// A struct that indicates the position of a given object or item in a 2D spacee
    /// </summary>
    public struct Coordinate
    {
        /// <summary>
        /// Gets the horizontal coordinate
        /// </summary>
        public int X { get; }

        /// <summary>
        /// Gets the vertical coordinate
        /// </summary>
        public int Y { get; }

        /// <summary>
        /// Creates a new coordinate with the given values
        /// </summary>
        /// <param name="x">The horizontal offset</param>
        /// <param name="y">The vertical offset</param>
        public Coordinate(int x, int y)
        {
            X = x;
            Y = y;
        }
    }
}