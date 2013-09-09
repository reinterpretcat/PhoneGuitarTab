using System.Windows;
using System.Windows.Controls;

namespace PhoneGuitarTab.UI.Controls
{
    /// <summary>
    /// simple control for displaying 5 step rating
    /// without any interaction functions
    /// </summary>
    public partial class RatingControl : UserControl
    {
        #region Constructors
        
        public RatingControl()
        {
            InitializeComponent();
        }

        #endregion Constructors


        #region Properties

        #region RatingProperty

        public static readonly DependencyProperty RatingProperty = DependencyProperty.Register(
            "Rating", typeof(int),
            typeof(RatingControl), new PropertyMetadata(0, new PropertyChangedCallback(OnRatingChanged)));

        public int Rating
        {
            get { return (int)GetValue(RatingProperty); }
            set { SetValue(RatingProperty, value); }
        }

        private static void OnRatingChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            var src = (RatingControl)sender;
            src.SetStarStep((int)args.NewValue);
        }
        #endregion RatingProperty

        #endregion Properties


        #region Public methods

        /// <summary>
        /// Set stars in rating control
        /// </summary>
        /// <param name="starValue">star rating (step)</param>
        public void SetStarStep(int starValue)
        {
            RatingStar ratingStar = new RatingStar();
            ratingStar.Value = starValue;

            SetStarStep(ratingStar);
        }

        #endregion Public methods


        #region Helper methods

        /// <summary>
        /// Set stars in rating control
        /// </summary>
        /// <param name="star">object with rating state and rating step</param>
        private void SetStarStep(RatingStar star)
        {
            int counter = 1;

            foreach (RatingStar ratingStar in pnlStarContainer.Children)
            {
                ((RatingStar)ratingStar).State = counter <= star.Value
                                                      ? RatingStar.StarState.Filled
                                                      : RatingStar.StarState.Empty;

                counter++;
            }
        }

        #endregion Helper methods
    }
}
