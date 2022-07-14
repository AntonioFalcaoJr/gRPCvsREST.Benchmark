import http from 'k6/http';
import { check, sleep } from 'k6';

export const options = {
  vus: 50,
  duration: '60s',
}; 

export default function () {
  let res = http.post('https://localhost:7100/grpc');
  check(res, { 'status was 200': (r) => r.status == 200 })
}