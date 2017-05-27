namespace NDS
{
    /// <summary>Represents the relationship a range has to another.</summary>
    public enum RangeRelationship
    {
        /// <summary>
        /// Other range is equal to this range.
        /// </summary>
        Equal,

        /// <summary>
        /// Start and end of other range occur before the start of this range.
        /// ( other ) ... [ this ]
        /// </summary>
        Before,

        /// <summary>
        /// Other range starts before this range but ends within it.
        /// ( other  [ this ) ]
        /// </summary>
        OverlapsStart,

        /// <summary>
        /// Other range is contained within this range.
        /// [ this ( other ) ]
        /// </summary>
        Within,

        /// <summary>
        /// Other range starts within this range but ends outside.
        /// [ this ( other ]  )
        /// </summary>
        OverlapsEnd,

        /// <summary>
        /// Other range begins and ends after thsi range.
        /// </summary>
        After,

        /// <summary>
        /// Other range begins before and ends after this range.
        /// </summary>
        Encloses
    }
}
