using Microsoft.Extensions.Logging;
using System;

namespace Server.Utils
{
    public static class Common
    {
        public static void TryCatchAction<TException>(Action action, Action<TException> excpetionHandler, ILogger logger = null) where TException : Exception
        {
            try
            {
                action.Invoke();
            }
            catch(TException ex)
            {
                if (logger != null) logger.LogError(ex.ToString());
                excpetionHandler.Invoke(ex);
            }
        }
    }
}
