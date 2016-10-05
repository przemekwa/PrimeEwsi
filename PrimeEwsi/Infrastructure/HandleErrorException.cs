

namespace PrimeEwsi.Infrastructure
{
    using System;
    using System.Web.Mvc;
    using Models;

    /// <summary>
    /// Handle error exception
    /// </summary>
    public class HandleErrorException : FilterAttribute, IExceptionFilter
    {
        /// <summary>
        /// Method when exception occure
        /// </summary>
        /// <param name="filterContext">Filter context</param>
        public void OnException(ExceptionContext filterContext)
        {
            var errorModel = new ErrorModel("Wystąpił błąd. Skontaktuj się z administratorem");
            
            try
            {
                if (filterContext.ExceptionHandled)
                {
                    return;
                }

                filterContext.ExceptionHandled = true;

                errorModel = new ErrorModel(filterContext.Exception.Message);
            }
            catch (Exception e)
            {
                errorModel =
                    new ErrorModel($"Wystapił błąd podczas generowania modelu do blędu {e.Message}");
            }
            finally
            {
                filterContext.ExceptionHandled = true;

                filterContext.Result = new ViewResult
                {
                    ViewName = "Error",
                    ViewData = new ViewDataDictionary(errorModel)
                };
            }
        }
    }
}