JobSeeker - Real-World Problem It Solves
Searching for relevant job postings manually across multiple platforms can be time-consuming, repetitive, and error-prone. JobSeeker automates the process by:
Querying multiple job boards simultaneously
Filtering results based on your target roles and keywords
Providing instant feedback via API or scraping methods
This helps job seekers maximize opportunities while minimizing manual effort.

🛠 Tech Stack & Architecture
Language: C#
Framework: .NET 6 / Console Application

Libraries / Packages:

Newtonsoft.Json – for JSON parsing

System.Net.Http – for HTTP requests to APIs and web scraping

System.Text.RegularExpressions – for HTML content parsing

System.Web – for URL encoding of search queries

Integration & APIs:

Google Custom Search API – for querying job postings programmatically

Optional scraping fallback via Google Search HTML parsing

Design Concepts:

Modular architecture (JobSearchChecker class) for querying via API or scraping

Extensible job board list for easy addition of new platforms
