using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace BTProb.Controllers
{
    public class BaseController : Controller
    {
        protected internal async Task<IActionResult> FromResult(Func<Task<object>> func)
        {
            if (!ModelState.IsValid)
            {
                return await Task.FromResult(BadRequest(ModelState));
            }

            try
            {
                var result = await func();

                return await Task.FromResult(Ok(result));
            }
            catch (Exception ex)
            {
                return await Task.FromResult(BadRequest(ex));
            }
        }
    }
}