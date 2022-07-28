Demo: Concurrent requests with HttpClient
---

This is a demo of using the same HttpClient concurrently.
HttpClient is thread-safe and doesn't store state,
so there's no problem with doing this.

Usage:
---
`npm start`

if you'd like to try larger request volumes, add `-- {some number}`, eg
`npm start -- 25`
