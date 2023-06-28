import { Injectable } from '@angular/core';

@Injectable()
export class AuthErrorInterceptor {
    
    interceptRequest(request: Request): Request {
        return request;
    }

    async interceptResponse(response: Response, originalRequestMethod: string): Promise<Response> {
        if (response.status === 401 || response.status === 404) {
            return await this.handleAuthError(response, originalRequestMethod);
        }
        return response;
    }

    private handleAuthError(response: Response, originalRequestMethod: string): Promise<Response> {
        const refreshToken = localStorage.getItem('refresh-token');
        const id = localStorage.getItem('id');

        if (refreshToken == null && id == null) {
            window.location.href = '/signin';
            return Promise.reject(response);
        } else {
            return this.requestToUpdateAccessToken()
                .then(() => this.retryOriginalRequest(response, originalRequestMethod))
                .catch((error) => {
                    console.error('Access token update error:', error);
                    throw response;
                });
        }
    }

    private async requestToUpdateAccessToken(): Promise<void> {        
        const userId = localStorage.getItem('id') || '';
        const refreshToken = localStorage.getItem('refresh-token') || '';
        const request = await fetch(`https://localhost:5081/api/Auth/refresh-token`, {
            method: "POST",
            headers: {
                'Content-Type': 'application/json',
                'userId': userId,   
                'refreshToken': refreshToken
            }
        });
        
        if (request.status === 200) {
            const newAccessToken = await request.text();
            localStorage.setItem('access-token', newAccessToken);
        } else {
            throw new Error('Access token update failed.');
        }
    }

    private retryOriginalRequest(originalResponse: Response, originalRequestMethod: string): Promise<Response> {
        const originalRequest = new Request(originalResponse.url);
        const requestOptions: RequestInit = {
            method: originalRequestMethod,
            headers: originalRequest.headers,
            body: originalRequest.body
        };
        return fetch(originalRequest, requestOptions);
    }
}

const authErrorInterceptor = new AuthErrorInterceptor();
const originalFetch = window.fetch;

window.fetch = async function (input: RequestInfo | URL, init?: RequestInit): Promise<Response> {
    const request = new Request(input, init);
    const interceptedRequest = authErrorInterceptor.interceptRequest(request);

    try {
        const response = await originalFetch(interceptedRequest);
        return authErrorInterceptor.interceptResponse(response, request.method);
    } catch (error) {
        console.error('Fetch error:', error);
        throw error;
    }
};