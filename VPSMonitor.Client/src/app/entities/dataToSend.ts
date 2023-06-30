interface dataToSend {
    host: string,
    username: string,
    password: string, 
    command?: string
}

interface newUserDataToSend{
    hostAddress: string | undefined,
    hostUsername: string | undefined,
    hostPassword: string | null, 
    userUsername: string,
    userPassword: string, 
    userConfirmPassword: string
}