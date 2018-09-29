﻿using System;
using Newtonsoft.Json;
namespace CWITC.DataObjects
{
    public class FeaturedEvent : BaseDataObject
    {
        /// <summary>
        /// Gets or sets the type of the event such as: offsite, session, break, etc
        /// </summary>
        /// <value>The type.</value>
        public string Type { get; set; }

        /// <summary>
        /// Gets or sets the title of the event such as: Evolve Keynote
        /// </summary>
        /// <value>The title.</value>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the descriptionof the event
        /// </summary>
        /// <value>The description.</value>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the start time.
        /// </summary>
        /// <value>The start time.</value>
        public DateTime? StartTime { get; set; }

        /// <summary>
        /// Gets or sets the end time.
        /// </summary>
        /// <value>The end time.</value>
        public DateTime? EndTime { get; set; }

        public string LocationName { get; set; }

        public string SponsorId { get; set; }

		[JsonIgnore]
        /// <summary>
        /// Gets or sets the sponsor if there is one for the event
        /// </summary>
        /// <value>The sponsor.</value>
        public virtual Sponsor Sponsor { get; set; }

        [JsonIgnore]
        public bool HasSponsor => Sponsor != null;

        [JsonIgnore]
        public DateTime StartTimeOrderBy { get { return StartTime.HasValue ? StartTime.Value : DateTime.MinValue; } }
    }
}

