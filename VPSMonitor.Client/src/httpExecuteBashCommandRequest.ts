export async function httpExecuteBashCommandRequest(command: string): Promise<string> {
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