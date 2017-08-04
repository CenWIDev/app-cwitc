using System;
namespace CWITC.iOS
{
	public abstract class RatingView : UIStackView, IMvxRatingView<UIView>
	{
		private bool _readonly;
		private int _selectedRating;

		public event EventHandler SelectedRatingChanged;
		public event EventHandler ReadOnlyChanged;

		public List<UIView> Views { get; set; }

		public int MaxRating { get; set; }

		public int SelectedRating
		{
			get { return _selectedRating; }
			set
			{
				if (_selectedRating.Equals(value))
				{
					return;
				}

				_selectedRating = value;
				RefreshViews();
			}
		}

		public bool ReadOnly
		{
			get { return _readonly; }
			set
			{
				if (_readonly.Equals(value))
				{
					return;
				}

				_readonly = value;
				if (_readonly)
				{
					AddOnClickListeners();
				}
				else
				{
					RemoveOnClickListeners();
				}
			}
		}

		protected MvxRatingView(IntPtr p)
			: base(p)
		{
			Initialise(5);
		}

		protected MvxRatingView(CGRect rect, int maxRating = 5)
			: base(rect)
		{
			Initialise(maxRating);
		}

		private void Initialise(int maxRating)
		{
			Views = new List<UIView>();
			Distribution = UIStackViewDistribution.EqualSpacing;
			MaxRating = maxRating;
			SetupView();
		}

		public void RefreshViews()
		{
			foreach (var mvxRatingViewItem in Views)
			{
				var rating = (int)mvxRatingViewItem.Tag;
				var isSelected = SelectedRating >= rating;

				UpdateView(mvxRatingViewItem, rating, isSelected);
			}
		}

		public void AddOnClickListeners()
		{
			Views?.ForEach(v =>
			{
				v.AddGestureRecognizer(new UITapGestureRecognizer(gr => OnClick(gr.View)));
			});
		}

		public void RemoveOnClickListeners(bool disposing = false)
		{
			Views?.ForEach(v =>
			{
				v.GestureRecognizers.ToList().ForEach(v.RemoveGestureRecognizer);

				if (!disposing)
					return;

				v.Dispose();
			});
		}

		public void OnClick(UIView view)
		{
			SelectedRating = (int)view.Tag;
			SelectedRatingChanged?.Invoke(this, EventArgs.Empty);
		}

		public abstract UIView GetRatingViewItem(int rating, bool isSelected);

		public int GetMaxRating()
		{
			return MaxRating;
		}

		public abstract void UpdateView(UIView view, int rating, bool isSelected);

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);

			if (!disposing) return;

			RemoveOnClickListeners(true);

			Views?.Clear();
			Views = null;
		}

		private void SetupView()
		{
			var maxRating = GetMaxRating();
			for (var i = 1; i <= maxRating; i++)
			{
				var isSelected = SelectedRating >= i;
				var ratingViewItem = GetRatingViewItem(i, isSelected);
				ratingViewItem.Tag = i;

				SetupRatingViewItem(ratingViewItem);
			}

			if (ReadOnly)
				return;

			AddOnClickListeners();
		}

		private void SetupRatingViewItem(UIView ratingViewItem)
		{
			Views.Add(ratingViewItem);
			AddArrangedSubview(ratingViewItem);
		}
	}
}
