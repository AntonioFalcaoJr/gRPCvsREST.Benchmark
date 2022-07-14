import http from 'k6/http';
import { check } from 'k6';

export const options = {
  vus: 50,
  duration: '60s',
}; 

export default function () {
  let res = http.get('https://localhost:7100/rest');
  check(res, { 'status was 200': (r) => r.status == 200 })
}