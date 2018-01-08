using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.ApplicationInsights;

namespace AppInsightsDemo.Controllers
{
    public class ErrorController : Controller
    {
        private IConfiguration config;
        public ErrorController(IConfiguration configuration)
        {
            config = configuration;
        }

        [Route("error")]
        public ActionResult Index(int primesLessThan = 100)
        {
            var tc = new TelemetryClient();
            tc.TrackMetric("PrimesLessThan", primesLessThan);

            tc.TrackEvent("GetPrimes", new Dictionary<string, string> { { "primesLessThan", primesLessThan.ToString() } }, new Dictionary<string, double> { });

            CheckIfErrorCondition();

            var primes = GetPrimes(primesLessThan);

            return View(primes);
        }

        private IEnumerable<int> GetPrimes(int primesLessThan)
        {
            var results = new Dictionary<int, bool>();
            for (int i = 2; i < primesLessThan; i++)
            {
                var isPrime = IsPrime(i);
                results.Add(i, isPrime);
            }

            return results.Where(x => x.Value).Select(x => x.Key);
        }

        private bool IsPrime(int number)
        {
            var isPrime = true;
            for (int i = 2; i < number; i++)
            {
                if (number % i == 0)
                {
                    isPrime = false;
                }
            }
            return isPrime;
        }

        private void CheckIfErrorCondition()
        {
            var errorsPer100 = config.GetValue<int>("errorsPer100");
            var rnGesus = new Random();
            if (rnGesus.Next(100) < errorsPer100)
            {
                throw new InvalidOperationException("Some Random Error with a Useless Message");
            }
        }
    }
}