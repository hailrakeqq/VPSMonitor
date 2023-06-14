export class HttpClient{

    static async httpExecuteBashCommandRequest(command: string): Promise<string> {
        const hostAddress = sessionStorage.getItem('host')?.split('@');
        const password = sessionStorage.getItem('password')

        if (hostAddress != undefined && password != undefined) {
            const username = hostAddress[0]
            const address = hostAddress[1];
            const objectToSend: Object = {
                host: address,
                username: username,
                password: password,
                command: command
            }

        const request = await fetch(`http://localhost:5081/api/Core/ExecuteCommand`, {
            method: "POST",
            headers: {
                'Authorization': `bearer ${localStorage.getItem('access-token')}`,
                'Content-Type': 'application/json'
            },
                body: JSON.stringify(objectToSend)
            })
        
                return await request.text()
            }

        return "data isn't valid"
    }

    //TODO: сделать заметку для использование core controller
    static async httpRequest(method: string, url: string, itemToSend?: any, headers?: any): Promise<any>{
        const requestOptions = {
            method: method,
            headers: headers,
            body: ''
        }

        if (headers != null) {
            requestOptions.headers = headers
        }
        if (itemToSend != null) {
            requestOptions.body = JSON.stringify(itemToSend)
        }
        

        const request = await fetch(url, requestOptions)
        let response = ''

        try {
            response = await request.json()
        } catch {
            response = await request.text()
        } finally {
            return response
        }
    }
}
    