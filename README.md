# downloaderapi
##


## How to run
* Install docker <a href="https://docs.docker.com/desktop/">Docker engine and docker compose</a></br>
* Run `docker compose -p all up` from `./backend/TestSolution`
* Open swagger on localhost:5062 <a href="http://localhost:5062/swagger/index.html">this link</a> 

# Desgin choices
Application uses a lighter version of Clean Arcitecture, and some elements of DDD.
I decided not to go with complex abstraction and inderection keeping application only 3 layers deep.
I split project by layers to make it easier to review, as it is a more conventional way to do it. Project can be easily regrouped by feature.
I cut some corners removing authentication, preview generation, retries and so on to reduce this task scope. 

# Ideas
* Add proper Ci/CD pieplines
* Move zipping/ preview generation in an outside async process
* Add paging
* Add Circuit Breaker and retry policies using something like Polly
* Introduce metrics collection
* Move configs to a config store, like AWS Secrets Manager, or a ConfigMap
* Add proper resource access authentication
* If it's a multiregional service we can use CDN, depends on how the service will be used
* If the API is mostly for service-to-service usage it makes sense to directly share files from S3 without restriming
* Add checksumm check
* Add e2e and integration tests
* Introduce some DDD concepts like value objects.