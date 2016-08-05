using System;
using System.Diagnostics;

namespace TileUpdateBackgroundProcess
{
    public static class CommonData
    {
        public static TimeSpan GetRemainingTime()
        {
            var remainingTime = new TimeSpan();

            try
            {
                object weddingDateObject = null;

                if (Windows.Storage.ApplicationData.Current.RoamingSettings.Values["WeddingDate"] != null)
                {
                    weddingDateObject = Windows.Storage.ApplicationData.Current.RoamingSettings.Values["WeddingDate"];
                }

                var weddingDate = Convert.ToDateTime(weddingDateObject);

                if (weddingDate != null)
                {
                    var days = Convert.ToInt32((weddingDate - DateTime.Now).TotalDays);
                    var hours = Convert.ToInt32((weddingDate - DateTime.Now).Hours);

                    remainingTime = new TimeSpan(days, hours, 0, 0);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }

            return remainingTime;
        }
    }
}
