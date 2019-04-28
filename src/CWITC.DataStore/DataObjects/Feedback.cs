namespace CWITC.DataObjects
{
    /// <summary>
    /// Per user feedback
    /// </summary>
    public class Feedback : BaseDataObject
    {
        public string SessionId { get; set; }

		public string SessionName { get; set; }

		public string SpeakerNames { get; set; }

        public double SessionRating { get; set; }

		public string FeedbackText { get; set; }
    }
}