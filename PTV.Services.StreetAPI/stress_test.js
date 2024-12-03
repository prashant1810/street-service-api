import http from 'k6/http';
import { check, sleep } from 'k6';

export let options = {
    stages: [
        { duration: '5s', target: 5 },    
        { duration: '30s', target: 5 },   
        { duration: '5s', target: 50 },   
        { duration: '30s', target: 50 },  
        { duration: '5s', target: 100 },  
        { duration: '30s', target: 100 }, 
        { duration: '5s', target: 300 },  
        { duration: '30s', target: 300 }, 
        { duration: '5s', target: 0 },    
    ],
    thresholds: {
        http_req_duration: ['p(95)<1000'], 
        http_req_failed: ['rate<0.1'],    
    },
};

export default function () {
    var response = http.get('https://localhost:5001/api/Street');

    check(response, {
        'is status 200': (x) => x.status === 200
    });

    sleep(0.1);
}