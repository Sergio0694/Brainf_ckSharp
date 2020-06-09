namespace Brainf_ck_sharp.Legacy.UWP.PopupService.InternalTypes
{
    /// <summary>
    /// A struct that represents the 2D offset of a control
    /// </summary>
    public struct Offset
    {
        /// <summary>
        /// Gets the horizontal offset
        /// </summary>
        public double X { get; }

        /// <summary>
        /// Gets the vertical offset
        /// </summary>
        public double Y { get; }

        /// <summary>
        /// Creates a new instance with the given parameters
        /// </summary>
        /// <param name="x">The horizontal offset</param>
        /// <param name="y">The vertical offset</param>
        public Offset(double x, double y)
        {
            X = x;
            Y = y;
        }
    }
}