import { Injectable } from '@angular/core';

@Injectable()
export class AuthErrorInterceptor {
    
    interceptRequest(request: Request): Request {
        return request;
    }

    interceptResponse(response: Response): Response {
 
        if (response.status === 401 || response.status === 404) {
            this.handleAuthError();
        }
        return response;
    }

    private handleAuthError(): void{
        const refreshToken = localStorage.getItem('refresh-token')
        const id = localStorage.getItem('id')

        if (refreshToken == null && id == null)
            window.location.href = '/signin'
        else 
            this.requestToUpdateAccessToken();
    }

    private async requestToUpdateAccessToken(): Promise<void> {
        const userId = localStorage.getItem('id') || '';
        const refreshToken = localStorage.getItem('refresh-token') || '';
        const request = await fetch(`http://localhost:5081/api/Auth/refresh-token`, {
            method: "POST",
            headers: {
                'Content-Type': 'application/json',
                'userId': userId,   
                'refreshToken': refreshToken
            }
        })
        if (request.status == 200) {
            var newAccessToken = await request.text()
            localStorage.setItem('access-token', newAccessToken)
        }   
    }
}

const authErrorInterceptor = new AuthErrorInterceptor();

const originalFetch = window.fetch;

window.fetch = function (input: RequestInfo | URL, init?: RequestInit): Promise<Response> {
  const request = new Request(input, init);

  const interceptedRequest = authErrorInterceptor.interceptRequest(request);

  return originalFetch(interceptedRequest)
    .then((response) => authErrorInterceptor.interceptResponse(response))
    .catch((error) => {
      console.error('Fetch error:', error);
      throw error;
    });
};
