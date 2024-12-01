import http from 'k6/http';
import { sleep, check } from 'k6';
import { Rate } from 'k6/metrics';

// Custom metrics
const failureRate = new Rate('failed_requests');

// Test configuration
export const options = {
    scenarios: {
        constant_load: {
            executor: 'constant-arrival-rate',
            rate: 1000,              // 200 iterations per second
            timeUnit: '1s',         // 1 second
            duration: '10m',         // Test duration: 5 minutes
            preAllocatedVUs: 100,    // Initial pool of VUs
            maxVUs: 200,           // Maximum number of VUs to scale to
        },
    }
};

const BASE_URL = 'http://host.docker.internal:5069'; // Replace with your API endpoint

export default function () {
    // Main request
    const response = http.get(`${BASE_URL}/send-sqs-message`);
    
    // Check if request was successful
    const success = check(response, {
        'status is 200': (r) => r.status === 200,
        'response time < 500ms': (r) => r.timings.duration < 500,
    });
    
    failureRate.add(!success);

    // Add some randomized sleep time between requests (50-150ms)
    sleep(Math.random() * 0.1 + 0.05);
}