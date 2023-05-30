export class Toolchain{
    static ValidateInputData(email: string, password: string, confirmPassword?: string): boolean {
        const emailPattern = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
        const uppercaseRegex = /[A-Z]/;
        // && uppercaseRegex.test(password)
        if (email && password && emailPattern.test(email) && password.length >= 6) {
            if (confirmPassword) {
                return password === confirmPassword;
            } else {
                return true;
            }
        }
  
        return false;
    }
}