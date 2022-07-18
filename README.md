# gRPC vs REST Benchmark 

## Run the benchmark

```bash
cd ./test
```

### REST

```bash
docker run --network=internal --name k6 --rm -i grafana/k6 run - <usecase-rest-retrieve.js
```

```bash
docker run --network=internal --name k6 --rm -i grafana/k6 run - <usecase-rest-submission.js
```

### gRPC

```bash
docker run --network=internal --name k6 --rm -i grafana/k6 run - <usecase-rest-retrieve.js
```

```bash
docker run --network=internal --name k6 --rm -i grafana/k6 run - <usecase-rest-submission.js
```
