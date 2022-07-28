using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

var consoleLock = new object();
var httpClient = new HttpClient();
var stopwatch = new Stopwatch();
var requestCount = int.TryParse(args.FirstOrDefault(), out var parsed)
    ? parsed
    : 10;

var target = "https://www.google.com";

Console.WriteLine($"Performing {requestCount} sequential requests to {target}");
stopwatch.Start();
for (var i = 0; i < requestCount; i++)
{
    await HitGoogle();
}

stopwatch.Stop();
Console.WriteLine($"{requestCount} sequential requests completed in {stopwatch.Elapsed}");

Console.WriteLine($"Performing {requestCount} parallel requests to {target}");
var threads = new List<Thread>();

var barrier = new Barrier(requestCount + 1);
stopwatch.Reset();
stopwatch.Start();
for (var i = 0; i < requestCount; i++)
{
    var t = new Thread(() =>
    {
        var task = HitGoogle();
        task.ConfigureAwait(false);
        task.Wait();
    });
    t.Start();
    threads.Add(t);
}

Console.WriteLine("... waiting for all threads to complete ...");
foreach (var t in threads)
{
    t.Join();
}

stopwatch.Stop();
Console.WriteLine($"{requestCount} parallel requests completed in {stopwatch.Elapsed}");

async Task HitGoogle()
{
    var response = await httpClient.GetAsync(target);
    if (response.StatusCode != HttpStatusCode.OK)
    {
        lock (consoleLock)
        {
            Console.WriteLine($"Failed to fetch {target} [{response.StatusCode}]");
        }

        return;
    }

    // ensure we read all of the page
    var page = await response.Content.ReadAsStringAsync();
    lock (consoleLock)
    {
        Console.WriteLine($"Read {page.Length} bytes");
    }
}