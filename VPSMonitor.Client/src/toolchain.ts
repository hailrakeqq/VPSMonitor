export class Toolchain{
    static ValidateInputData(email: string, password: string, confirmPassword?: string): boolean {
        const emailPattern = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;

        if (email && password && this.ValidateInputEmail(email) && password.length >= 6) {
            if (confirmPassword) {
                return password === confirmPassword;
            } else {
                return true;
            }
        }
  
        return false;
    }
    
    static ValidateInputForChangePassword(newPassword: string, confirmPassword: string): boolean{
        if (newPassword !== confirmPassword && newPassword.length >= 6)
            return true;
        return false;
    }

    static ValidateInputEmail(email: string): boolean {
        const emailPattern = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
        
        if (emailPattern.test(email))
            return true
        return false;
    }
    static ValidateSessionStorageData():boolean {
        const sessionStorageData = { 
            host: sessionStorage.getItem('host'),
            password: sessionStorage.getItem('password')
        }
        if (sessionStorageData.host != "" && sessionStorageData.host != undefined &&
            sessionStorageData.password != "" && sessionStorageData.password != undefined)
            return true;
        
        return false;
    };
    static ValidateInputNewUserData(user: newUserDataToSend): boolean {
        if (user.userUsername != "" && user.userUsername != null &&
            user.userPassword != "" && user.userPassword != null &&
            user.userConfirmPassword != "" && user.userConfirmPassword != null &&
            user.userPassword === user.userConfirmPassword)
            return true;

        return false;
    }
}