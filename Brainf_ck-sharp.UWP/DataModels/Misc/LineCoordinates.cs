namespace Brainf_ck_sharp_UWP.DataModels.Misc
{
    /// <summary>
    /// A simple struct representing the coordinates and the size info on a 2D line
    /// </summary>
    public struct LineCoordinates
    {
        /// <summary>
        /// Gets the height of the line
        /// </summary>
        public double Height { get; }

        /// <summary>
        /// Gets the X coordinate of the line
        /// </summary>
        public double X { get; }

        /// <summary>
        /// Gets the Y coordinate of the line
        /// </summary>
        public double Y { get; }

        /// <summary>
        /// Creates a new instance for a target line to draw
        /// </summary>
        /// <param name="height">The line height</param>
        /// <param name="x">The line X position</param>
        /// <param name="y">The line Y position</param>
        public LineCoordinates(double height, double x, double y)
        {
            Height = height;
            X = x;
            Y = y;
        }
    }
}