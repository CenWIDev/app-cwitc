namespace CWITC.DataObjects
{
    /// <summary>
    /// Per user feedback
    /// </summary>
    public class Feedback : BaseDataObject
    {
        public string ContentfulId { get; set; }

        public double SessionRating { get; set; }

		public string FeedbackText { get; set; }
    }
}