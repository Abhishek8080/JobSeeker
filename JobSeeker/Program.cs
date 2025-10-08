// See https://aka.ms/new-console-template for more information
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;
using System.Web;

using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;





class JobSearchChecker
{
    private static readonly string[] JobBoards = {
        "linkedin.com", "indeed.com", "computrabajo.com", "ziprecruiter.com",
        "jooble.org", "naukri.com", "seek.com", "glassdoor.com", "talent.com", "monster.com"
    };

    public static string GenerateGoogleQuery(string jobSnippetQ, string jobSnippetP)
    {
        var encodedSnippet = $"\"{jobSnippetQ}\" + {jobSnippetP}";
        var siteFilters = string.Join(" OR ", Array.ConvertAll(JobBoards, domain =>
            $"site:{domain} OR site:*.{domain}"));
        return $"({siteFilters}) {encodedSnippet}";
    }

   
    public static async Task<List<string>> GetJobsViaGoogleApiAsync(string jobSnippetQ, string jobSnippetP, string apiKey, string cx)
    {
        var query = GenerateGoogleQuery(jobSnippetQ, jobSnippetP);
        var encodedQuery = HttpUtility.UrlEncode(query);

        var url = $"https://www.googleapis.com/customsearch/v1?q={encodedQuery}&key={apiKey}&cx={cx}";

        using var client = new HttpClient();
        var response = await client.GetAsync(url);

        if (!response.IsSuccessStatusCode) return new List<string>();

        var json = await response.Content.ReadAsStringAsync();
        var obj = JObject.Parse(json);
        var items = obj["items"];

        var results = new List<string>();
        if (items != null)
        {
            foreach (var item in items)
            {
                string title = item["title"]?.ToString();
                string link = item["link"]?.ToString();
                results.Add($"{title} → {link}");
            }
        }

        return results;
    }

   
    public static async Task<bool> CheckViaScrapingAsync(string jobSnippetQ, string jobSnippetP)
    {
        var query = GenerateGoogleQuery(jobSnippetQ, jobSnippetP);
        var encodedQuery = HttpUtility.UrlEncode(query);
        var url = $"https://www.google.com/search?q={encodedQuery}";

        using var client = new HttpClient();
        client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64)");

        var html = await client.GetStringAsync(url);

        
        if (html.Contains("Our systems have detected unusual traffic"))
        {
            Console.WriteLine("[Scraping Error] Google blocked the request (CAPTCHA).");
            return false;
        }

        
        return Regex.IsMatch(html, "(<div class=\"tF2Cxc\"|<div class=\"g\"|<div class=\"yuRUbf\")");
    }
}

class App  
{
    static async Task Main(string[] args)
    {
        string jobTextQuote = "Software Engineer";
        string jobTextPlain = ">NET";
        //Use Your API key and Custom Search Engine ID
        string apiKey = "";
        string cx = "";

        var jobs = await JobSearchChecker.GetJobsViaGoogleApiAsync(jobTextQuote, jobTextPlain, apiKey, cx);

        if (jobs.Count > 0)
        {
            Console.WriteLine("Jobs found:");
            foreach (var job in jobs)
            {
                Console.WriteLine(job);
            }
        }
        else
        {
            Console.WriteLine("No jobs found.");
        }

        
        bool foundViaScraping = await JobSearchChecker.CheckViaScrapingAsync(jobTextQuote, jobTextPlain);
        Console.WriteLine($"Found via scraping: {foundViaScraping}");

        if (!foundViaScraping)
        {
            Console.WriteLine("No matching jobs found — check API credentials, query, or job board list.");
        }

        Console.ReadLine();
    }
}