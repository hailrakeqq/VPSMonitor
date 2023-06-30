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

        const request = await fetch(`https://localhost:5081/api/Core/ExecuteCommand`, {
            method: "POST",
            headers: {
                'Authorization': `bearer ${localStorage.getItem('access-token')}`,
                'Content-Type': 'application/json'
            },
                body: JSON.stringify(objectToSend)
            })

                return await request.text()
            }
        //TODO no valid data exception
        return "data isn't valid"
    }

    /**
     * If using this function for make request to core controller, request body should look like this:
     * 
     * const body = {
     *  host: "host address(VPS address)",
     *  username: "username using for connect to VPS",
     *  password: "password for connect to VPS",
     *  command: "bash command to execute"
     * }
     * 
     * */
    static async httpRequest(method: string, url: string, itemToSend?: any, headers?: any): Promise<Response>{
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

        return await fetch(url, requestOptions)
    }
}
