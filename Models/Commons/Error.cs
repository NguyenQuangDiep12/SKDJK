namespace SKDJK.Models.commons
{
    public record Error(string Code, string Message)
    {

        // Auth Service 
        public static readonly Error InvalidInput = new Error("Auth.InvalidInput", "Thong tin khong hop le");
        public static readonly Error EmailAlreadyExist = new Error("Auth.EmailAlreadyExist", "Tai khoan email da ton tai");
        public static readonly Error InvalidCreadential = new Error("Auth.InvalidCreadential", "Email hoac mat khau khong hop le");
        // Mau trang thai Error None
        public static readonly Error None = new Error(string.Empty, string.Empty);
    }
}
