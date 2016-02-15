using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WedChecker.Common;
using Windows.UI.Popups;

namespace WedChecker.Extensions
{
    public static class ExceptionExtensions
    {
        /// <summary>
        /// Handles failure for application exception on UI thread (or initiated from UI thread via async void handler)
        /// </summary>
        public static async Task HandleException(this Exception ex)
        {

            ex.LogException();

            var dialog = new MessageDialog(ex.GetDisplayMessage(), "Unknown Error");
            await dialog.ShowAsync();
        }

        public static void LogException(this Exception ex)
        {
            // e.g. MarkedUp.AnalyticClient.Error(ex.Message, ex);
        }

        /// <summary>
        /// Gets the error message to display from an exception
        /// </summary>
        public static string GetDisplayMessage(this Exception ex)
        {
            string errorMessage;
#if DEBUG
            errorMessage = (ex.Message + " " + ex.StackTrace);
#else
                errorMessage = "An unknown error has occurred, please try again";
#endif

            if (ex is WedCheckerInvalidDataException)
            {
                errorMessage = ex.Message;
            }

            return errorMessage;
        }
    }
}