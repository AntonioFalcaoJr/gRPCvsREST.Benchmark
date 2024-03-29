import http from 'k6/http';
import { check } from 'k6';

export const options = {
  vus: 100,
  duration: '60s',
};

function getHost() {
  let protocol = __ENV.HTTPS ? "https" : "http";
  let host = __ENV.HOST || "localhost:7000";
  return `${protocol}://${host}`;
}

export default function () {
  let res = http.get(`http://webapi/rest/health`);
  check(res, { 'status was 200': (r) => r.status === 200 })
}