import http from 'k6/http';
import { check, sleep } from 'k6';

export let options = {
    stages: [
        { duration: '1m', target: 100 },
        { duration: '3m', target: 100 },
        { duration: '1m', target: 0 },
    ],
    thresholds: {
        http_req_duration: ['p(95)<300'],
    },
};

export default function () {

    var response = http.get('https://localhost:5001/api/Street');

    check(response, {
        'is status 200': (x) => x.status === 200
    });

    sleep(1);
}
