using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Server.Utils
{
    public static class Common
    {
        public static bool TryCatchAction(Action action, ILogger logger, string exceptionMessage)
            => TryCatchFunc<bool>(() => { action.Invoke(); return true; }, logger, exceptionMessage);

        public static bool TryCatchAction<TException>(Action action, Action<TException> excpetionHandler = null, ILogger logger = null) where TException : Exception
           => TryCatchFunc<bool, TException>(() =>
           {
               action.Invoke();
               return true;
           },
           (ex) =>
           {
               logger?.LogError(ex.Message);
               excpetionHandler?.Invoke(ex);
               return false;
           });

        public static TResult TryCatchFunc<TResult>(Func<TResult> func, ILogger logger, string exceptionMessage)
        {
            try
            {
                return func.Invoke();
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, exceptionMessage);
                return default(TResult);
            }
        }

        public static TResult TryCatchFunc<TResult, TException>(Func<TResult> func, Func<TException, TResult> exceptionHandler, ILogger logger = null) where TException : Exception
        {
            try
            {
                return func.Invoke();
            }
            catch (TException ex)
            {
                logger?.LogError(ex.Message);
                return exceptionHandler.Invoke(ex);
            }
            catch (Exception unhadledException)
            {
                logger?.LogError("Unhandled exception occured in TryCatchFunc: {0}{1}", Environment.NewLine, unhadledException.ToString());
                return default(TResult);
            }
        }

    }
}
