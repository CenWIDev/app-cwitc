using System;
using Xamarin.Forms;
using CWITC.DataObjects;
using System.Threading.Tasks;
using Xamarin.Forms;
using System.Windows.Input;
using FormsToolkit;
using System.Collections.Generic;

namespace CWITC.Clients.Portable
{
    public class FeedbackViewModel : ViewModelBase
    {
        Session session;
        public Session Session
        {
            get { return session; }
            set { SetProperty(ref session, value); }
        }


        public FeedbackViewModel(INavigation navigation, Session session) : base(navigation)
        {
            Session = session;
        }

        ICommand  submitRatingCommand;
        public ICommand SubmitRatingCommand =>
            submitRatingCommand ?? (submitRatingCommand = new Command<double>(async (rating) => await ExecuteSubmitRatingCommandAsync(rating))); 

        async Task ExecuteSubmitRatingCommandAsync(double rating)
        {
            if(IsBusy)
                return;

            IsBusy = true;
            try
            {
                if(rating == 0)
                {
                    
                    MessagingService.Current.SendMessage<MessagingServiceAlert>(MessageKeys.Message, new MessagingServiceAlert
                        {
                            Title = "No Rating Selected",
                            Message = "Please select a rating to leave feedback for this session.",
                            Cancel = "OK"
                        });
                        return;
                }

                Logger.Track(EvolveLoggerKeys.LeaveFeedback, "Title", rating.ToString());
                
                MessagingService.Current.SendMessage<MessagingServiceAlert>(MessageKeys.Message, new MessagingServiceAlert
                    {
                        Title = "Feedback Received",
                        Message = "Thanks for the feedback, have a great CWITC.",
                        Cancel = "OK",
                        OnCompleted = async () => 
                        {
                            await Navigation.PopModalAsync ();
                            if (Device.OS == TargetPlatform.Android)
                                MessagingService.Current.SendMessage ("eval_finished");
                        }
                    });

                Session.FeedbackLeft = true;
                await StoreManager.FeedbackStore.InsertAsync(new Feedback
                    {
                        SessionId = session.Id,
                        SessionRating = rating
                    });
            }
            catch(Exception ex)
            {
                Logger.Report(ex);
            }
            finally
            {
                IsBusy = false;
            }
        }


    }
}

