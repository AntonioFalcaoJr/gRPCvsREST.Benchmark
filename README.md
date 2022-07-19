# gRPC vs REST Benchmark

This Benchmark aims to measure the efficiency of communication between services through REST versus gRPC. The specific scenario is a BFF that consumes data in a given service.

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
docker run --network=internal --name k6 --rm -i grafana/k6 run - <usecase-rest-retrieve.js
```

```bash
docker run --network=internal --name k6 --rm -i grafana/k6 run - <usecase-rest-submission.js
```

#### gRPC

```bash
docker run --network=internal --name k6 --rm -i grafana/k6 run - <usecase-rest-retrieve.js
```

```bash
docker run --network=internal --name k6 --rm -i grafana/k6 run - <usecase-rest-submission.js
```
