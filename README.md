# gRPC vs REST Benchmark

This Benchmark aims to measure the efficiency of communication between services through REST versus gRPC. The specific scenario is a BFF that consumes data in a given service.

## Scenario

[**Unary RPC call**](https://grpc.io/docs/what-is-grpc/core-concepts/#unary-rpc)

## Performance best practices with gRPC

### Client-side load balancing

With client-side load balancing, the client knows about endpoints. For each gRPC call, it selects a different endpoint to send the call to.

#### DnsResolverFactory

```text
dns:///my-example-host
```
### Reuse gRPC channels

[**gRPC client factory**](https://docs.microsoft.com/en-us/aspnet/core/grpc/clientfactory?view=aspnetcore-6.0) offers a centralized way to configure channels. It automatically reuses underlying channels.

Each call will require multiple network round-trips between the client and the server to create a new HTTP/2 connection:

1. Opening a socket
2. Establishing TCP connection
3. Negotiating TLS
4. Starting HTTP/2 connection
5. Making the gRPC call

### Connection concurrency

HTTP/2 connections typically have a limit on the number of maximum concurrent streams on a connection at one time.

```c#
new SocketsHttpHandler
{
    EnableMultipleHttp2Connections = true
}
```

### Keep alive pings

Having an existing HTTP/2 connection ready when an app resumes activity allows for the initial gRPC calls to be made quickly, without a delay caused by the connection being reestablished.

```c#
new SocketsHttpHandler
{
    PooledConnectionIdleTimeout = Timeout.InfiniteTimeSpan,
    KeepAlivePingDelay = TimeSpan.FromSeconds(60),
    KeepAlivePingTimeout = TimeSpan.FromSeconds(30),
}
```

## Running

The respective [./docker-compose.yaml](./docker-compose.yaml) will provision all system dependencies, with the minimal resources needed for the benchmark:

```bash
docker-compose up -d
```

### Run the benchmark

```bash
cd ./test
```

#### REST

```bash
docker run --network=internal --name k6 --rm -i grafana/k6 run - <usecase-rest-health.js
```

```bash
docker run --network=internal --name k6 --rm -i grafana/k6 run - <usecase-rest-retrieve.js
```

```bash
docker run --network=internal --name k6 --rm -i grafana/k6 run - <usecase-rest-submission.js
```

#### gRPC

```bash
docker run --network=internal --name k6 --rm -i grafana/k6 run - <usecase-grpc-health.js
```

```bash
docker run --network=internal --name k6 --rm -i grafana/k6 run - <usecase-grpc-retrieve.js
```

```bash
docker run --network=internal --name k6 --rm -i grafana/k6 run - <usecase-grpc-submission.js
```

## References

* [gRPC - A high performance, open source universal RPC framework](https://grpc.io/docs/languages/csharp/quickstart/#run)
* [Performance best practices with gRPC](https://docs.microsoft.com/en-us/aspnet/core/grpc/performance?view=aspnetcore-6.0)