# How to run
## Run
* Install docker <a href="https://docs.docker.com/desktop/">Docker engine and Compose plugin</a></br>
* Run `docker compose --profile all up` from `./backend/DocumentStore`
* Open swagger on localhost:5062 <a href="http://localhost:5204/swagger/index.html">this link</a> 

## Debug
* Install <a href="https://dotnet.microsoft.com/en-us/download/dotnet/8.0">.NET8</a></br>
* Go to  `./backend/DocumentStore`
* Run `docker compose --profile infra up` to spin up dependancies
* Run `dotnet run --project DocumentStore.csproj`

# Design choices
* The application uses a lighter version of Clean Architecture and incorporates some elements of Domain-Driven Design (DDD). 
* I decided not to employ complex abstraction and indirection, keeping the application only three layers deep. 
* I split the project by layers to make it easier to review, as it is a more conventional approach. 
* The project can be easily regrouped by feature if necessary. 
* I simplified the implementation by removing authentication, preview generation, retries, and other features to reduce the task's scope.

# Ideas
* Add proper Ci/CD pieplines
* Move zipping/preview generation to an external asynchronous process
* Add paging
* Add Circuit Breaker and retry policies using something like Polly
* Introduce metrics collection
* Move configs to a config store, like AWS Secrets Manager, or a ConfigMap
* Add proper resource access authentication
* If it's a multiregional service, consider using a CDN, depending on how the service will be used
* If the API is primarily for service-to-service usage, it makes sense to directly share files from S3 without re-streaming
* Add checksum verification
* Add e2e and integration tests
* Introduce some DDD concepts like value objects.

# Shortcuts and assumptions 
I made several assumptions to reduce the scope of this task, as implementing a production-level service like this is very time consuming.
1. We do not want any paging.
2. Files are expected to be small, fitting within default limits, and will not require streaming.
3. There is no versioning or replacing of files; each upload is treated as a separate document.
4. Test code coverage is not a metric for the submission assessment; existing tests should be sufficient to provide an idea of my testing approach.
5. It is acceptable to remove rudimentary logging to make the code easier to review.
6. I used dummy previews instead of actual generation, which can be done with something like this: https://docs.groupdocs.com/viewer/net/licensing-and-evaluation/
7. It is fine to make small shortcuts, such as simpler error handling.
8. No frontend.
