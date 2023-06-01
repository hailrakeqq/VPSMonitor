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
}