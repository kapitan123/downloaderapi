# downloaderapi
##


## How to run
* Install docker <a href="https://docs.docker.com/desktop/">Docker engine and docker compose</a></br>
* Run `docker compose --profile all up` from `./backend/TestSolution`
* Open swagger on localhost:5062 <a href="http://localhost:5204/swagger/index.html">this link</a> 

# Design choices
The application uses a lighter version of Clean Architecture and incorporates some elements of Domain-Driven Design (DDD). 
I decided not to employ complex abstraction and indirection, keeping the application only three layers deep. 
I split the project by layers to make it easier to review, as it is a more conventional approach. 
The project can be easily regrouped by feature if necessary. 
I simplified the implementation by removing authentication, preview generation, retries, and other features to reduce the task's scope.

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