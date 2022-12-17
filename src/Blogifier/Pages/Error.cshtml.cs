using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Serilog;

namespace Blogifier.Pages
{
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
   //////////// [IgnoreAntiforgeryToken]
    public class ErrorModel : PageModel
    {
        public string RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);

        private readonly ILogger _logger;

        public ErrorModel(ILogger logger)
        {
            _logger = logger;
        }

        public void OnGet(string? code)
        {
            if (null == code)
                code = HttpContext.Response.StatusCode.ToString();
            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
        }
    }
}
