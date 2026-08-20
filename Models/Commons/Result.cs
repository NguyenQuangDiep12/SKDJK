using System.Diagnostics;

namespace SKDJK.Models.commons
{
    public class Result
    {
        public bool IsSuccess { get; }
        public Error Error { get; }
        protected Result(
            bool IsSuccess,
            Error Error)
        {
            if(IsSuccess == true && Error != Error.None)
            {
                throw new InvalidOperationException("Ma trang thai thanh cong khong duoc co trang thai loi");
            }
            if(!IsSuccess && Error == Error.None)
            {
                throw new InvalidOperationException("Ma trang thai that bai phai chua loi");
            }
           this.IsSuccess = IsSuccess;
           this.Error = Error;
        }
        public static Result Success()
        {
            return new Result(true, Error.None);
        }
        public static Result Failure(Error error)
        {
            return new Result(false, error);
        }
    }

    public class Result<T> : Result
    {
        private readonly T? _value;
        public T Value
        {
            get
            {
                if(IsSuccess == false)
                {
                    throw new InvalidOperationException("Khong the lay value tu result that bai");
                }
                return this._value!;
            }
        }
        private Result(T? value, bool IsSuccess, Error error) : base(IsSuccess, error)
        {
            this._value = value;
        }

        public static Result<T> Success(T Value)
        {
            return new Result<T>(Value, true, Error.None);
        }
        public new static Result<T> Failure(Error error)
        {
            return new Result<T>(default(T),false, error);
        }
    }
}
